// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;
using Foundation;
using ObjCRuntime;
using ReactiveUI.Internal;

#if UIKIT
using UIKit;
#else
using AppKit;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>An <see cref="ICreatesCommandBinding"/> implementation that binds commands using Cocoa's Target/Action mechanism.</summary>
/// <remarks>
/// <para>
/// Many Cocoa controls (buttons, menu items, toolbar items, etc.) participate in the Target/Action pattern.
/// This binder sets the control's <c>Target</c> and (when present) <c>Action</c> properties to route UI
/// invocations to an <see cref="ICommand"/>.
/// </para>
/// <para>
/// Trimming/AOT: the Target/Action path reflects over an unknown runtime type to locate properties named
/// <c>Target</c>, <c>Action</c>, and optionally <c>Enabled</c>. This is not trimming-safe and is annotated accordingly.
/// Prefer the add/remove handler overloads on <see cref="ICreatesCommandBinding"/> where applicable.
/// </para>
/// </remarks>
public class TargetActionCommandBinder : ICreatesCommandBinding
{
    /// <summary>The affinity score returned when the object type is a valid Target/Action host.</summary>
    private const int TargetActionAffinity = 4;

    /// <summary>The Cocoa property name whose setter routes control invocations to the bound command.</summary>
    private const string ActionPropertyName = "Action";

#if UIKIT
    /// <summary>The set of Cocoa types that are valid Target/Action hosts in UIKit builds.</summary>
    private static readonly Type[] ValidTypes = [typeof(UIControl)];
#else
    /// <summary>The set of Cocoa types that are valid Target/Action hosts in AppKit builds.</summary>
    private static readonly Type[] ValidTypes =
    [
        typeof(NSControl),
        typeof(NSCell),
        typeof(NSMenu),
        typeof(NSMenuItem),
        typeof(NSToolbarItem),
    ];
#endif

    /// <summary>Cache of runtime property setters used to apply Target/Action/Enabled on Cocoa objects.</summary>
    /// <remarks>
    /// <para>
    /// This is stable type metadata and is therefore cached indefinitely. Eviction provides no value here and
    /// would re-trigger reflection and setter generation.
    /// </para>
    /// <para>
    /// A <see langword="null"/> <c>Action</c> setter indicates the type does not expose an <c>Action</c> property.
    /// An <c>Enabled</c> setter is optional.
    /// </para>
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, Setters> PropertySetterCache = new();

