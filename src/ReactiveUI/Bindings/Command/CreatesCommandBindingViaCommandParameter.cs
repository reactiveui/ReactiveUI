// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// Creates command bindings for objects that expose <c>Command</c> and <c>CommandParameter</c>
    /// as public instance properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This binder targets command-source style controls (for example, WPF-style controls) where command execution
    /// is driven by setting properties rather than subscribing to an event.
    /// </para>
    /// <para>
    /// Trimming/AOT note: This type uses name-based reflection to locate public properties. Consumers running under
    /// trimming must ensure the relevant public properties are preserved on the target control types. This
    /// requirement is expressed via <see cref="DynamicallyAccessedMembersAttribute"/> on the public generic entry points.
    /// </para>
    /// <para>
    /// Performance note: This implementation uses a per-closed-generic static cache (“holder”) rather than a global MRU.
    /// Steady-state access is lock-free and reduces lookup overhead to static field reads.
    /// </para>
    /// </remarks>
    public sealed class CreatesCommandBindingViaCommandParameter : ICreatesCommandBinding
    {
        /// <summary>
        /// The expected name of the command property.
        /// </summary>
        private const string CommandPropertyName = "Command";

        /// <summary>
        /// The expected name of the command parameter property.
        /// </summary>
        private const string CommandParameterPropertyName = "CommandParameter";

        /// <inheritdoc />
        /// <remarks>
        /// If an explicit event target exists, this binder is not applicable and returns 0.
        /// Otherwise, it returns 5 if the target type exposes the required public instance properties; otherwise it returns 0.
        /// </remarks>
        public int GetAffinityForObject<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.PublicProperties)] T>(bool hasEventTarget)
        {
            if (hasEventTarget)
            {
                return 0;
            }

            return Holder<T>.HasRequiredProperties ? 5 : 0;
        }

        /// <inheritdoc />
        /// <remarks>
        /// <para>
        /// This implementation is intentionally “best effort.” If required properties cannot be resolved for
        /// <typeparamref name="T"/>, it returns <see cref="Disposable.Empty"/> to preserve legacy behavior where binder
        /// selection is expected to be affinity-driven rather than exception-driven.
        /// </para>
        /// <para>
        /// Disposal ordering minimizes observable races: the parameter subscription is disposed before restoring
        /// the original parameter value.
        /// </para>
        /// <para>
        /// The command property is set after establishing the parameter subscription, preserving historical ordering
        /// semantics (“set Command last”).
        /// </para>
        /// </remarks>
        [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
        public IDisposable? BindCommandToObject<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T>(
            ICommand? command,
            T? target,
            IObservable<object?> commandParameter)
            where T : class
        {
            ArgumentExceptionHelper.ThrowIfNull(target);
            ArgumentExceptionHelper.ThrowIfNull(commandParameter);

            var commandProperty = Holder<T>.CommandProperty;
            var commandParameterProperty = Holder<T>.CommandParameterProperty;

            // Preserve historical behavior: silently no-op if properties are missing.
            if (commandProperty is null || commandParameterProperty is null)
            {
                return Disposable.Empty;
            }

            // Snapshot original values once. Restoration occurs on disposal.
            var originalCommand = commandProperty.GetValue(target);
            var originalParameter = commandParameterProperty.GetValue(target);

            // Subscribe first so dispose can stop updates before restoration.
            // This delegate intentionally remains simple and allocation-minimal (no LINQ).
            var subscription = commandParameter.Subscribe(
                value => commandParameterProperty.SetValue(target, value));

            // Set command last to preserve ordering semantics.
            commandProperty.SetValue(target, command);

            // Use Rx disposable to unbind and restore original values. AnonymousDisposable is idempotent and thread-safe.
            return Disposable.Create(() =>
            {
                // Stop parameter updates first.
                subscription.Dispose();

                // Restore original values in a predictable order.
                commandParameterProperty.SetValue(target, originalParameter);
                commandProperty.SetValue(target, originalCommand);
            });
        }

        /// <inheritdoc />
        /// <remarks>
        /// This binder is for command-property based binding. If an event name is specified, event-based binders
        /// should be used. This method therefore returns <see cref="Disposable.Empty"/>.
        /// </remarks>
        [RequiresUnreferencedCode("String/reflection-based event binding may require members removed by trimming.")]
        public IDisposable? BindCommandToObject<T, TEventArgs>(
            ICommand? command,
            T? target,
            IObservable<object?> commandParameter,
            string eventName)
            where T : class => Disposable.Empty;

        /// <inheritdoc />
        /// <remarks>
        /// This binder is for command-property based binding. If an event name is specified, event-based binders
        /// should be used. This method therefore returns <see cref="Disposable.Empty"/>.
        /// </remarks>
        public IDisposable? BindCommandToObject<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)] T, TEventArgs>(ICommand? command, T? target, IObservable<object?> commandParameter, Action<EventHandler<TEventArgs>> addHandler, Action<EventHandler<TEventArgs>> removeHandler)
            where T : class
            where TEventArgs : EventArgs => Disposable.Empty;

        /// <summary>
        /// Per-closed-generic cache of resolved command properties for a target type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The target type. Public properties must be preserved in trimmed applications.
        /// </typeparam>
        /// <remarks>
        /// <para>
        /// This pattern avoids a global type-keyed cache that performs reflection in a cache factory delegate,
        /// which is a frequent source of trimming warnings and hard-to-annotate member requirements.
        /// </para>
        /// <para>
        /// Static initialization is thread-safe by the CLR. After initialization, access is lock-free.
        /// </para>
        /// </remarks>
        private static class Holder<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
        {
            /// <summary>
            /// Gets a value indicating whether the target type exposes both required properties.
            /// </summary>
            internal static readonly bool HasRequiredProperties;

            /// <summary>
            /// Gets the resolved public instance property named <c>Command</c>, or <see langword="null"/> if missing.
            /// </summary>
            internal static readonly PropertyInfo? CommandProperty;

            /// <summary>
            /// Gets the resolved public instance property named <c>CommandParameter</c>, or <see langword="null"/> if missing.
            /// </summary>
            internal static readonly PropertyInfo? CommandParameterProperty;

            /// <summary>
            /// Initializes static members of the <see cref="Holder{T}"/> class.
            /// </summary>
            static Holder()
            {
                ResolveProperties(typeof(T), out CommandProperty, out CommandParameterProperty);
                HasRequiredProperties = CommandProperty is not null && CommandParameterProperty is not null;
            }

            /// <summary>
            /// Resolves required properties via a single pass over public instance properties.
            /// </summary>
            /// <param name="type">The target type to inspect.</param>
            /// <param name="command">Receives the resolved <c>Command</c> property, if present.</param>
            /// <param name="commandParameter">Receives the resolved <c>CommandParameter</c> property, if present.</param>
            /// <remarks>
            /// The method avoids LINQ and repeated reflection calls to reduce overhead.
            /// </remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void ResolveProperties(
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type,
                out PropertyInfo? command,
                out PropertyInfo? commandParameter)
            {
                command = null;
                commandParameter = null;

                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                for (var i = 0; i < properties.Length; i++)
                {
                    var p = properties[i];
                    var name = p.Name;

                    if (command is null &&
                        string.Equals(name, CommandPropertyName, StringComparison.Ordinal))
                    {
                        command = p;
                        continue;
                    }

                    if (commandParameter is null &&
                        string.Equals(name, CommandParameterPropertyName, StringComparison.Ordinal))
                    {
                        commandParameter = p;
                    }

                    if (command is not null && commandParameter is not null)
                    {
                        return;
                    }
                }
            }
        }
    }
}
