// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#nullable enable

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Foundation;

using ObjCRuntime;

#if UIKIT
using UIKit;
#else
using AppKit;
#endif

namespace ReactiveUI;

/// <summary>
/// An <see cref="ICreatesCommandBinding"/> implementation that binds commands using Cocoa's
/// Target/Action mechanism.
/// </summary>
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
#if UIKIT
    /// <summary>
    /// The set of Cocoa types that are valid Target/Action hosts in UIKit builds.
    /// </summary>
    private static readonly Type[] ValidTypes = [typeof(UIControl)];
#else
    /// <summary>
    /// The set of Cocoa types that are valid Target/Action hosts in AppKit builds.
    /// </summary>
    private static readonly Type[] ValidTypes =
    [
        typeof(NSControl),
        typeof(NSCell),
        typeof(NSMenu),
        typeof(NSMenuItem),
        typeof(NSToolbarItem),
    ];
#endif

    /// <summary>
    /// Cache of runtime property setters used to apply Target/Action/Enabled on Cocoa objects.
    /// </summary>
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
                return 4;
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
    [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
    public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter)
        where T : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);

        // Match other binders: null command means "no binding".
        if (command is null)
        {
            return Disposable.Empty;
        }

        commandParameter ??= Observable.Return((object?)target);

        object? latestParam = null;

        // Cocoa routes UI actions to a selector on the target; we provide a stable NSObject instance.
        var ctlDelegate = new ControlDelegate(static _ => { })
        {
            // IsEnabled is used on AppKit to validate menu items; keep it aligned with CanExecute.
            IsEnabled = command.CanExecute(null),
        };

        // Avoid capturing in the Export method; store the block on the delegate.
        ctlDelegate.SetBlock(_ =>
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        });

        // Selector name must match [Export] on ControlDelegate.
        var selector = new Selector("theAction:");

        var runtimeType = target.GetType();
        var setters = PropertySetterCache.GetOrAdd(runtimeType, static t => BuildSetters(t));

        // Apply Action (if present) and Target (required).
        setters.ActionSetter?.Invoke(target, selector, null);
        setters.TargetSetter.Invoke(target, ctlDelegate, null);

        // Ensure we always detach target (and action if applicable) on dispose.
        var detach = Disposable.Create(() =>
        {
            // Clear Target first to stop invocation, then clear Action (if available).
            setters.TargetSetter.Invoke(target, null, null);
            setters.ActionSetter?.Invoke(target, null, null);
        });

        // If Enabled isn't supported, binding is complete.
        if (setters.EnabledSetter is null)
        {
            // Still track parameters so command execution uses the latest, but do not attempt Enabled sync.
            return new CompositeDisposable(
                detach,
                commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x)));
        }

        // Initial enabled state.
        setters.EnabledSetter.Invoke(target, command.CanExecute(Volatile.Read(ref latestParam)), null);
        ctlDelegate.IsEnabled = command.CanExecute(Volatile.Read(ref latestParam));

        // Keep Enabled (and AppKit validate) in sync with CanExecuteChanged.
        var canExecuteChangedSub = Observable.FromEvent<EventHandler, bool>(
                eventHandler =>
                {
                    void Handler(object? s, EventArgs e) =>
                        eventHandler(command.CanExecute(Volatile.Read(ref latestParam)));
                    return Handler;
                },
                h => command.CanExecuteChanged += h,
                h => command.CanExecuteChanged -= h)
            .Subscribe(x =>
            {
                setters.EnabledSetter.Invoke(target, x, null);
                ctlDelegate.IsEnabled = x;
            });

        return new CompositeDisposable(
            detach,
            commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x)),
            canExecuteChangedSub);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This overload binds to a named .NET event. It is reflection-based and therefore not trimming-safe.
    /// Prefer the add/remove handler overload when you can supply delegates.
    /// </remarks>
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
            return Disposable.Empty;
        }

        ArgumentExceptionHelper.ThrowIfNull(eventName);

        commandParameter ??= Observable.Return((object?)target);

        object? latestParam = null;

        // Stable handler for deterministic unsubscription is provided by Rx's FromEventPattern.
        var evt = Observable.FromEventPattern<TEventArgs>(target, eventName);

        var paramSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));
        var evtSub = evt.Subscribe(_ =>
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        });

        return new CompositeDisposable(paramSub, evtSub);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This overload is fully AOT-compatible and should be preferred when an explicit event subscription API is available.
    /// </remarks>
    public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T, TEventArgs>(
        ICommand? command,
        T? target,
        IObservable<object?> commandParameter,
        Action<EventHandler<TEventArgs>> addHandler,
        Action<EventHandler<TEventArgs>> removeHandler)
        where T : class
        where TEventArgs : EventArgs
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(addHandler);
        ArgumentNullException.ThrowIfNull(removeHandler);

        if (command is null)
        {
            return Disposable.Empty;
        }

        commandParameter ??= Observable.Return((object?)target);

        object? latestParam = null;

        void Handler(object? sender, TEventArgs e)
        {
            var param = Volatile.Read(ref latestParam);
            if (command.CanExecute(param))
            {
                command.Execute(param);
            }
        }

        var paramSub = commandParameter.Subscribe(x => Volatile.Write(ref latestParam, x));
        addHandler(Handler);

        return new CompositeDisposable(
            paramSub,
            Disposable.Create(() => removeHandler(Handler)));
    }

    /// <summary>
    /// Creates and caches property setters required for Target/Action binding on the specified runtime type.
    /// </summary>
    /// <param name="type">The runtime type to inspect.</param>
    /// <returns>The cached setter bundle.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the runtime type does not expose a required <c>Target</c> property.</exception>
    [RequiresUnreferencedCode("Cocoa Target/Action binding reflects over runtime types to locate properties that may be removed by trimming.")]
    private static Setters BuildSetters(Type type)
    {
        var actionProp = type.GetRuntimeProperty("Action");
        var targetProp = type.GetRuntimeProperty("Target");
        var enabledProp = type.GetRuntimeProperty("Enabled");

        if (targetProp is null)
        {
            throw new InvalidOperationException(
                $"Target property is required for {nameof(TargetActionCommandBinder)} on type {type.FullName}.");
        }

        return new Setters(
            ActionSetter: actionProp is not null ? Reflection.GetValueSetterOrThrow(actionProp) : null,
            TargetSetter: Reflection.GetValueSetterOrThrow(targetProp),
            EnabledSetter: enabledProp is not null ? Reflection.GetValueSetterForProperty(enabledProp) : null);
    }

    /// <summary>
    /// Represents the set of cached setters required to wire Target/Action and optionally Enabled.
    /// </summary>
    private readonly record struct Setters(
        Action<object?, object?, object?[]?>? ActionSetter,
        Action<object?, object?, object?[]?> TargetSetter,
        Action<object?, object?, object?[]?>? EnabledSetter);

    /// <summary>
    /// Delegate object installed as the Cocoa Target for the <c>theAction:</c> selector.
    /// </summary>
    /// <remarks>
    /// This object must remain alive for the binding lifetime; it is referenced by the bound control's <c>Target</c>.
    /// </remarks>
    private sealed class ControlDelegate : NSObject
    {
        private Action<NSObject> _block;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlDelegate"/> class.
        /// </summary>
        /// <param name="block">The action invoked when the control fires the bound selector.</param>
        public ControlDelegate(Action<NSObject> block) => _block = block;

        /// <summary>
        /// Gets or sets a value indicating whether the command is currently executable.
        /// </summary>
        /// <remarks>
        /// On AppKit, this is used by <c>validateMenuItem:</c> to control menu item enabled state.
        /// </remarks>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Replaces the action invoked by <see cref="TheAction(NSObject)"/>.
        /// </summary>
        /// <param name="block">The new block to invoke.</param>
        public void SetBlock(Action<NSObject> block) => _block = block;

        /// <summary>
        /// Selector invoked by Cocoa controls for Target/Action.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        [Export("theAction:")]
        public void TheAction(NSObject sender) => _block(sender);

#if !UIKIT
        /// <summary>
        /// AppKit menu item validation hook used to enable/disable menu items.
        /// </summary>
        /// <param name="menuItem">The menu item being validated.</param>
        /// <returns><see langword="true"/> if the item should be enabled; otherwise <see langword="false"/>.</returns>
        [Export("validateMenuItem:")]
        public bool ValidateMenuItem(NSMenuItem menuItem) => IsEnabled;
#endif
    }
}
