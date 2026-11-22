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
    private static readonly ConditionalWeakTable<ISuspensionHost, PendingLoad> _pendingLoads = new();

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

            RunPendingLoad(item);

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

            var pendingLoad = _pendingLoads.GetValue(item, static _ => new PendingLoad());
            pendingLoad.Arm(() => EnsureLoadAppState(item, resolvedDriver));

            MultipleDisposable ret = [];
            ret.Add(item.ShouldInvalidateState.Subscribe(new DriverOperationObserver<RxVoid>(
                item,
                _ => resolvedDriver.InvalidateState(),
                static _ => { },
                "Invalidated app state",
                "Tried to invalidate app state")));

            ret.Add(item.ShouldPersistState.Subscribe(new DriverOperationObserver<IDisposable>(
                item,
                _ =>
                {
                    pendingLoad.Run();
                    return resolvedDriver.SaveState(item.AppState!);
                },
                static token => token.Dispose(),
                "Persisted application state",
                "Tried to persist app state")));

            ret.Add(item.IsResuming.Subscribe(new DelegateObserver<RxVoid>(_ => pendingLoad.Run())));
            ret.Add(item.IsLaunchingNew.Subscribe(new DelegateObserver<RxVoid>(_ => pendingLoad.Run())));

            return ret;
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
            item.AppState = WaitForResult(driver.LoadState()) ?? item.CreateNewAppState?.Invoke();
        }
        catch (Exception ex)
        {
            item.Log().Warn(ex, "Failed to restore app state from storage, creating from scratch");
            item.AppState = item.CreateNewAppState?.Invoke();
        }

        return _completed;
    }

    /// <summary>Runs the host's pending one-time app-state load exactly once, if one is registered.</summary>
    /// <param name="item">The suspension host.</param>
    private static void RunPendingLoad(ISuspensionHost item)
    {
        if (!_pendingLoads.TryGetValue(item, out var pendingLoad))
        {
            return;
        }

        pendingLoad.Run();
    }

    /// <summary>Subscribes to a single-value observable and blocks until it terminates, returning its last value.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <returns>The last value produced by the source, or default if none was produced.</returns>
    private static T? WaitForResult<T>(IObservable<T> source)
    {
        using var signal = new ManualResetEventSlim(false);
        var observer = new BlockingObserver<T>(signal);
        using (source.Subscribe(observer))
        {
            signal.Wait();
        }

        return observer.GetResult();
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

    /// <summary>For each lifecycle signal, runs a suspension-driver operation, logging success and swallowing/logging failures.</summary>
    /// <typeparam name="TArg">The lifecycle signal type.</typeparam>
    /// <param name="logHost">The object used for logging.</param>
    /// <param name="operation">Produces the driver operation for a signal.</param>
    /// <param name="onFinally">Runs after the operation terminates.</param>
    /// <param name="successMessage">Logged when the operation succeeds.</param>
    /// <param name="errorMessage">Logged when the operation fails.</param>
    private sealed class DriverOperationObserver<TArg>(
        IEnableLogger logHost,
        Func<TArg, IObservable<RxVoid>> operation,
        Action<TArg> onFinally,
        string successMessage,
        string errorMessage) : IObserver<TArg>
    {
        /// <inheritdoc/>
        public void OnNext(TArg value)
        {
            IObservable<RxVoid> op;
            try
            {
                op = operation(value);
            }
            catch (Exception ex)
            {
                logHost.Log().Warn(ex, errorMessage);
                onFinally(value);
                return;
            }

            _ = op.Subscribe(new ResultObserver(logHost, value, onFinally, successMessage, errorMessage));
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnError(Exception error) => logHost.Log().Warn(error, errorMessage);

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <summary>Logs the result of a single driver operation and runs the finalizer exactly once.</summary>
        /// <param name="logHost">The object used for logging.</param>
        /// <param name="arg">The signal the operation was run for.</param>
        /// <param name="onFinally">Runs after the operation terminates.</param>
        /// <param name="successMessage">Logged when the operation succeeds.</param>
        /// <param name="errorMessage">Logged when the operation fails.</param>
        private sealed class ResultObserver(
            IEnableLogger logHost,
            TArg arg,
            Action<TArg> onFinally,
            string successMessage,
            string errorMessage) : IObserver<RxVoid>
        {
            /// <summary>Whether the finalizer has run; latched to 1 by the first thread to reach it.</summary>
            private int _finished;

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnNext(RxVoid value) => logHost.Log().Info(successMessage);

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                logHost.Log().Warn(error, errorMessage);
                Finish();
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted() => Finish();

            /// <summary>Runs the finalizer exactly once.</summary>
            private void Finish()
            {
                if (Interlocked.Exchange(ref _finished, 1) != 0)
                {
                    return;
                }

                onFinally(arg);
            }
        }
    }

    /// <summary>Holds one host's pending one-time app-state load.</summary>
    private sealed class PendingLoad
    {
        /// <summary>The load to run, or <see langword="null"/> once it has run or before any setup.</summary>
        private Func<IObservable<RxVoid>>? _load;

        /// <summary>Registers the load to run, replacing any load that has not run yet.</summary>
        /// <param name="load">The load to run.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Arm(Func<IObservable<RxVoid>> load) => Volatile.Write(ref _load, load);

        /// <summary>Runs the registered load exactly once; later calls do nothing until the next <see cref="Arm"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run() => Interlocked.Exchange(ref _load, null)?.Invoke();
    }

    /// <summary>Captures the last value (or terminal error) of a blocking subscription and signals on termination.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="signal">Set when the source terminates.</param>
    private sealed class BlockingObserver<T>(ManualResetEventSlim signal) : IObserver<T>
    {
        /// <summary>The last value produced by the source.</summary>
        private T? _value;

        /// <summary>The terminal error, if the source errored.</summary>
        private Exception? _error;

        /// <inheritdoc/>
        public void OnNext(T value) => _value = value;

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            _error = error;
            signal.Set();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted() => signal.Set();

        /// <summary>Returns the captured value, rethrowing any terminal error.</summary>
        /// <returns>The last value produced by the source.</returns>
        public T? GetResult() => _error is not null ? throw _error : _value;
    }
}
