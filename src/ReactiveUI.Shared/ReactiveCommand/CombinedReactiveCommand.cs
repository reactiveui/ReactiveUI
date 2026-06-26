// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Encapsulates a composite user interaction.</summary>
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
/// <typeparam name="TParam">
/// The type of parameter values passed in during command execution.
/// </typeparam>
/// <typeparam name="TResult">
/// The type of the values that are the result of command execution.
/// </typeparam>
public class CombinedReactiveCommand<TParam, TResult> : ReactiveCommandBase<TParam, IList<TResult>>
{
    /// <summary>The inner command that executes all child commands and aggregates their results.</summary>
    private readonly ReactiveCommand<TParam, IList<TResult>> _innerCommand;

    /// <summary>Subscription that drives the <see cref="System.Windows.Input.ICommand"/> CanExecuteChanged event.</summary>
    private readonly IDisposable _canExecuteSubscription;

    /// <summary>Subscription that observes (and discards) the inner command's exceptions to keep them handled.</summary>
    private readonly IDisposable _innerExceptionsSubscription;

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

        ThrownExceptions = new MergedExceptionsObservable(exceptionSources);
        _innerExceptionsSubscription = _innerCommand.ThrownExceptions.Subscribe(new DelegateObserver<Exception>(static _ => { }));

