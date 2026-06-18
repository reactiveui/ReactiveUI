// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Encapsulates a user interaction behind a reactive interface.</summary>
/// <remarks>
/// This class provides the bulk of the actual implementation for reactive commands. You should not create instances
/// of this class directly, but rather via the static creation methods on the non-generic <see cref="ReactiveCommand"/>
/// class. The execution state (<see cref="CanExecute"/>, <see cref="IsExecuting"/>, results and exceptions) is held in
/// inline fields and fanned out through lightweight <see cref="Broadcaster{T}"/> streams rather than subjects; the
/// canExecute / isExecuting state is recomputed inline (<c>userCanExecute &amp;&amp; inFlight == 0</c>) under a single gate.
/// State transitions driven by execution begin/end, results and exceptions are scheduled onto the output scheduler;
/// values returned directly from <see cref="Execute(TParam)"/> are delivered on the execution thread.
/// </remarks>
/// <typeparam name="TParam">The type of parameter values passed in during command execution.</typeparam>
/// <typeparam name="TResult">The type of the values that are the result of command execution.</typeparam>
[System.Diagnostics.DebuggerDisplay("CanExecute = {_canExecuteValue}, IsExecuting = {_isExecutingValue}, InFlight = {_inFlight}")]
public class ReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, TResult>
{
    /// <summary>Guards every observer-state and command-state field; held only across snapshots and field writes.</summary>
#if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
#else
    private readonly object _gate = new();
#endif

    /// <summary>The scheduler that begin/end transitions, results and exceptions are delivered on.</summary>
    private readonly ISequencer _outputScheduler;

    /// <summary>The execution function producing, per parameter, an observable of the result observable and a cancel callback.</summary>
    private readonly Func<TParam, IObservable<(IObservable<TResult> Result, Action Cancel)>> _execute;

    /// <summary>Subscription to the user-supplied canExecute observable.</summary>
    private readonly IDisposable _canExecuteSubscription;

    /// <summary>Broadcaster for the effective canExecute value.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<bool> _canExecuteBroadcaster;

    /// <summary>Broadcaster for the isExecuting value.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<bool> _isExecutingBroadcaster;

    /// <summary>Broadcaster for thrown exceptions.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<Exception> _exceptionsBroadcaster;

    /// <summary>Broadcaster for result values delivered to command subscribers.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<TResult> _resultsBroadcaster;

    /// <summary>The latest value from the user-supplied canExecute observable (false until it first emits).</summary>
    private bool _userCanExecute;

    /// <summary>The current effective canExecute value (<c>userCanExecute &amp;&amp; inFlight == 0</c>).</summary>
    private bool _canExecuteValue;

    /// <summary>The current isExecuting value (<c>inFlight &gt; 0</c>).</summary>
    private bool _isExecutingValue;

    /// <summary>The number of executions currently in flight.</summary>
    private int _inFlight;

    /// <summary>Latched once the command has been disposed.</summary>
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveCommand{TParam, TResult}" /> class for work that
    /// signals cancellation through a separate callback (as opposed to cancelling by unsubscribing).
    /// </summary>
    /// <param name="execute">The function producing the result observable and a cancellation callback per execution.</param>
    /// <param name="canExecute">An observable governing whether the command can execute.</param>
    /// <param name="outputScheduler">The scheduler on which output is delivered.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
    protected internal ReactiveCommand(
        Func<TParam, IObservable<(IObservable<TResult> Result, Action Cancel)>> execute,
        IObservable<bool>? canExecute,
        ISequencer? outputScheduler)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _outputScheduler = outputScheduler ?? RxSchedulers.MainThreadScheduler;
        CanExecute = new BoolStream(this, isExecuting: false);
        IsExecuting = new BoolStream(this, isExecuting: true);
        ThrownExceptions = new ExceptionStream(this);
        _canExecuteSubscription = (canExecute ?? SingleValueObservable.True)
            .Subscribe(new DelegateObserver<bool>(OnUserCanExecute, OnUserCanExecuteError));
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCommand{TParam, TResult}" /> class.</summary>
    /// <param name="execute">The function producing the result observable per execution.</param>
    /// <param name="canExecute">An observable governing whether the command can execute.</param>
    /// <param name="outputScheduler">The scheduler on which output is delivered.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="execute"/> is <see langword="null"/>.</exception>
    protected internal ReactiveCommand(
        Func<TParam, IObservable<TResult>> execute,
        IObservable<bool>? canExecute,
        ISequencer? outputScheduler)
        : this(
            p => new SingleValueObservable<(IObservable<TResult> Result, Action Cancel)>((execute(p), NoCancel)),
            canExecute,
            outputScheduler)
    {
    }