    /// <inheritdoc/>
    /// <typeparam name="T">The candidate target type.</typeparam>
    /// <param name="hasEventTarget">A value indicating whether an explicit event target was supplied.</param>
    /// <returns>The affinity score for binding a command to the candidate type.</returns>
    public int GetAffinityForObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(bool hasEventTarget)
    {
        if (hasEventTarget)
        {
            return 0;
        }

        var t = typeof(T);
        for (var i = 0; i < ValidTypes.Length; i++)
        {
            if (ValidTypes[i].IsAssignableFrom(t))
            {
                return TargetActionAffinity;
            }
        }

        return 0;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// This binds via Target/Action, not via a .NET event. It requires that the runtime type exposes a <c>Target</c>
    /// property and that a selector named <c>theAction:</c> can be invoked on the target.
    /// </para>
    /// <para>
    /// If the runtime type also exposes an <c>Enabled</c> property, it is synchronized with
    /// <see cref="ICommand.CanExecute(object?)"/> and <see cref="ICommand.CanExecuteChanged"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The target type being bound.</typeparam>
    /// <param name="command">The command to bind to the target.</param>
    /// <param name="target">The target object to bind the command to.</param>
    /// <param name="commandParameter">An observable providing the latest command parameter.</param>
    /// <returns>A disposable that tears down the binding, or <see langword="null"/> when binding is not possible.</returns>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicEvents |
            DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        if (command is null)
        {
            return Scope.Empty;
        }

        commandParameter ??= Signal.Emit<object?>(target);

        object? latestParam = null;

        var ctlDelegate = CreateControlDelegate(command, () => Volatile.Read(ref latestParam));

        // Selector name must match [Export] on ControlDelegate.
        var selector = new Selector("theAction:");

        var runtimeType = target.GetType();
        var setters = PropertySetterCache.GetOrAdd(runtimeType, BuildSetters);

        // Apply Action (if present) and Target (required).
        setters.ActionSetter?.Invoke(target, selector, null);
        setters.TargetSetter.Invoke(target, ctlDelegate, null);

        // Ensure we always detach target (and action if applicable) on dispose.
        var detach = Scope.Create(
            (Setters: setters, Target: target),
            static state =>
            {
                // Clear Target first to stop invocation, then clear Action (if available).
                state.Setters.TargetSetter.Invoke(state.Target, null, null);
                state.Setters.ActionSetter?.Invoke(state.Target, null, null);
            });

        // If Enabled isn't supported, binding is complete.
        if (setters.EnabledSetter is null)
        {
            // Still track parameters so command execution uses the latest, but do not attempt Enabled sync.
            return new MultipleDisposable(
                detach,
                commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParam, x))));
        }

        // Initial enabled state.
        setters.EnabledSetter.Invoke(target, command.CanExecute(Volatile.Read(ref latestParam)), null);
        ctlDelegate.IsEnabled = command.CanExecute(Volatile.Read(ref latestParam));

        // Keep Enabled (and AppKit validate) in sync with CanExecuteChanged.
        var canExecuteChangedSub = new FromEventObservable<bool>(onNext =>
            {
                EventHandler handler = (_, _) =>
                    onNext(command.CanExecute(Volatile.Read(ref latestParam)));
                command.CanExecuteChanged += handler;
                return new ActionDisposable(() => command.CanExecuteChanged -= handler);
            })
            .Subscribe(new DelegateObserver<bool>(x =>
            {
                setters.EnabledSetter.Invoke(target, x, null);
                ctlDelegate.IsEnabled = x;
            }));

        return new MultipleDisposable(
            detach,
            commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParam, x))),
            canExecuteChangedSub);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This overload binds to a named .NET event. It is reflection-based and therefore not trimming-safe.
    /// Prefer the add/remove handler overload when you can supply delegates.
    /// </remarks>
    /// <typeparam name="T">The target type being bound.</typeparam>
    /// <typeparam name="TEventArgs">The event args type of the named event.</typeparam>
    /// <param name="command">The command to bind to the target.</param>
    /// <param name="target">The target object to bind the command to.</param>
    /// <param name="commandParameter">An observable providing the latest command parameter.</param>
    /// <param name="eventName">The name of the event to subscribe to.</param>
    /// <returns>A disposable that tears down the binding, or <see langword="null"/> when binding is not possible.</returns>
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        string eventName)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        if (command is null)
        {
            return Scope.Empty;
        }

        ArgumentExceptionHelper.ThrowIfNull(eventName);

        commandParameter ??= Signal.Emit<object?>(target);

        object? latestParam = null;

        // Stable handler for deterministic unsubscription is provided by EventPatternObservable.
        var evt = new EventPatternObservable<TEventArgs>(target, eventName);

        var paramSub = commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParam, x)));
        var evtSub = evt.Subscribe(new DelegateObserver<TEventArgs>(_ =>
        {
            var param = Volatile.Read(ref latestParam);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        }));

        return new MultipleDisposable(paramSub, evtSub);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This overload is fully AOT-compatible and should be preferred when an explicit event subscription API is available.
    /// </remarks>
    /// <typeparam name="T">The target type being bound.</typeparam>
    /// <typeparam name="TEventArgs">The event args type of the subscribed event.</typeparam>
    /// <param name="command">The command to bind to the target.</param>
    /// <param name="target">The target object to bind the command to.</param>
    /// <param name="commandParameter">An observable providing the latest command parameter.</param>
    /// <param name="addHandler">Adds the event handler to the target.</param>
    /// <param name="removeHandler">Removes the event handler from the target.</param>
    /// <returns>A disposable that tears down the binding, or <see langword="null"/> when binding is not possible.</returns>
    public IDisposable? BindCommandToObject<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicEvents |
            DynamicallyAccessedMemberTypes.NonPublicEvents)] T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler,
        Action<EventHandler<TEventArgs>> removeHandler)
        where T : class
        where TEventArgs : EventArgs
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(addHandler);
        ArgumentExceptionHelper.ThrowIfNull(removeHandler);

        if (command is null)
        {
            return Scope.Empty;
        }

        commandParameter ??= Signal.Emit<object?>(target);

        object? latestParam = null;

        EventHandler<TEventArgs> handler = (_, _) =>
        {
            var param = Volatile.Read(ref latestParam);
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        };

        var paramSub = commandParameter.Subscribe(new DelegateObserver<object?>(x => Volatile.Write(ref latestParam, x)));
        addHandler(handler);

        return new MultipleDisposable(
            paramSub,
            Scope.Create((RemoveHandler: removeHandler, Handler: handler), static state => state.RemoveHandler(state.Handler)));
    }

    /// <summary>Creates and caches property setters required for Target/Action binding on the specified runtime type.</summary>
    /// <param name="type">The runtime type to inspect.</param>
    /// <returns>The cached setter bundle.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the runtime type does not expose a required <c>Target</c> property.</exception>
    [RequiresUnreferencedCode("Cocoa Target/Action binding reflects over runtime types to locate properties that may be removed by trimming.")]
    private static Setters BuildSetters(Type type)
    {
        var actionProp = type.GetRuntimeProperty(ActionPropertyName);
        var targetProp = type.GetRuntimeProperty("Target");
        var enabledProp = type.GetRuntimeProperty("Enabled");

        if (targetProp is null)
        {
            throw new InvalidOperationException(
                $"Target property is required for {nameof(TargetActionCommandBinder)} on type {type.FullName}.");
        }

        return new(
            ActionSetter: actionProp is not null ? Reflection.GetValueSetterOrThrow(actionProp) : null,
            TargetSetter: Reflection.GetValueSetterOrThrow(targetProp),
            EnabledSetter: enabledProp is not null ? Reflection.GetValueSetterForProperty(enabledProp) : null);
    }

    /// <summary>Creates the Cocoa <see cref="ControlDelegate"/> that routes UI actions to <paramref name="command"/> using the latest parameter.</summary>
    /// <param name="command">The command invoked when the control fires the bound selector.</param>
    /// <param name="readLatestParam">Reads the most recent command parameter to pass to the command.</param>
    /// <returns>The configured delegate to install as the control's Target.</returns>
    private static ControlDelegate CreateControlDelegate(ICommand command, Func<object?> readLatestParam)
    {
        // Cocoa routes UI actions to a selector on the target; we provide a stable NSObject instance.
        var ctlDelegate = new ControlDelegate(static _ => { })
        {
            // IsEnabled is used on AppKit to validate menu items; keep it aligned with CanExecute.
            IsEnabled = command.CanExecute(null),
        };

        // Avoid capturing in the Export method; store the block on the delegate.
        ctlDelegate.SetBlock(_ =>
        {
            var param = readLatestParam();
            if (!command.CanExecute(param))
            {
                return;
            }

            command.Execute(param);
        });

        return ctlDelegate;
    }

    /// <summary>Represents the set of cached setters required to wire Target/Action and optionally Enabled.</summary>
    /// <param name="ActionSetter">The setter for the <c>Action</c> property, or <see langword="null"/> when the type has no <c>Action</c> property.</param>
    /// <param name="TargetSetter">The setter for the required <c>Target</c> property.</param>
    /// <param name="EnabledSetter">The setter for the optional <c>Enabled</c> property, or <see langword="null"/> when the type has no <c>Enabled</c> property.</param>
    private readonly record struct Setters(
        Action<object?, object?, object?[]?>? ActionSetter,
        Action<object?, object?, object?[]?> TargetSetter,
        Action<object?, object?, object?[]?>? EnabledSetter);

    /// <summary>Delegate object installed as the Cocoa Target for the <c>theAction:</c> selector.</summary>
    /// <remarks>
    /// This object must remain alive for the binding lifetime; it is referenced by the bound control's <c>Target</c>.
    /// </remarks>
    private sealed class ControlDelegate : NSObject
    {
        /// <summary>The action block invoked when the Cocoa control fires the bound selector.</summary>
        private Action<NSObject> _block;

        /// <summary>Initializes a new instance of the <see cref="ControlDelegate"/> class.</summary>
        /// <param name="block">The action invoked when the control fires the bound selector.</param>
        public ControlDelegate(Action<NSObject> block) => _block = block;

        /// <summary>Gets or sets a value indicating whether the command is currently executable.</summary>
        /// <remarks>
        /// On AppKit, this is used by <c>validateMenuItem:</c> to control menu item enabled state.
        /// </remarks>
        public bool IsEnabled { get; set; }

        /// <summary>Replaces the action invoked by <see cref="TheAction(NSObject)"/>.</summary>
        /// <param name="block">The new block to invoke.</param>
        public void SetBlock(Action<NSObject> block) => _block = block;

        /// <summary>Selector invoked by Cocoa controls for Target/Action.</summary>
        /// <param name="sender">The sender object.</param>
        [Export("theAction:")]
        private void TheAction(NSObject sender) => _block(sender);

#if !UIKIT
        /// <summary>AppKit menu item validation hook used to enable/disable menu items.</summary>
        /// <param name="menuItem">The menu item being validated.</param>
        /// <returns><see langword="true"/> if the item should be enabled; otherwise <see langword="false"/>.</returns>
        [Export("validateMenuItem:")]
        private bool ValidateMenuItem(NSMenuItem menuItem) => IsEnabled;
#endif
    }
}
