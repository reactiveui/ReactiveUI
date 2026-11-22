// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif

/// <summary>Shared suspension-host helpers used by both the AOT-safe and reflection suspension extensions.</summary>
internal static class SuspensionHostHelpers
{
    /// <summary>Subscribes to the host's invalidate, persist, resume, and launch lifecycle signals.</summary>
    /// <param name="item">The suspension host.</param>
    /// <param name="pendingLoad">The one-time load gate for <paramref name="item"/>.</param>
    /// <param name="invalidate">Invoked when the host signals state invalidation.</param>
    /// <param name="save">Invoked when the host signals state persistence (after <paramref name="pendingLoad"/> runs).</param>
    /// <returns>A composite disposable owning all lifecycle subscriptions.</returns>
    internal static IDisposable SubscribeLifecycle(
        ISuspensionHost item,
        PendingLoad pendingLoad,
        Func<IObservable<RxVoid>> invalidate,
        Func<IObservable<RxVoid>> save)
    {
        MultipleDisposable ret = [];
        ret.Add(item.ShouldInvalidateState.Subscribe(new DriverOperationObserver<RxVoid>(
            item,
            _ => invalidate(),
            static _ => { },
            "Invalidated app state",
            "Tried to invalidate app state")));

        ret.Add(item.ShouldPersistState.Subscribe(new DriverOperationObserver<IDisposable>(
            item,
            _ =>
            {
                pendingLoad.Run();
                return save();
            },
            static token => token.Dispose(),
            "Persisted application state",
            "Tried to persist app state")));

        ret.Add(item.IsResuming.Subscribe(new DelegateObserver<RxVoid>(_ => pendingLoad.Run())));
        ret.Add(item.IsLaunchingNew.Subscribe(new DelegateObserver<RxVoid>(_ => pendingLoad.Run())));

        return ret;
    }

    /// <summary>Subscribes to a single-value observable and blocks until it terminates, returning its last value.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <returns>The last value produced by the source, or default if none was produced.</returns>
    internal static T? WaitForResult<T>(IObservable<T> source)
    {
        using var signal = new ManualResetEventSlim(false);
        var observer = new BlockingObserver<T>(signal);
        using (source.Subscribe(observer))
        {
            signal.Wait();
        }

        return observer.GetResult();
    }

    /// <summary>For each lifecycle signal, runs a suspension-driver operation, logging success and swallowing/logging failures.</summary>
    /// <typeparam name="TArg">The lifecycle signal type.</typeparam>
    /// <param name="logHost">The object used for logging.</param>
    /// <param name="operation">Produces the driver operation for a signal.</param>
    /// <param name="onFinally">Runs after the operation terminates (for example, disposing the persist token).</param>
    /// <param name="successMessage">Logged when the operation succeeds.</param>
    /// <param name="errorMessage">Logged when the operation (or its creation) fails.</param>
    internal sealed class DriverOperationObserver<TArg>(
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

    /// <summary>Holds one host's pending one-time app-state load, bound to the driver that host was set up with.</summary>
    internal sealed class PendingLoad
    {
        /// <summary>The load to run, or <see langword="null"/> once it has run or before any setup.</summary>
        private Func<IObservable<RxVoid>>? _load;

        /// <summary>Registers the load to run, replacing any load that has not run yet.</summary>
        /// <param name="load">The load to run.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Arm(Func<IObservable<RxVoid>> load) => Volatile.Write(ref _load, load);

        /// <summary>Runs the registered load exactly once; later calls do nothing until the next <see cref="Arm"/>.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Run() => Interlocked.Exchange(ref _load, null)?.Invoke();
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
        internal T? GetResult() => _error is not null ? throw _error : _value;
    }
}
