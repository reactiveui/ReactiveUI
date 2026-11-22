// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
#if NET9_0_OR_GREATER
using GateLock = System.Threading.Lock;
#else
using GateLock = object;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Encapsulates a composite user interaction.</summary>
/// <typeparam name="TParam">
/// The type of parameter values passed in during command execution.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the values that are the result of command execution.
/// </typeparam>
/// <remarks>
/// <para>
/// This class provides the bulk of the actual implementation for combined reactive commands. You should not
/// create instances of this class directly, but rather via the static creation methods on the non-generic
/// <see cref="ReactiveCommand"/> class.
/// </para>
/// <para>
/// A <c>CombinedReactiveCommand</c> combines multiple reactive commands into a single command. Executing
/// the combined command executes all child commands. Since all child commands will receive the same execution
/// parameter, all child commands must accept a parameter of the same type.
/// </para>
/// <para>
/// In order for the combined command to be executable, all child commands must themselves be executable.
/// In addition, any <c>canExecute</c> observable passed in during construction must also yield <c>true</c>.
/// </para>
/// </remarks>
[DebuggerDisplay("{CanExecute}, {IsExecuting}")]
public class CombinedReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, IList<TResult>>
{
    /// <summary>The inner command that executes all child commands and aggregates their results.</summary>
    private readonly ReactiveCommand<TParam, IList<TResult>> _innerCommand;

    /// <summary>Subscription that drives the <see cref="System.Windows.Input.ICommand"/> CanExecuteChanged event.</summary>
    private readonly IDisposable _canExecuteSubscription;

    /// <summary>Subscription that observes (and discards) the inner command's exceptions to keep them handled.</summary>
    private readonly IDisposable _innerExceptionsSubscription;

    /// <summary>The merged exception stream surfaced by <see cref="ThrownExceptions"/>.</summary>
    private readonly MergedExceptionsObservable _thrownExceptions;