    /// <inheritdoc/>
    public override IObservable<bool> CanExecute { get; }

    /// <inheritdoc/>
    public override IObservable<bool> IsExecuting { get; }

    /// <inheritdoc/>
    public override IObservable<Exception> ThrownExceptions { get; }

    /// <inheritdoc/>
    public override IObservable<TResult> Execute(TParam parameter) => new ExecuteObservable(this, parameter);

    /// <inheritdoc/>
    public override IObservable<TResult> Execute() => Execute(default!);

    /// <inheritdoc/>
    public override IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        lock (_gate)
        {
            _resultsBroadcaster.Add(observer);
        }

        return new ResultSubscription(this, observer);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        lock (_gate)
        {
            _disposed = true;
        }

        _canExecuteSubscription.Dispose();
    }

    /// <summary>The no-op cancellation callback used when cancellation happens by unsubscribing.</summary>
    private static void NoCancel()
    {
        // Intentionally empty: cancellation is performed by disposing the subscription, so no callback work is needed.
    }

    /// <summary>Records the latest user canExecute value and recomputes the effective state.</summary>
    /// <param name="value">The value emitted by the user canExecute observable.</param>
    private void OnUserCanExecute(bool value)
    {
        bool changed;
        bool effective;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _userCanExecute = value;
            effective = value && _inFlight == 0;
            changed = effective != _canExecuteValue;
            if (changed)
            {
                _canExecuteValue = effective;
            }
        }

        if (!changed)
        {
            return;
        }

        OnCanExecuteChanged(effective);
        _canExecuteBroadcaster.Next(effective);
    }

    /// <summary>Routes a canExecute error to the exceptions stream and treats the command as not executable.</summary>
    /// <param name="error">The error from the user canExecute observable.</param>
    private void OnUserCanExecuteError(Exception error)
    {
        DeliverException(error);
        OnUserCanExecute(false);
    }

    /// <summary>Schedules an execution-begin transition on the output scheduler.</summary>
    private void NotifyBegin() =>
        _outputScheduler.ScheduleOrInline(this, static (_, command) =>
        {
            command.ApplyBegin();
            return EmptyDisposable.Instance;
        });

    /// <summary>Schedules an execution-end transition on the output scheduler.</summary>
    private void NotifyEnd() =>
        _outputScheduler.ScheduleOrInline(this, static (_, command) =>
        {
            command.ApplyEnd();
            return EmptyDisposable.Instance;
        });

    /// <summary>Schedules a result broadcast to command subscribers on the output scheduler.</summary>
    /// <param name="result">The result value to broadcast.</param>
    private void NotifyResult(TResult result) =>
        _outputScheduler.ScheduleOrInline((Command: this, Result: result), static (_, state) =>
        {
            state.Command._resultsBroadcaster.Next(state.Result);
            return EmptyDisposable.Instance;
        });

    /// <summary>Schedules exception delivery on the output scheduler.</summary>
    /// <param name="error">The exception to deliver.</param>
    private void DeliverException(Exception error) =>
        _outputScheduler.ScheduleOrInline((Command: this, Error: error), static (_, state) =>
        {
            state.Command.ApplyException(state.Error);
            return EmptyDisposable.Instance;
        });

    /// <summary>Applies an execution-begin: increments in-flight count and updates isExecuting / canExecute.</summary>
    private void ApplyBegin()
    {
        bool isExecutingChanged;
        bool canExecuteChanged;
        bool canExecuteValue;
        lock (_gate)
        {
            isExecutingChanged = _inFlight == 0;
            _inFlight++;
            if (isExecutingChanged)
            {
                _isExecutingValue = true;
            }

            canExecuteValue = _userCanExecute && _inFlight == 0;
            canExecuteChanged = canExecuteValue != _canExecuteValue;
            if (canExecuteChanged)
            {
                _canExecuteValue = canExecuteValue;
            }
        }

        if (isExecutingChanged)
        {
            _isExecutingBroadcaster.Next(true);
        }

        if (!canExecuteChanged)
        {
            return;
        }

        OnCanExecuteChanged(canExecuteValue);
        _canExecuteBroadcaster.Next(canExecuteValue);
    }

    /// <summary>Applies an execution-end: decrements in-flight count and updates isExecuting / canExecute.</summary>
    private void ApplyEnd()
    {
        bool isExecutingChanged;
        bool canExecuteChanged;
        bool canExecuteValue;
        lock (_gate)
        {
            if (_inFlight > 0)
            {
                _inFlight--;
            }

            isExecutingChanged = _inFlight == 0 && _isExecutingValue;
            if (isExecutingChanged)
            {
                _isExecutingValue = false;
            }

            canExecuteValue = _userCanExecute && _inFlight == 0;
            canExecuteChanged = canExecuteValue != _canExecuteValue;
            if (canExecuteChanged)
            {
                _canExecuteValue = canExecuteValue;
            }
        }

        if (isExecutingChanged)
        {
            _isExecutingBroadcaster.Next(false);
        }

        if (!canExecuteChanged)
        {
            return;
        }

        OnCanExecuteChanged(canExecuteValue);
        _canExecuteBroadcaster.Next(canExecuteValue);
    }

    /// <summary>Delivers an exception to subscribers, or to the default handler when there are none.</summary>
    /// <param name="error">The exception to deliver.</param>
    private void ApplyException(Exception error)
    {
        bool hasObservers;
        lock (_gate)
        {
            hasObservers = _exceptionsBroadcaster.HasObservers;
        }

        if (hasObservers)
        {
            _exceptionsBroadcaster.Next(error);
            return;
        }

        RxState.DefaultExceptionHandler.OnNext(error);
    }

    /// <summary>Removes a results observer. Called when a command subscription is disposed.</summary>
    /// <param name="observer">The observer to remove.</param>
    private void RemoveResult(IObserver<TResult> observer)
    {
        lock (_gate)
        {
            _resultsBroadcaster.Remove(observer);
        }
    }

    /// <summary>Adds a canExecute / isExecuting observer and returns the value to replay to it.</summary>
    /// <param name="observer">The observer to add.</param>
    /// <param name="isExecuting"><see langword="true"/> for the isExecuting stream; otherwise the canExecute stream.</param>
    /// <returns>The current value to replay to the new observer.</returns>
    private bool AddBool(IObserver<bool> observer, bool isExecuting)
    {
        lock (_gate)
        {
            if (isExecuting)
            {
                _isExecutingBroadcaster.Add(observer);
                return _isExecutingValue;
            }

            _canExecuteBroadcaster.Add(observer);
            return _canExecuteValue;
        }
    }

    /// <summary>Removes a canExecute / isExecuting observer.</summary>
    /// <param name="observer">The observer to remove.</param>
    /// <param name="isExecuting"><see langword="true"/> for the isExecuting stream; otherwise the canExecute stream.</param>
    private void RemoveBool(IObserver<bool> observer, bool isExecuting)
    {
        lock (_gate)
        {
            if (isExecuting)
            {
                _isExecutingBroadcaster.Remove(observer);
                return;
            }

            _canExecuteBroadcaster.Remove(observer);
        }
    }

    /// <summary>Adds an exceptions observer.</summary>
    /// <param name="observer">The observer to add.</param>
    private void AddException(IObserver<Exception> observer)
    {
        lock (_gate)
        {
            _exceptionsBroadcaster.Add(observer);
        }
    }

    /// <summary>Removes an exceptions observer.</summary>
    /// <param name="observer">The observer to remove.</param>
    private void RemoveException(IObserver<Exception> observer)
    {
        lock (_gate)
        {
            _exceptionsBroadcaster.Remove(observer);
        }
    }

    /// <summary>The <see cref="CanExecute"/> / <see cref="IsExecuting"/> stream; replays the current value on subscribe.</summary>
    /// <param name="owner">The owning command.</param>
    /// <param name="isExecuting"><see langword="true"/> for isExecuting; otherwise canExecute.</param>
    private sealed class BoolStream(ReactiveCommand<TParam, TResult> owner, bool isExecuting) : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var current = owner.AddBool(observer, isExecuting);
            observer.OnNext(current);
            return new BoolSubscription(owner, observer, isExecuting);
        }
    }

    /// <summary>Unsubscribes a canExecute / isExecuting observer on dispose.</summary>
    /// <param name="owner">The owning command.</param>
    /// <param name="observer">The subscribed observer.</param>
    /// <param name="isExecuting">Which stream the observer belongs to.</param>
    private sealed class BoolSubscription(ReactiveCommand<TParam, TResult> owner, IObserver<bool> observer, bool isExecuting) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => owner.RemoveBool(observer, isExecuting);
    }

    /// <summary>The <see cref="ThrownExceptions"/> stream.</summary>
    /// <param name="owner">The owning command.</param>
    private sealed class ExceptionStream(ReactiveCommand<TParam, TResult> owner) : IObservable<Exception>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Exception> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            owner.AddException(observer);
            return new ExceptionSubscription(owner, observer);
        }
    }

    /// <summary>Unsubscribes an exceptions observer on dispose.</summary>
    /// <param name="owner">The owning command.</param>
    /// <param name="observer">The subscribed observer.</param>
    private sealed class ExceptionSubscription(ReactiveCommand<TParam, TResult> owner, IObserver<Exception> observer) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => owner.RemoveException(observer);
    }

    /// <summary>Unsubscribes a results observer on dispose.</summary>
    /// <param name="owner">The owning command.</param>
    /// <param name="observer">The subscribed observer.</param>
    private sealed class ResultSubscription(ReactiveCommand<TParam, TResult> owner, IObserver<TResult> observer) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => owner.RemoveResult(observer);
    }

    /// <summary>The cold observable returned by <see cref="Execute(TParam)"/>; each subscription runs one execution.</summary>
    /// <param name="owner">The owning command.</param>
    /// <param name="parameter">The parameter for this execution.</param>
    private sealed class ExecuteObservable(ReactiveCommand<TParam, TResult> owner, TParam parameter) : IObservable<TResult>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Execution(owner, observer).Run(parameter);
        }
    }

    /// <summary>
    /// Drives a single execution: begins, subscribes to the produced result observable, forwards results to the
    /// caller (raw) while scheduling result/exception/end notifications, and balances the begin with exactly one end.
    /// </summary>
    /// <param name="owner">The owning command.</param>
    /// <param name="downstream">The observer subscribed to this execution.</param>
    private sealed class Execution(ReactiveCommand<TParam, TResult> owner, IObserver<TResult> downstream)
        : IObserver<(IObservable<TResult> Result, Action Cancel)>, IDisposable
    {
        /// <summary>Subscription to the execution-source observable (the result/cancel tuple producer).</summary>
        private IDisposable? _outer;

        /// <summary>Subscription to the produced result observable.</summary>
        private IDisposable? _inner;

        /// <summary>The cancellation callback supplied by the execution source.</summary>
        private Action? _cancel;

        /// <summary>Guards the once-only execution-end (0 = not fired, 1 = fired).</summary>
        private int _endFired;

        /// <summary>Begins the execution and subscribes to the execution source.</summary>
        /// <param name="parameter">The parameter for this execution.</param>
        /// <returns>A disposable that cancels the execution.</returns>
        public IDisposable Run(TParam parameter)
        {
            owner.NotifyBegin();
            try
            {
                _outer = owner._execute(parameter).Subscribe(this);
            }
            catch (Exception ex)
            {
                EndOnce();
                owner.DeliverException(ex);
                downstream.OnError(ex);
                return EmptyDisposable.Instance;
            }

            return this;
        }

        /// <inheritdoc/>
        public void OnNext((IObservable<TResult> Result, Action Cancel) value)
        {
            _cancel = value.Cancel;
            _inner = value.Result.Subscribe(new ResultObserver(this));
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            EndOnce();
            owner.DeliverException(error);
            downstream.OnError(error);
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Capture before tearing down: disposing the inner subscription disposes the async cancellation source,
            // so the cancel action must be invoked first (while it is still valid).
            var cancel = _cancel;

            // Cancel first, while the cancellation source is still alive: disposing the inner subscription tears down
            // the async run (which disposes that source), so invoking the cancel action afterwards would hit a
            // disposed CancellationTokenSource.
            cancel?.Invoke();
            _inner?.Dispose();
            _outer?.Dispose();

            // For asynchronous executions the cancelled task keeps running until it actually completes, and
            // IsExecuting must stay true until then — its terminal notification (OnError / OnResultCompleted) raises
            // the balancing end. For synchronous/observable executions cancellation is performed by unsubscribing
            // (NoCancel), so disposal ends the execution here.
            if (cancel is not null && cancel != NoCancel)
            {
                return;
            }

            EndOnce();
        }

        /// <summary>Schedules the balancing execution-end exactly once.</summary>
        private void EndOnce()
        {
            if (Interlocked.Exchange(ref _endFired, 1) != 0)
            {
                return;
            }

            owner.NotifyEnd();
        }

        /// <summary>Forwards a produced result to the caller (raw) and broadcasts it to command subscribers.</summary>
        /// <param name="value">The produced result.</param>
        private void OnResult(TResult value)
        {
            downstream.OnNext(value);
            owner.NotifyResult(value);
        }

        /// <summary>Completes this execution once the result observable completes.</summary>
        private void OnResultCompleted()
        {
            EndOnce();
            downstream.OnCompleted();
        }

        /// <summary>Forwards the produced result observable's notifications back into the execution.</summary>
        /// <param name="execution">The owning execution.</param>
        private sealed class ResultObserver(Execution execution) : IObserver<TResult>
        {
            /// <inheritdoc/>
            public void OnNext(TResult value) => execution.OnResult(value);

            /// <inheritdoc/>
            public void OnError(Exception error) => execution.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => execution.OnResultCompleted();
        }
    }
}
