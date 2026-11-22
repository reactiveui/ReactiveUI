// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods associated with the <see cref="ISuspensionHost{TAppState}"/> interface.</summary>
/// <remarks>
/// <para>
/// These helpers provide strongly-typed access to the current application state and wire up the
/// <see cref="ISuspensionDriver"/> responsible for persisting it. They are typically invoked from platform bootstrap
/// classes after registering an <c>AutoSuspendHelper</c>.
/// </para>
/// </remarks>
public static class SuspensionHostExtensions
{
    /// <summary>A shared completed RxVoid signal. Typed as the interface so the synchronous helpers below
    /// surface it without CA1859 demanding the concrete singleton type leak into their signatures.</summary>
    private static readonly IObservable<RxVoid> _completed = ImmutableReturnRxVoidSignal.Instance;

    /// <summary>
    /// The pending one-time app-state load of each host, keyed by host so that every host loads through the driver it
    /// was set up with. The table holds its keys weakly, so it does not keep a host alive.
    /// </summary>
    private static readonly ConditionalWeakTable<ISuspensionHost, SuspensionHostHelpers.PendingLoad> _pendingLoads = new();

    /// <summary>Provides the app-state load hook for the untyped <see cref="ISuspensionHost"/>.</summary>
    /// <param name="item">The suspension host.</param>
    extension(ISuspensionHost item)
    {
        /// <summary>Runs the one-time app-state load that a typed <c>SetupDefaultSuspendResume</c> call registered for the host, if it has not run yet.</summary>
        /// <remarks>
        /// The typed <c>GetAppState</c> does this itself. This member exists so that <c>ReactiveUI.Unsafe</c> can read the
        /// state of a host through the untyped <see cref="ISuspensionHost"/> after a typed setup.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EnsureAppStateLoaded()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            RunPendingLoad(item);
        }
    }

    /// <summary>Provides strongly-typed app-state access and suspend/resume setup extension members for <see cref="ISuspensionHost{TAppState}"/>.</summary>
    /// <typeparam name="TAppState">The application state type.</typeparam>
    /// <param name="item">The typed suspension host.</param>
    extension<TAppState>(ISuspensionHost<TAppState> item)
        where TAppState : class
    {
        /// <summary>Gets the current strongly-typed application state.</summary>
        /// <returns>The app state.</returns>
        /// <remarks>
        /// Calling this method triggers a one-time load if the state has not yet been materialized.
        /// For trimming/AOT-safe persistence, use <see cref="SetupDefaultSuspendResume{TAppState}(ISuspensionHost{TAppState}, JsonTypeInfo{TAppState}, ISuspensionDriver?)"/>.
        /// </remarks>
        public TAppState GetAppState()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            RunPendingLoad(item);

            return item.AppStateValue!;
        }

        /// <summary>Observes changes to the typed application state without using WhenAny APIs (trimming/AOT friendly).</summary>
        /// <returns>An observable of the typed application state.</returns>
        /// <remarks>
        /// Emits the current value immediately (if available) and every subsequent assignment to <see cref="ISuspensionHost{TAppState}.AppStateValue"/>.
        /// </remarks>
        public IObservable<TAppState> ObserveAppState()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return new AppStateValueObservable<TAppState>(item);
        }

        /// <summary>Sets up suspend/resume using a strongly-typed host and source-generated JSON metadata, using a resolved driver.</summary>
        /// <param name="typeInfo">Source-generated metadata for TAppState.</param>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable SetupDefaultSuspendResume(
            JsonTypeInfo<TAppState> typeInfo) =>
            item.SetupDefaultSuspendResume(typeInfo, null);

        /// <summary>Sets up suspend/resume using a strongly-typed host and source-generated JSON metadata (trimming/AOT friendly).</summary>
        /// <param name="typeInfo">Source-generated metadata for TAppState.</param>
        /// <param name="driver">The suspension driver.</param>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        /// <remarks>
        /// This overload persists and restores state using ISuspensionDriver.LoadState and ISuspensionDriver.SaveState
        /// to avoid reflection-based serialization.
        /// </remarks>
        public IDisposable SetupDefaultSuspendResume(
            JsonTypeInfo<TAppState> typeInfo,
            ISuspensionDriver? driver)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);
            ArgumentExceptionHelper.ThrowIfNull(typeInfo);

            var resolvedDriver = driver ?? AppLocator.Current.GetService<ISuspensionDriver>();

            if (resolvedDriver is null)
            {
                item.Log().Error("Could not find a valid driver and therefore cannot setup Suspend/Resume.");
                return EmptyDisposable.Instance;
            }

            var pendingLoad = _pendingLoads.GetValue(item, static _ => new SuspensionHostHelpers.PendingLoad());
            pendingLoad.Arm(() => EnsureLoadAppState(item, resolvedDriver, typeInfo));

            return SuspensionHostHelpers.SubscribeLifecycle(
                item,
                pendingLoad,
                resolvedDriver.InvalidateState,
                () => resolvedDriver.SaveState(item.AppStateValue!, typeInfo));
        }
    }

    /// <summary>Ensures a one-time typed app state load from storage using source-generated JSON metadata (trimming/AOT friendly).</summary>
    /// <typeparam name="TAppState">The application state type.</typeparam>
    /// <param name="item">The typed suspension host.</param>
    /// <param name="driver">The suspension driver.</param>
    /// <param name="typeInfo">Source-generated metadata for <typeparamref name="TAppState"/>.</param>
    /// <returns>A completed observable.</returns>
    private static IObservable<RxVoid> EnsureLoadAppState<TAppState>(
        ISuspensionHost<TAppState> item,
        ISuspensionDriver driver,
        JsonTypeInfo<TAppState> typeInfo)
        where TAppState : class
    {
        if (item.AppStateValue is not null)
        {
            return _completed;
        }

        try
        {
            // Fall back to a freshly created state when the driver yields no persisted state (see #4349).
            item.AppStateValue = SuspensionHostHelpers.WaitForResult(driver.LoadState(typeInfo)) ?? item.CreateNewAppStateTyped?.Invoke();
        }
        catch (Exception ex)
        {
            item.Log().Warn(ex, "Failed to restore app state from storage, creating from scratch");
            item.AppStateValue = item.CreateNewAppStateTyped?.Invoke();
        }

        return _completed;
    }

    /// <summary>Runs the host's pending one-time app-state load exactly once, if one is registered.</summary>
    /// <param name="item">The suspension host.</param>
    private static void RunPendingLoad(ISuspensionHost item)
    {
        if (_pendingLoads.TryGetValue(item, out var pendingLoad))
        {
            pendingLoad.Run();
        }
    }

    /// <summary>Emits the current typed app state (when present) followed by every non-null assignment. Specialised to <see cref="ObserveAppState{TAppState}(ISuspensionHost{TAppState})"/>.</summary>
    /// <typeparam name="TAppState">The application state type.</typeparam>
    /// <param name="item">The typed suspension host.</param>
    private sealed class AppStateValueObservable<TAppState>(ISuspensionHost<TAppState> item) : IObservable<TAppState>
        where TAppState : class
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TAppState> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            var current = item.AppStateValue;
            if (current is not null)
            {
                observer.OnNext(current);
            }

            return item.AppStateValueChanged.Subscribe(new Sink(observer));
        }

        /// <summary>Forwards each non-null app-state assignment.</summary>
        /// <param name="downstream">The observer receiving the app state.</param>
        private sealed class Sink(IObserver<TAppState> downstream) : IObserver<TAppState?>
        {
            /// <inheritdoc/>
            public void OnNext(TAppState? value)
            {
                if (value is null)
                {
                    return;
                }

                downstream.OnNext(value);
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