    /// <summary>Initializes a new instance of the <see cref="CombinedReactiveCommand{TParam, TResult}"/> class using the default output scheduler.</summary>
    /// <param name="childCommands">The child commands which will be executed.</param>
    /// <param name="canExecute">An observable indicating when the command can be executed.</param>
    /// <exception cref="ArgumentNullException">Fires when required arguments are null.</exception>
    /// <exception cref="ArgumentException">Fires if the child commands container is empty.</exception>
    protected internal CombinedReactiveCommand(
        IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
        IObservable<bool>? canExecute)
        : this(childCommands, canExecute, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CombinedReactiveCommand{TParam, TResult}"/> class using the default can-execute behavior.</summary>
    /// <param name="childCommands">The child commands which will be executed.</param>
    /// <param name="outputScheduler">The scheduler where to dispatch the output from the command.</param>
    /// <exception cref="ArgumentNullException">Fires when required arguments are null.</exception>
    /// <exception cref="ArgumentException">Fires if the child commands container is empty.</exception>
    protected internal CombinedReactiveCommand(
        IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
        ISequencer? outputScheduler)
        : this(childCommands, null, outputScheduler)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CombinedReactiveCommand{TParam, TResult}"/> class.</summary>
    /// <param name="childCommands">The child commands which will be executed.</param>
    /// <param name="canExecute">A observable when the command can be executed.</param>
    /// <param name="outputScheduler">The scheduler where to dispatch the output from the command.</param>
    /// <exception cref="ArgumentNullException">Fires when required arguments are null.</exception>
    /// <exception cref="ArgumentException">Fires if the child commands container is empty.</exception>
    protected internal CombinedReactiveCommand(
        IEnumerable<ReactiveCommandBase<TParam, TResult>> childCommands,
        IObservable<bool>? canExecute,
        ISequencer? outputScheduler)
    {
        ArgumentExceptionHelper.ThrowIfNull(childCommands);

        var localOutputScheduler = outputScheduler ?? RxSchedulers.MainThreadScheduler;

        ReactiveCommandBase<TParam, TResult>[] childCommandsArray = [.. childCommands];

        if (childCommandsArray.Length == 0)
        {
            throw new ArgumentException("No child commands provided.", nameof(childCommands));
        }

        var childCanExecute = new IObservable<bool>[childCommandsArray.Length];
        for (var i = 0; i < childCommandsArray.Length; i++)
        {
            childCanExecute[i] = childCommandsArray[i].CanExecute;
        }

        var parentGate = canExecute ?? new ReturnSignal<bool>(true, Sequencer.Immediate);

        // all-true of [parent, child0..childN] (the parent gate plus every child's CanExecute).
        var canExecuteSources = new IObservable<bool>[childCommandsArray.Length + 1];
        canExecuteSources[0] = parentGate;
        Array.Copy(childCanExecute, 0, canExecuteSources, 1, childCanExecute.Length);
        var combinedCanExecute = new AllTrueCanExecuteObservable(canExecuteSources);

        _innerCommand = new(
            param =>
            {
                var executions = new IObservable<TResult>[childCommandsArray.Length];
                for (var i = 0; i < childCommandsArray.Length; i++)
                {
                    executions[i] = childCommandsArray[i].Execute(param);
                }

                return new CombinedResultsObservable(executions);
            },
            combinedCanExecute,
            localOutputScheduler);

        // The public exception stream surfaces each child command's exceptions plus any failure of the parent gate,
        // each exactly once. A child error also propagates through the inner command (it aggregates the child
        // executions), as does a parent-gate failure, so the inner command's ThrownExceptions would re-report the
        // same exceptions; observe and discard it separately to keep them handled without double-counting.
        var exceptionSources = new IObservable<Exception>[childCommandsArray.Length + 1];
        for (var i = 0; i < childCommandsArray.Length; i++)
        {
            exceptionSources[i] = childCommandsArray[i].ThrownExceptions;
        }

        exceptionSources[childCommandsArray.Length] = new CanExecuteFailureObservable(parentGate);

        _thrownExceptions = new(exceptionSources);
        _innerExceptionsSubscription = _innerCommand.ThrownExceptions.Subscribe(new DelegateObserver<Exception>(static _ => { }));

        _canExecuteSubscription = _innerCommand.CanExecute.Subscribe(new DelegateObserver<bool>(OnCanExecuteChanged));
    }

    /// <inheritdoc/>
    public override IObservable<bool> CanExecute => _innerCommand.CanExecute;

    /// <inheritdoc/>
    public override IObservable<bool> IsExecuting => _innerCommand.IsExecuting;

    /// <inheritdoc/>
    public override IObservable<Exception> ThrownExceptions => _thrownExceptions;

    /// <inheritdoc/>
    public override IDisposable Subscribe(IObserver<IList<TResult>> observer) => _innerCommand.Subscribe(observer);

    /// <inheritdoc/>
    public override IObservable<IList<TResult>> Execute(TParam parameter) => _innerCommand.Execute(parameter);

    /// <inheritdoc/>
    public override IObservable<IList<TResult>> Execute() => _innerCommand.Execute();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _canExecuteSubscription.Dispose();
        _innerExceptionsSubscription.Dispose();
        _innerCommand.Dispose();
    }

    /// <summary>Disposes every non-null subscription in the array.</summary>
    /// <param name="subscriptions">The subscriptions to dispose.</param>
    private static void DisposeAll(IDisposable?[] subscriptions)
    {
        for (var i = 0; i < subscriptions.Length; i++)
        {
            subscriptions[i]?.Dispose();
        }
    }

