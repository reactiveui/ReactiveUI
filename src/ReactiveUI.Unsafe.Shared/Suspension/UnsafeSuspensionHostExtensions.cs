// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Reflection-based suspension host extension methods for untyped <see cref="ISuspensionHost"/>.</summary>
public static class UnsafeSuspensionHostExtensions
{
    /// <summary>A shared completed RxVoid signal.</summary>
    private static readonly IObservable<RxVoid> _completed = ImmutableReturnRxVoidSignal.Instance;

    /// <summary>The pending one-time app-state load of each host, keyed by host.</summary>
    private static readonly ConditionalWeakTable<ISuspensionHost, SuspensionHostHelpers.PendingLoad> _pendingLoads = new();

    /// <summary>Provides reflection-based app-state access and suspend/resume setup extension members for <see cref="ISuspensionHost"/>.</summary>
    /// <param name="item">The suspension host.</param>
    extension(ISuspensionHost item)
    {
        /// <summary>Get the current App State of a class derived from ISuspensionHost.</summary>
        /// <typeparam name="T">The app state type.</typeparam>
        /// <returns>The app state.</returns>
        [RequiresUnreferencedCode(
            "This overload may invoke IUnsafeSuspensionDriver.LoadState(), which is commonly reflection-based. "
            + "Prefer GetAppState<TAppState>(ISuspensionHost<TAppState>) used with SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload may invoke IUnsafeSuspensionDriver.LoadState(), which is commonly reflection-based. "
            + "Prefer GetAppState<TAppState>(ISuspensionHost<TAppState>) used with SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [SuppressMessage(
            "Design",
            "SST2307:Generic method type parameters should be inferable from the parameters",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public T GetAppState<T>()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            if (_pendingLoads.TryGetValue(item, out var pendingLoad))
            {
                pendingLoad.Run();
            }

            return (T)item.AppState!;
        }

        /// <summary>Observe changes to the AppState of a class derived from ISuspensionHost.</summary>
        /// <typeparam name="T">The observable type.</typeparam>
        /// <returns>An observable of the app state.</returns>
        [RequiresUnreferencedCode(
            "This overload uses WhenAny, which can require unreferenced/dynamic code in trimming/AOT scenarios. "
            + "Prefer ObserveAppState<TAppState>(ISuspensionHost<TAppState>) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload uses WhenAny, which can require unreferenced/dynamic code in trimming/AOT scenarios. "
            + "Prefer ObserveAppState<TAppState>(ISuspensionHost<TAppState>) for trimming/AOT scenarios.")]
        [SuppressMessage(
            "Design",
            "SST2307:Generic method type parameters should be inferable from the parameters",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<T> ObserveAppState<T>()
            where T : class
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return new NonNullCastObservable<T>(item.WhenAnyUnsafe(
                x => x.AppState,
                static observedChange => observedChange.Value));
        }

        /// <summary>Setup our suspension driver for a class derived off ISuspensionHost interface using a resolved driver.</summary>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        [RequiresUnreferencedCode(
            "This overload may invoke IUnsafeSuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. "
            + "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload may invoke IUnsafeSuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. "
            + "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable SetupDefaultSuspendResume() =>
            item.SetupDefaultSuspendResume(null);

        /// <summary>
        /// Setup our suspension driver for a class derived off ISuspensionHost interface.
        /// This will make your suspension host respond to suspend and resume requests.
        /// </summary>
        /// <param name="driver">The unsafe suspension driver.</param>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        [RequiresUnreferencedCode(
            "This overload may invoke IUnsafeSuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. "
            + "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload may invoke IUnsafeSuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. "
            + "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        public IDisposable SetupDefaultSuspendResume(IUnsafeSuspensionDriver? driver)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            var resolvedDriver = driver
                ?? AppLocator.Current.GetService<IUnsafeSuspensionDriver>()
                ?? AppLocator.Current.GetService<ISuspensionDriver>() as IUnsafeSuspensionDriver;

            if (resolvedDriver is null)
            {
                item.Log().Error("Could not find a valid driver and therefore cannot setup Suspend/Resume.");
                return EmptyDisposable.Instance;
            }

            var pendingLoad = _pendingLoads.GetValue(item, static _ => new SuspensionHostHelpers.PendingLoad());
            pendingLoad.Arm(() => EnsureLoadAppState(item, resolvedDriver));

            return SuspensionHostHelpers.SubscribeLifecycle(
                item,
                pendingLoad,
                resolvedDriver.InvalidateState,
                () => resolvedDriver.SaveState(item.AppState!));
        }
    }

    /// <summary>Ensures one time app state load from storage.</summary>
    /// <param name="item">The suspension host.</param>
    /// <param name="driver">The unsafe suspension driver.</param>
    /// <returns>A completed observable.</returns>
    [RequiresUnreferencedCode(
        "This overload may invoke IUnsafeSuspensionDriver.LoadState(), which is commonly reflection-based. "
        + "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
    [RequiresDynamicCode(
        "This overload may invoke IUnsafeSuspensionDriver.LoadState(), which is commonly reflection-based. "
        + "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
    private static IObservable<RxVoid> EnsureLoadAppState(ISuspensionHost item, IUnsafeSuspensionDriver driver)
    {
        if (item.AppState is not null)
        {
            return _completed;
        }

        try
        {
            item.AppState = SuspensionHostHelpers.WaitForResult(driver.LoadState()) ?? item.CreateNewAppState?.Invoke();
        }
        catch (Exception ex)
        {
            item.Log().Warn(ex, "Failed to restore app state from storage, creating from scratch");
            item.AppState = item.CreateNewAppState?.Invoke();
        }

        return _completed;
    }

    /// <summary>Emits only the non-null values of a <c>WhenAny</c> stream, cast to the requested type.</summary>
    /// <typeparam name="TResult">The requested app-state type.</typeparam>
    /// <param name="source">The source app-state value stream.</param>
    private sealed class NonNullCastObservable<TResult>(IObservable<object?> source) : IObservable<TResult>
        where TResult : class
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Forwards each non-null value cast to the requested type.</summary>
        /// <param name="downstream">The observer receiving the cast values.</param>
        private sealed class Sink(IObserver<TResult> downstream) : IObserver<object?>
        {
            /// <inheritdoc/>
            public void OnNext(object? value)
            {
                if (value is not TResult typed)
                {
                    return;
                }

                downstream.OnNext(typed);
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted() => downstream.OnCompleted();
        }
    }
}