        _canExecuteSubscription = CanExecute.Subscribe(new DelegateObserver<bool>(OnCanExecuteChanged));
    }

    /// <inheritdoc/>
    public override IObservable<bool> CanExecute => _innerCommand.CanExecute;

    /// <inheritdoc/>
    public override IObservable<bool> IsExecuting => _innerCommand.IsExecuting;

    /// <inheritdoc/>
    public override IObservable<Exception> ThrownExceptions { get; }

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

    /// <summary>
    /// Combines the latest <c>CanExecute</c> of the parent gate and every child command, emitting true only when all of
    /// them are currently true. Specialised to the combined command; no generic combine-latest operator.
    /// </summary>
    /// <param name="sources">The parent gate followed by each child command's <c>CanExecute</c>.</param>
    private sealed class AllTrueCanExecuteObservable(IObservable<bool>[] sources) : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, sources);
        }

        /// <summary>Tracks the latest value of each source and emits their all-true when every source has reported.</summary>
        private sealed class Sink : IDisposable
        {
            /// <summary>Guards the latest values and the arrival/completion counters.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the combined value.</summary>
            private readonly IObserver<bool> _downstream;

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

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to every source.</summary>
            /// <param name="downstream">The observer receiving the combined value.</param>
            /// <param name="sources">The sources to combine.</param>
            public Sink(IObserver<bool> downstream, IObservable<bool>[] sources)
            {
                _downstream = downstream;
                _latest = new bool[sources.Length];
                _has = new bool[sources.Length];
                _subscriptions = new IDisposable?[sources.Length];
                for (var i = 0; i < sources.Length; i++)
                {
                    _subscriptions[i] = sources[i].Subscribe(new Element(this, i));
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                for (var i = 0; i < _subscriptions.Length; i++)
                {
                    _subscriptions[i]?.Dispose();
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
                    if (_stopped)
                    {
                        return;
                    }

                    if (!_has[index])
                    {
                        _has[index] = true;
                        _haveCount++;
                    }

                    _latest[index] = value;
                    if (_haveCount < _latest.Length)
                    {
                        return;
                    }

                    all = true;
                    for (var j = 0; j < _latest.Length; j++)
                    {
                        if (!_latest[j])
                        {
                            all = false;
                            break;
                        }
                    }
                }

                _downstream.OnNext(all);
            }

            /// <summary>Forwards an error from any source.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAt(Exception error)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnError(error);
            }

            /// <summary>Completes the downstream once every source has completed.</summary>
            private void OnCompletedAt()
            {
                lock (_gate)
                {
                    if (_stopped || ++_doneCount < _latest.Length)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnCompleted();
            }

            /// <summary>Routes one source's notifications to the parent sink, tagged with its index.</summary>
            /// <param name="parent">The owning sink.</param>
            /// <param name="index">The source index.</param>
            private sealed class Element(Sink parent, int index) : IObserver<bool>
            {
                /// <inheritdoc/>
                public void OnNext(bool value) => parent.OnNextAt(index, value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }

    /// <summary>
    /// Combines the latest result of every child execution into a single list, emitting once every child has produced a
    /// value. Specialised to the combined command; no generic combine-latest operator.
    /// </summary>
    /// <param name="sources">The in-flight execution of each child command.</param>
    private sealed class CombinedResultsObservable(IObservable<TResult>[] sources) : IObservable<IList<TResult>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IList<TResult>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, sources);
        }

        /// <summary>Tracks the latest result of each child and emits their list once every child has reported.</summary>
        private sealed class Sink : IDisposable
        {
            /// <summary>Guards the latest results and the arrival/completion counters.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the combined list.</summary>
            private readonly IObserver<IList<TResult>> _downstream;

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

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to every child execution.</summary>
            /// <param name="downstream">The observer receiving the combined list.</param>
            /// <param name="sources">The child executions to combine.</param>
            public Sink(IObserver<IList<TResult>> downstream, IObservable<TResult>[] sources)
            {
                _downstream = downstream;
                _latest = new TResult[sources.Length];
                _has = new bool[sources.Length];
                _subscriptions = new IDisposable?[sources.Length];
                for (var i = 0; i < sources.Length; i++)
                {
                    _subscriptions[i] = sources[i].Subscribe(new Element(this, i));
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                for (var i = 0; i < _subscriptions.Length; i++)
                {
                    _subscriptions[i]?.Dispose();
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
                    if (_stopped)
                    {
                        return;
                    }

                    if (!_has[index])
                    {
                        _has[index] = true;
                        _haveCount++;
                    }

                    _latest[index] = value;
                    if (_haveCount < _latest.Length)
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
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnError(error);
            }

            /// <summary>Completes the downstream once every child execution has completed.</summary>
            private void OnCompletedAt()
            {
                lock (_gate)
                {
                    if (_stopped || ++_doneCount < _latest.Length)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnCompleted();
            }

            /// <summary>Routes one child execution's notifications to the parent sink, tagged with its index.</summary>
            /// <param name="parent">The owning sink.</param>
            /// <param name="index">The child index.</param>
            private sealed class Element(Sink parent, int index) : IObserver<TResult>
            {
                /// <inheritdoc/>
                public void OnNext(TResult value) => parent.OnNextAt(index, value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }

    /// <summary>
    /// Surfaces only the failure of a CanExecute source as a single exception, ignoring its boolean values and normal
    /// completion. Used so a parent-gate error reaches the combined command's exception stream once.
    /// </summary>
    /// <param name="source">The parent CanExecute gate to observe for failure.</param>
    private sealed class CanExecuteFailureObservable(IObservable<bool> source) : IObservable<Exception>
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
    private sealed class MergedExceptionsObservable(IObservable<Exception>[] sources) : IObservable<Exception>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<Exception> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, sources);
        }

        /// <summary>Forwards every source value and completes once every source has completed.</summary>
        private sealed class Sink : IDisposable
        {
            /// <summary>Guards downstream delivery and the completion counter.</summary>
#if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
#else
            private readonly object _gate = new();
#endif

            /// <summary>The observer receiving the merged exceptions.</summary>
            private readonly IObserver<Exception> _downstream;

            /// <summary>The subscriptions to each source.</summary>
            private readonly IDisposable?[] _subscriptions;

            /// <summary>The number of sources.</summary>
            private readonly int _count;

            /// <summary>The number of sources that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to every source.</summary>
            /// <param name="downstream">The observer receiving the merged exceptions.</param>
            /// <param name="sources">The exception streams to merge.</param>
            public Sink(IObserver<Exception> downstream, IObservable<Exception>[] sources)
            {
                _downstream = downstream;
                _count = sources.Length;
                _subscriptions = new IDisposable?[sources.Length];
                for (var i = 0; i < sources.Length; i++)
                {
                    _subscriptions[i] = sources[i].Subscribe(new Element(this));
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                for (var i = 0; i < _subscriptions.Length; i++)
                {
                    _subscriptions[i]?.Dispose();
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
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnError(error);
            }

            /// <summary>Completes the downstream once every source has completed.</summary>
            private void OnCompletedAt()
            {
                lock (_gate)
                {
                    if (_stopped || ++_doneCount < _count)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnCompleted();
            }

            /// <summary>Routes one source's notifications to the parent sink.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class Element(Sink parent) : IObserver<Exception>
            {
                /// <inheritdoc/>
                public void OnNext(Exception value) => parent.OnNextAt(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }
}