    /// <summary>Records an arrival for one source while holding the sink's gate.</summary>
    /// <typeparam name="T">The source value type.</typeparam>
    /// <param name="index">The source index.</param>
    /// <param name="value">The reported value.</param>
    /// <param name="latest">The latest values array.</param>
    /// <param name="has">The arrival-tracking array.</param>
    /// <param name="haveCount">The count of sources that have reported at least once.</param>
    /// <returns><see langword="true"/> once every source has reported at least one value.</returns>
    private static bool TryRecordValue<T>(int index, T value, T[] latest, bool[] has, ref int haveCount)
    {
        if (!has[index])
        {
            has[index] = true;
            haveCount++;
        }

        latest[index] = value;
        return haveCount >= latest.Length;
    }

    /// <summary>Marks a sink stopped on error if it has not already terminated.</summary>
    /// <param name="gate">The sink's synchronization gate.</param>
    /// <param name="stopped">The sink's stopped flag.</param>
    /// <returns><see langword="true"/> if the caller should forward the error downstream.</returns>
    private static bool TryFail(GateLock gate, ref bool stopped)
    {
        lock (gate)
        {
            if (stopped)
            {
                return false;
            }

            stopped = true;
            return true;
        }
    }

    /// <summary>Records one source completion and marks the sink stopped once all sources have completed.</summary>
    /// <param name="gate">The sink's synchronization gate.</param>
    /// <param name="stopped">The sink's stopped flag.</param>
    /// <param name="doneCount">The count of completed sources.</param>
    /// <param name="count">The total number of sources.</param>
    /// <returns><see langword="true"/> if the caller should complete the downstream observer.</returns>
    private static bool TryComplete(GateLock gate, ref bool stopped, ref int doneCount, int count)
    {
        lock (gate)
        {
            if (stopped)
            {
                return false;
            }

            doneCount++;
            if (doneCount < count)
            {
                return false;
            }

            stopped = true;
            return true;
        }
    }

