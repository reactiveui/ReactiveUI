// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Builder;
#else
namespace ReactiveUI.Builder;
#endif

/// <summary>Service-resolution extension members for <see cref="IReactiveUIInstance"/>.</summary>
[SuppressMessage(
    "Design",
    "SST2307:Generic method type parameters should be inferable from the parameters",
    Justification = "Resolution methods take the target type as an explicit generic argument by design; it identifies the type to resolve and cannot be inferred from the parameters.")]
public static partial class BuilderMixins
{
    /// <summary>Validates the instance and returns its service resolver, if one is set.</summary>
    /// <param name="reactiveUiInstance">The reactive UI instance to validate.</param>
    /// <returns>The current dependency resolver, or <see langword="null"/> when the instance has no current resolver.</returns>
    private static IReadonlyDependencyResolver? Resolver(IReactiveUIInstance reactiveUiInstance)
    {
        ArgumentExceptionHelper.ThrowIfNull(reactiveUiInstance);
        return reactiveUiInstance.Current;
    }

    /// <summary>Provides service-resolution extension members for <see cref="IReactiveUIInstance"/>.</summary>
    /// <param name="reactiveUiInstance">The reactive UI instance.</param>
    extension(IReactiveUIInstance reactiveUiInstance)
    {
        /// <summary>Resolves a single instance and passes it to the action.</summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T>(Action<T?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(current.GetService<T>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves two instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2>(
            Action<T1?, T2?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(current.GetService<T1>(), current.GetService<T2>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves three instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3>(
            Action<T1?, T2?, T3?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves four instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4>(
            Action<T1?, T2?, T3?, T4?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>(), current.GetService<T4>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves five instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5>(
            Action<T1?, T2?, T3?, T4?, T5?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>(), current.GetService<T4>(), current.GetService<T5>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves six instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(current.GetService<T1>(), current.GetService<T2>(), current.GetService<T3>(), current.GetService<T4>(), current.GetService<T5>(), current.GetService<T6>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves seven instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves eight instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves nine instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves ten instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <typeparam name="T10">The tenth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>(),
                    current.GetService<T10>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves eleven instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <typeparam name="T10">The tenth type to resolve.</typeparam>
        /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>(),
                    current.GetService<T10>(),
                    current.GetService<T11>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves twelve instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <typeparam name="T10">The tenth type to resolve.</typeparam>
        /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
        /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>(),
                    current.GetService<T10>(),
                    current.GetService<T11>(),
                    current.GetService<T12>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves thirteen instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <typeparam name="T10">The tenth type to resolve.</typeparam>
        /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
        /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
        /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>(),
                    current.GetService<T10>(),
                    current.GetService<T11>(),
                    current.GetService<T12>(),
                    current.GetService<T13>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves fourteen instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <typeparam name="T10">The tenth type to resolve.</typeparam>
        /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
        /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
        /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
        /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>(),
                    current.GetService<T10>(),
                    current.GetService<T11>(),
                    current.GetService<T12>(),
                    current.GetService<T13>(),
                    current.GetService<T14>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves fifteen instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <typeparam name="T10">The tenth type to resolve.</typeparam>
        /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
        /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
        /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
        /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
        /// <typeparam name="T15">The fifteenth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>(),
                    current.GetService<T10>(),
                    current.GetService<T11>(),
                    current.GetService<T12>(),
                    current.GetService<T13>(),
                    current.GetService<T14>(),
                    current.GetService<T15>());
            }

            return reactiveUiInstance;
        }

        /// <summary>Resolves sixteen instances and passes them to the action.</summary>
        /// <typeparam name="T1">The first type to resolve.</typeparam>
        /// <typeparam name="T2">The second type to resolve.</typeparam>
        /// <typeparam name="T3">The third type to resolve.</typeparam>
        /// <typeparam name="T4">The fourth type to resolve.</typeparam>
        /// <typeparam name="T5">The fifth type to resolve.</typeparam>
        /// <typeparam name="T6">The sixth type to resolve.</typeparam>
        /// <typeparam name="T7">The seventh type to resolve.</typeparam>
        /// <typeparam name="T8">The eighth type to resolve.</typeparam>
        /// <typeparam name="T9">The ninth type to resolve.</typeparam>
        /// <typeparam name="T10">The tenth type to resolve.</typeparam>
        /// <typeparam name="T11">The eleventh type to resolve.</typeparam>
        /// <typeparam name="T12">The twelfth type to resolve.</typeparam>
        /// <typeparam name="T13">The thirteenth type to resolve.</typeparam>
        /// <typeparam name="T14">The fourteenth type to resolve.</typeparam>
        /// <typeparam name="T15">The fifteenth type to resolve.</typeparam>
        /// <typeparam name="T16">The sixteenth type to resolve.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>
        /// IReactiveUIInstance instance for chaining.
        /// </returns>
        /// <exception cref="ArgumentNullException">reactiveUIInstance.</exception>
        public IReactiveUIInstance
            WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
                Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?> action)
        {
            if (Resolver(reactiveUiInstance) is { } current && action is not null)
            {
                action(
                    current.GetService<T1>(),
                    current.GetService<T2>(),
                    current.GetService<T3>(),
                    current.GetService<T4>(),
                    current.GetService<T5>(),
                    current.GetService<T6>(),
                    current.GetService<T7>(),
                    current.GetService<T8>(),
                    current.GetService<T9>(),
                    current.GetService<T10>(),
                    current.GetService<T11>(),
                    current.GetService<T12>(),
                    current.GetService<T13>(),
                    current.GetService<T14>(),
                    current.GetService<T15>(),
                    current.GetService<T16>());
            }

            return reactiveUiInstance;
        }
    }
}
