// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Primitives.Disposables;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods associated with the ISuspensionHost interface.</summary>
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

    /// <summary>Func used to load app state exactly once. Backing field for testing purposes.</summary>
    private static Func<IObservable<RxVoid>>? _ensureLoadAppStateFunc;

    /// <summary>Suspension driver reference field. Backing field for testing purposes.</summary>
    private static ISuspensionDriver? _suspensionDriver;

    /// <summary>Gets or sets the ensure load app state function. Internal for testing purposes only.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "S2292:Trivial properties should be auto-implemented",
        Justification = "Backing field required for Interlocked.Exchange(ref).")]
    [SuppressMessage(
        "Roslynator",
        "RCS1085:Use auto-implemented property",
        Justification = "Need explicit backing field for Interlocked.Exchange")]
    internal static Func<IObservable<RxVoid>>? EnsureLoadAppStateFunc
    {
        get => _ensureLoadAppStateFunc;
        set => _ensureLoadAppStateFunc = value;
    }

    /// <summary>Gets or sets the suspension driver. Internal for testing purposes only.</summary>
    [SuppressMessage(
        "Minor Code Smell",
        "S2292:Trivial properties should be auto-implemented",
        Justification = "Backing field required for Interlocked.Exchange(ref).")]
    [SuppressMessage(
        "Roslynator",
        "RCS1085:Use auto-implemented property",
        Justification = "Need explicit backing field for Interlocked.Exchange")]
    internal static ISuspensionDriver? SuspensionDriver
    {
        get => _suspensionDriver;
        set => _suspensionDriver = value;
    }

    /// <summary>Provides app-state access and suspend/resume setup extension members for <see cref="ISuspensionHost"/>.</summary>
    /// <param name="item">The suspension host.</param>
    extension(ISuspensionHost item)
    {
        /// <summary>Get the current App State of a class derived from ISuspensionHost.</summary>
        /// <typeparam name="T">The app state type.</typeparam>
        /// <returns>The app state.</returns>
        /// <remarks>
        /// Calling this method triggers a one-time load via <see cref="ISuspensionDriver.LoadState"/> if the state has not
        /// yet been materialized, ensuring late subscribers still receive persisted data.
        /// </remarks>
        [RequiresUnreferencedCode(
            "This overload may invoke ISuspensionDriver.LoadState(), which is commonly reflection-based. " +
            "Prefer GetAppState<TAppState>(ISuspensionHost<TAppState>) used with SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload may invoke ISuspensionDriver.LoadState(), which is commonly reflection-based. " +
            "Prefer GetAppState<TAppState>(ISuspensionHost<TAppState>) used with SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public T GetAppState<T>()
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            Interlocked.Exchange(ref _ensureLoadAppStateFunc, null)?.Invoke();

            return (T)item.AppState!;
        }

        /// <summary>Observe changes to the AppState of a class derived from ISuspensionHost.</summary>
        /// <typeparam name="T">The observable type.</typeparam>
        /// <returns>An observable of the app state.</returns>
        /// <remarks>
        /// Emits the current value immediately (if available) and every subsequent assignment so downstream components can
        /// react to hot reloads or state restoration.
        /// </remarks>
        [RequiresUnreferencedCode(
            "This overload uses WhenAny, which can require unreferenced/dynamic code in trimming/AOT scenarios. " +
            "Prefer ObserveAppState<TAppState>(ISuspensionHost<TAppState>) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload uses WhenAny, which can require unreferenced/dynamic code in trimming/AOT scenarios. " +
            "Prefer ObserveAppState<TAppState>(ISuspensionHost<TAppState>) for trimming/AOT scenarios.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<T> ObserveAppState<T>()
            where T : class
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            return new NonNullCastObservable<T>(item.WhenAny<ISuspensionHost, object?, object?>(
                nameof(item.AppState),
                static observedChange => observedChange.Value));
        }

        /// <summary>Setup our suspension driver for a class derived off ISuspensionHost interface using a resolved driver.</summary>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        [RequiresUnreferencedCode(
            "This overload may invoke ISuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. " +
            "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload may invoke ISuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. " +
            "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        public IDisposable SetupDefaultSuspendResume() =>
            item.SetupDefaultSuspendResume(null);

        /// <summary>
        /// Setup our suspension driver for a class derived off ISuspensionHost interface.
        /// This will make your suspension host respond to suspend and resume requests.
        /// </summary>
        /// <param name="driver">The suspension driver.</param>
        /// <returns>A disposable which will stop responding to Suspend and Resume requests.</returns>
        /// <remarks>
        /// <para>
        /// Registers handlers for ISuspensionHost.ShouldPersistState, ISuspensionHost.ShouldInvalidateState,
        /// and resume notifications, delegating serialization to the provided driver (or a resolved
        /// instance from AppLocator).
        /// </para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// <![CDATA[
        /// RxSuspension.SuspensionHost.CreateNewAppState = () => new ShellState();
        /// RxSuspension.SuspensionHost.SetupDefaultSuspendResume(new FileSuspensionDriver(FileSystem.AppDataDirectory));
        /// ]]>
        /// </code>
        /// </example>
        [RequiresUnreferencedCode(
            "This overload may invoke ISuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. " +
            "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        [RequiresDynamicCode(
            "This overload may invoke ISuspensionDriver.LoadState()/SaveState<T>(T), which are commonly reflection-based. " +
            "Prefer SetupDefaultSuspendResume<TAppState>(..., JsonTypeInfo<TAppState>, ...) for trimming/AOT scenarios.")]
        public IDisposable SetupDefaultSuspendResume(ISuspensionDriver? driver)
        {
            ArgumentExceptionHelper.ThrowIfNull(item);

            MultipleDisposable ret = [];
            _suspensionDriver ??= driver ?? AppLocator.Current.GetService<ISuspensionDriver>();

            if (_suspensionDriver is null)
            {
                item.Log().Error("Could not find a valid driver and therefore cannot setup Suspend/Resume.");
                return EmptyDisposable.Instance;
            }

            _ensureLoadAppStateFunc = () => EnsureLoadAppState(item, _suspensionDriver);

            var resolvedDriver = _suspensionDriver!;
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
                    // Materialize app state (one-time load) before saving, so a shutdown that races ahead of
                    // the resume/launch load still persists real state rather than null (see #4353).
                    RunPendingLoad();
                    return resolvedDriver.SaveState(item.AppState!);
                },
                static token => token.Dispose(),
                "Persisted application state",
                "Tried to persist app state")));

            ret.Add(item.IsResuming.Subscribe(new DelegateObserver<RxVoid>(static _ => RunPendingLoad())));
            ret.Add(item.IsLaunchingNew.Subscribe(new DelegateObserver<RxVoid>(static _ => RunPendingLoad())));

            return ret;
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

            Interlocked.Exchange(ref _ensureLoadAppStateFunc, null)?.Invoke();

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

            MultipleDisposable ret = [];
            _suspensionDriver ??= driver ?? AppLocator.Current.GetService<ISuspensionDriver>();

            if (_suspensionDriver is null)
            {
                item.Log().Error("Could not find a valid driver and therefore cannot setup Suspend/Resume.");
                return EmptyDisposable.Instance;
            }

            _ensureLoadAppStateFunc = () => EnsureLoadAppState(item, _suspensionDriver, typeInfo);

            var resolvedDriver = _suspensionDriver!;
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
                    // Materialize app state (one-time load) before saving, so a shutdown that races ahead of
                    // the resume/launch load still persists real state rather than null (see #4353).
                    RunPendingLoad();
                    return resolvedDriver.SaveState(item.AppStateValue!, typeInfo);
                },
                static token => token.Dispose(),
                "Persisted application state",
                "Tried to persist app state")));

            ret.Add(item.IsResuming.Subscribe(new DelegateObserver<RxVoid>(static _ => RunPendingLoad())));
            ret.Add(item.IsLaunchingNew.Subscribe(new DelegateObserver<RxVoid>(static _ => RunPendingLoad())));

            return ret;
        }
    }

    /// <summary>Ensures one time app state load from storage.</summary>
    /// <param name="item">The suspension host.</param>
    /// <param name="driver">The suspension driver.</param>
    /// <returns>A completed observable.</returns>
    [RequiresUnreferencedCode(
        "This overload may invoke ISuspensionDriver.LoadState(), which is commonly reflection-based. " +
        "Prefer EnsureLoadAppState<TAppState>(ISuspensionHost<TAppState>, ISuspensionDriver?, JsonTypeInfo<TAppState>) for trimming/AOT scenarios.")]
    [RequiresDynamicCode(
        "This overload may invoke ISuspensionDriver.LoadState(), which is commonly reflection-based. " +
        "Prefer EnsureLoadAppState<TAppState>(ISuspensionHost<TAppState>, ISuspensionDriver?, JsonTypeInfo<TAppState>) for trimming/AOT scenarios.")]
    private static IObservable<RxVoid> EnsureLoadAppState(ISuspensionHost item, ISuspensionDriver? driver = null)
    {
        if (item.AppState is not null)
        {
            return _completed;
        }

        _suspensionDriver ??= driver ?? AppLocator.Current.GetService<ISuspensionDriver>();

        if (_suspensionDriver is null)
        {
            item.Log().Error("Could not find a valid driver and therefore cannot load app state.");
            return _completed;
        }

        try
        {
            // Fall back to a freshly created state when the driver yields no persisted state (see #4349).
            item.AppState = WaitForResult(_suspensionDriver.LoadState()) ?? item.CreateNewAppState?.Invoke();
        }
        catch (Exception ex)
        {
            item.Log().Warn(ex, "Failed to restore app state from storage, creating from scratch");
            item.AppState = item.CreateNewAppState?.Invoke();
        }

        return _completed;
    }

    /// <summary>Ensures a one-time typed app state load from storage using source-generated JSON metadata (trimming/AOT friendly).</summary>
    /// <typeparam name="TAppState">The application state type.</typeparam>
    /// <param name="item">The typed suspension host.</param>
    /// <param name="driver">The suspension driver.</param>
    /// <param name="typeInfo">Source-generated metadata for <typeparamref name="TAppState"/>.</param>
    /// <returns>A completed observable.</returns>
    private static IObservable<RxVoid> EnsureLoadAppState<TAppState>(
        ISuspensionHost<TAppState> item,
        ISuspensionDriver? driver,
        JsonTypeInfo<TAppState> typeInfo)
        where TAppState : class
    {
        if (item.AppStateValue is not null)
        {
            return _completed;
        }

        _suspensionDriver ??= driver ?? AppLocator.Current.GetService<ISuspensionDriver>();

        if (_suspensionDriver is null)
        {
            item.Log().Error("Could not find a valid driver and therefore cannot load app state.");
            return _completed;
        }

        try
        {
            // Fall back to a freshly created state when the driver yields no persisted state (see #4349).
            item.AppStateValue = WaitForResult(_suspensionDriver.LoadState(typeInfo)) ?? item.CreateNewAppStateTyped?.Invoke();
        }
        catch (Exception ex)
        {
            item.Log().Warn(ex, "Failed to restore app state from storage, creating from scratch");
            item.AppStateValue = item.CreateNewAppStateTyped?.Invoke();
        }

        return _completed;
    }

    /// <summary>Runs the pending one-time app-state load exactly once, if one is registered.</summary>
    private static void RunPendingLoad() => Interlocked.Exchange(ref _ensureLoadAppStateFunc, null)?.Invoke();

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

    /// <summary>Emits only the non-null values of a <c>WhenAny</c> stream, cast to the requested type. Specialised to <see cref="ObserveAppState{T}(ISuspensionHost)"/>.</summary>
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
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
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
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>
    /// For each lifecycle signal, runs a suspension-driver operation, logging success and swallowing/logging failures,
    /// and runs a finalizer afterwards. Fuses the prior <c>SelectMany</c> + <c>Finally</c> + <c>LoggedCatch</c> +
    /// <c>Subscribe</c> pipeline into one observer.
    /// </summary>
    /// <typeparam name="TArg">The lifecycle signal type.</typeparam>
    /// <param name="logHost">The object used for logging.</param>
    /// <param name="operation">Produces the driver operation for a signal.</param>
    /// <param name="onFinally">Runs after the operation terminates (for example, disposing the persist token).</param>
    /// <param name="successMessage">Logged when the operation succeeds.</param>
    /// <param name="errorMessage">Logged when the operation (or its creation) fails.</param>
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
            /// <summary>Whether the finalizer has run.</summary>
            private bool _finished;

            /// <inheritdoc/>
            public void OnNext(RxVoid value) => logHost.Log().Info(successMessage);

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                logHost.Log().Warn(error, errorMessage);
                Finish();
            }

            /// <inheritdoc/>
            public void OnCompleted() => Finish();

            /// <summary>Runs the finalizer exactly once.</summary>
            private void Finish()
            {
                if (_finished)
                {
                    return;
                }

                _finished = true;
                onFinally(arg);
            }
        }
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
        public void OnCompleted() => signal.Set();

        /// <summary>Returns the captured value, rethrowing any terminal error.</summary>
        /// <returns>The last value produced by the source.</returns>
        public T? GetResult() => _error is not null ? throw _error : _value;
    }
}