    /// <summary>
    /// Combines the latest <c>CanExecute</c> of the parent gate and every child command, emitting true only when all of
    /// them are currently true. Specialised to the combined command; no generic combine-latest operator.
    /// </summary>
    /// <param name="sources">The parent gate followed by each child command's <c>CanExecute</c>.</param>
    internal sealed class AllTrueCanExecuteObservable(IObservable<bool>[] sources) : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(observer, sources);
            sink.Run();
            return sink;
        }

        /// <summary>Tracks the latest value of each source and emits their all-true when every source has reported.</summary>
        internal sealed class Sink : IDisposable
        {
            /// <summary>Guards the latest values and the arrival/completion counters.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the combined value.</summary>
            private readonly IObserver<bool> _downstream;

            /// <summary>The sources to combine.</summary>
            private readonly IObservable<bool>[] _sources;

            /// <summary>The latest value reported by each source.</summary>
            private readonly bool[] _latest;

            /// <summary>Whether each source has reported at least one value.</summary>
            private readonly bool[] _has;

            /// <summary>The subscriptions to each source.</summary>
            private readonly IDisposable?[] _subscriptions;

            /// <summary>The number of sources that have reported at least one value.</summary>
            private int _haveCount;

            /// <summary>The number of sources that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class.</summary>
            /// <param name="downstream">The observer receiving the combined value.</param>
            /// <param name="sources">The sources to combine.</param>
            public Sink(IObserver<bool> downstream, IObservable<bool>[] sources)
            {
                _downstream = downstream;
                _sources = sources;
                _latest = new bool[sources.Length];
                _has = new bool[sources.Length];
                _subscriptions = new IDisposable?[sources.Length];
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => DisposeAll(_subscriptions);

            /// <summary>Subscribes to every source, driving the combined value.</summary>
            internal void Run()
            {
                for (var i = 0; i < _sources.Length; i++)
                {
                    _subscriptions[i] = _sources[i].Subscribe(new Element(this, i));
                }
            }

            /// <summary>Records a source value and emits the all-true once every source has reported.</summary>
            /// <param name="index">The source index.</param>
            /// <param name="value">The reported value.</param>
            private void OnNextAt(int index, bool value)
            {
                bool all;
                lock (_gate)
                {
                    if (_stopped || !TryRecordValue(index, value, _latest, _has, ref _haveCount))
                    {
                        return;
                    }

                    all = true;
                    for (var j = 0; j < _latest.Length; j++)
                    {
                        if (_latest[j])
                        {
                            continue;
                        }

                        all = false;
                        break;
                    }
                }

                _downstream.OnNext(all);
            }

            /// <summary>Forwards an error from any source.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAt(Exception error)
            {
                if (TryFail(_gate, ref _stopped))
                {
                    _downstream.OnError(error);
                }
            }

            /// <summary>Completes the downstream once every source has completed.</summary>
            private void OnCompletedAt()
            {
                if (TryComplete(_gate, ref _stopped, ref _doneCount, _latest.Length))
                {
                    _downstream.OnCompleted();
                }
            }

            /// <summary>Routes one source's notifications to the parent sink, tagged with its index.</summary>
            /// <param name="parent">The owning sink.</param>
            /// <param name="index">The source index.</param>
            private sealed class Element(Sink parent, int index) : IObserver<bool>
            {
                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnNext(bool value) => parent.OnNextAt(index, value);

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }

    /// <summary>
    /// Combines the latest result of every child execution into a single list, emitting once every child has produced a
    /// value. Specialised to the combined command; no generic combine-latest operator.
    /// </summary>
    /// <param name="sources">The in-flight execution of each child command.</param>
    internal sealed class CombinedResultsObservable(IObservable<TResult>[] sources) : IObservable<IList<TResult>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IList<TResult>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(observer, sources);
            sink.Run();
            return sink;
        }

        /// <summary>Tracks the latest result of each child and emits their list once every child has reported.</summary>
        internal sealed class Sink : IDisposable
        {
            /// <summary>Guards the latest results and the arrival/completion counters.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the combined list.</summary>
            private readonly IObserver<IList<TResult>> _downstream;

            /// <summary>The child executions to combine.</summary>
            private readonly IObservable<TResult>[] _sources;

            /// <summary>The latest result reported by each child.</summary>
            private readonly TResult[] _latest;

            /// <summary>Whether each child has reported at least one result.</summary>
            private readonly bool[] _has;

            /// <summary>The subscriptions to each child execution.</summary>
            private readonly IDisposable?[] _subscriptions;

            /// <summary>The number of children that have reported at least one result.</summary>
            private int _haveCount;

            /// <summary>The number of children that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class.</summary>
            /// <param name="downstream">The observer receiving the combined list.</param>
            /// <param name="sources">The child executions to combine.</param>
            public Sink(IObserver<IList<TResult>> downstream, IObservable<TResult>[] sources)
            {
                _downstream = downstream;
                _sources = sources;
                _latest = new TResult[sources.Length];
                _has = new bool[sources.Length];
                _subscriptions = new IDisposable?[sources.Length];
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => DisposeAll(_subscriptions);

            /// <summary>Subscribes to every child execution, driving the combined list.</summary>
            internal void Run()
            {
                for (var i = 0; i < _sources.Length; i++)
                {
                    _subscriptions[i] = _sources[i].Subscribe(new Element(this, i));
                }
            }

            /// <summary>Records a child result and emits the combined list once every child has reported.</summary>
            /// <param name="index">The child index.</param>
            /// <param name="value">The reported result.</param>
            private void OnNextAt(int index, TResult value)
            {
                IList<TResult> list;
                lock (_gate)
                {
                    if (_stopped || !TryRecordValue(index, value, _latest, _has, ref _haveCount))
                    {
                        return;
                    }

                    list = [.. _latest];
                }

                _downstream.OnNext(list);
            }

            /// <summary>Forwards an error from any child execution.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAt(Exception error)
            {
                if (TryFail(_gate, ref _stopped))
                {
                    _downstream.OnError(error);
                }
            }

            /// <summary>Completes the downstream once every child execution has completed.</summary>
            private void OnCompletedAt()
            {
                if (TryComplete(_gate, ref _stopped, ref _doneCount, _latest.Length))
                {
                    _downstream.OnCompleted();
                }
            }

            /// <summary>Routes one child execution's notifications to the parent sink, tagged with its index.</summary>
            /// <param name="parent">The owning sink.</param>
            /// <param name="index">The child index.</param>
            private sealed class Element(Sink parent, int index) : IObserver<TResult>
            {
                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnNext(TResult value) => parent.OnNextAt(index, value);

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }

    /// <summary>
    /// Surfaces only the failure of a CanExecute source as a single exception, ignoring its boolean values and normal
    /// completion. Used so a parent-gate error reaches the combined command's exception stream once.
    /// </summary>
    /// <param name="source">The parent CanExecute gate to observe for failure.</param>
    internal sealed class CanExecuteFailureObservable(IObservable<bool> source) : IObservable<Exception>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Exception> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer));
        }

        /// <summary>Translates the source's terminal signals: an error becomes a single value, completion passes through.</summary>
        /// <param name="downstream">The observer receiving the failure, if any.</param>
        private sealed class Sink(IObserver<Exception> downstream) : IObserver<bool>
        {
            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted() => downstream.OnCompleted();

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                downstream.OnNext(error);
                downstream.OnCompleted();
            }

            /// <inheritdoc/>
            public void OnNext(bool value)
            {
            }
        }
    }

    /// <summary>
    /// Forwards the thrown exceptions of every child command and the parent gate failure into one stream. Specialised
    /// to the combined command; no generic merge operator.
    /// </summary>
    /// <param name="sources">The child exception streams and the parent-gate failure stream.</param>
    internal sealed class MergedExceptionsObservable(IObservable<Exception>[] sources) : IObservable<Exception>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Exception> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(observer, sources);
            sink.Run();
            return sink;
        }

        /// <summary>Forwards every source value and completes once every source has completed.</summary>
        internal sealed class Sink : IDisposable
        {
            /// <summary>Guards downstream delivery and the completion counter.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the merged exceptions.</summary>
            private readonly IObserver<Exception> _downstream;

            /// <summary>The exception streams to merge.</summary>
            private readonly IObservable<Exception>[] _sources;

            /// <summary>The subscriptions to each source.</summary>
            private readonly IDisposable?[] _subscriptions;

            /// <summary>The number of sources.</summary>
            private readonly int _count;

            /// <summary>The number of sources that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class.</summary>
            /// <param name="downstream">The observer receiving the merged exceptions.</param>
            /// <param name="sources">The exception streams to merge.</param>
            public Sink(IObserver<Exception> downstream, IObservable<Exception>[] sources)
            {
                _downstream = downstream;
                _sources = sources;
                _count = sources.Length;
                _subscriptions = new IDisposable?[sources.Length];
            }

            /// <inheritdoc/>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Dispose() => DisposeAll(_subscriptions);

            /// <summary>Subscribes to every source, forwarding merged exceptions.</summary>
            internal void Run()
            {
                for (var i = 0; i < _sources.Length; i++)
                {
                    _subscriptions[i] = _sources[i].Subscribe(new Element(this));
                }
            }

            /// <summary>Forwards one source value to the downstream.</summary>
            /// <param name="value">The exception to forward.</param>
            private void OnNextAt(Exception value)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _downstream.OnNext(value);
                }
            }

            /// <summary>Forwards an error from any source.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAt(Exception error)
            {
                if (TryFail(_gate, ref _stopped))
                {
                    _downstream.OnError(error);
                }
            }

            /// <summary>Completes the downstream once every source has completed.</summary>
            private void OnCompletedAt()
            {
                if (TryComplete(_gate, ref _stopped, ref _doneCount, _count))
                {
                    _downstream.OnCompleted();
                }
            }

            /// <summary>Routes one source's notifications to the parent sink.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class Element(Sink parent) : IObserver<Exception>
            {
                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnNext(Exception value) => parent.OnNextAt(value);

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }
}
