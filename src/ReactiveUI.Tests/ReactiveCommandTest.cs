using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ReactiveCommandTest
    {
        [Fact]
        public void CreateThrowsIfExecutionParameterIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Action)null));
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit>)null));
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Action<Unit>)null));
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit, Unit>)null));
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<IObservable<Unit>>)null));
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Task<Unit>>)null));
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit, IObservable<Unit>>)null));
            Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit, Task<Unit>>)null));
        }

        [Fact]
        public void CanExecuteIsBehavioral()
        {
            var fixture = ReactiveCommand.Create(() => Observables.Unit);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            Assert.Equal(1, canExecute.Count);
            Assert.True(canExecute[0]);
        }

        [Fact]
        public void CanExecuteOnlyTicksDistinctValues()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(true);

            Assert.Equal(2, canExecute.Count);
            Assert.False(canExecute[0]);
            Assert.True(canExecute[1]);
        }

        [Fact]
        public void CanExecuteIsFalseIfCallerDictatesAsSuch()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(false);

            Assert.Equal(3, canExecute.Count);
            Assert.False(canExecute[0]);
            Assert.True(canExecute[1]);
            Assert.False(canExecute[2]);
        }

        [Fact]
        public void CanExecuteIsFalseIfAlreadyExecuting()
        {
            (new TestScheduler()).With(sched => {
                var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var canExecute = fixture
                    .CanExecute
                    .CreateCollection();

                fixture.Execute().Subscribe();
                sched.AdvanceByMs(100);

                Assert.Equal(2, canExecute.Count);
                Assert.False(canExecute[1]);

                sched.AdvanceByMs(901);

                Assert.Equal(3, canExecute.Count);
                Assert.True(canExecute[2]);
            });
        }

        [Fact]
        public void CanExecuteIsUnsubscribedAfterCommandDisposal()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject);

            Assert.True(canExecuteSubject.HasObservers);

            fixture.Dispose();

            Assert.False(canExecuteSubject.HasObservers);
        }

        [Fact]
        public void CanExecuteTicksFailuresThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void CanExecuteIsAvailableViaICommand()
        {
            var canExecuteSubject = new Subject<bool>();
            ICommand fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject);

            Assert.False(fixture.CanExecute(null));

            canExecuteSubject.OnNext(true);
            Assert.True(fixture.CanExecute(null));

            canExecuteSubject.OnNext(false);
            Assert.False(fixture.CanExecute(null));
        }

        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            var canExecuteSubject = new Subject<bool>();
            ICommand fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject);
            var canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecuteChanged.Count);
            Assert.True(canExecuteChanged[0]);
            Assert.False(canExecuteChanged[1]);
        }

        [Fact]
        public void IsExecutingIsBehavioral()
        {
            var fixture = ReactiveCommand.Create(() => Observables.Unit);
            var isExecuting = fixture
                .IsExecuting
                .CreateCollection();

            Assert.Equal(1, isExecuting.Count);
            Assert.False(isExecuting[0]);
        }

        [Fact]
        public void IsExecutingTicksAsExecutionsProgress()
        {
            (new TestScheduler()).With(sched => {
                var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var isExecuting = fixture
                    .IsExecuting
                    .CreateCollection();

                fixture.Execute().Subscribe();
                sched.AdvanceByMs(100);

                Assert.Equal(2, isExecuting.Count);
                Assert.False(isExecuting[0]);
                Assert.True(isExecuting[1]);

                sched.AdvanceByMs(901);

                Assert.Equal(3, isExecuting.Count);
                Assert.False(isExecuting[2]);
            });
        }

        [Fact]
        public void IsExecutingRemainsTrueAsLongAsExecutionPipelineHasNotCompleted()
        {
            var execute = new Subject<Unit>();
            var fixture = ReactiveCommand.CreateFromObservable(() => execute);

            fixture
                .Execute()
                .Subscribe();

            Assert.True(fixture.IsExecuting.FirstAsync().Wait());

            execute.OnNext(Unit.Default);
            Assert.True(fixture.IsExecuting.FirstAsync().Wait());

            execute.OnNext(Unit.Default);
            Assert.True(fixture.IsExecuting.FirstAsync().Wait());

            execute.OnCompleted();
            Assert.False(fixture.IsExecuting.FirstAsync().Wait());
        }

        [Fact]
        public void SynchronousCommandExecuteLazily()
        {
            var executionCount = 0;
            var fixture1 = ReactiveCommand.Create(() => { ++executionCount; });
            var fixture2 = ReactiveCommand.Create<int>(_ => { ++executionCount; });
            var fixture3 = ReactiveCommand.Create(() => { ++executionCount; return 42; });
            var fixture4 = ReactiveCommand.Create<int, int>(_ => { ++executionCount; return 42; });
            var execute1 = fixture1.Execute();
            var execute2 = fixture2.Execute();
            var execute3 = fixture3.Execute();
            var execute4 = fixture4.Execute();

            Assert.Equal(0, executionCount);

            execute1.Subscribe();
            Assert.Equal(1, executionCount);

            execute2.Subscribe();
            Assert.Equal(2, executionCount);

            execute3.Subscribe();
            Assert.Equal(3, executionCount);

            execute4.Subscribe();
            Assert.Equal(4, executionCount);
        }

        [Fact]
        public void SynchronousCommandsFailCorrectly()
        {
            var fixture1 = ReactiveCommand.Create(() => { throw new InvalidOperationException(); });
            var fixture2 = ReactiveCommand.Create<int>(_ => { throw new InvalidOperationException(); });
            var fixture3 = ReactiveCommand.Create(() => { throw new InvalidOperationException(); });
            var fixture4 = ReactiveCommand.Create<int, int>(_ => { throw new InvalidOperationException(); });

            var failureCount = 0;
            Observable
                .Merge(
                    fixture1.ThrownExceptions,
                    fixture2.ThrownExceptions,
                    fixture3.ThrownExceptions,
                    fixture4.ThrownExceptions)
                .Subscribe(_ => ++failureCount);

            fixture1
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });
            Assert.Equal(1, failureCount);

            fixture2
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });
            Assert.Equal(2, failureCount);

            fixture3
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });
            Assert.Equal(3, failureCount);

            fixture4
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });
            Assert.Equal(4, failureCount);
        }

        [Fact]
        public void ExecutePassesThroughParameter()
        {
            var parameters = new List<int>();
            var fixture = ReactiveCommand.CreateFromObservable<int, Unit>(param => {
                    parameters.Add(param);
                    return Observables.Unit;
                });

            fixture.Execute(1).Subscribe();
            fixture.Execute(42).Subscribe();
            fixture.Execute(348).Subscribe();

            Assert.Equal(3, parameters.Count);
            Assert.Equal(1, parameters[0]);
            Assert.Equal(42, parameters[1]);
            Assert.Equal(348, parameters[2]);
        }

        [Fact]
        public void ExecuteResultIsDeliveredOnSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched => {
                var execute = Observables.Unit;
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var executed = false;

                fixture.Execute().Subscribe(_ => executed = true);

                Assert.False(executed);
                sched.AdvanceByMs(1);
                Assert.True(executed);
            });
        }

        [Fact]
        public void ExecuteTicksThroughTheResult()
        {
            var num = 0;
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Return(num));
            var results = fixture
                .CreateCollection();

            num = 1;
            fixture.Execute().Subscribe();
            num = 10;
            fixture.Execute().Subscribe();
            num = 30;
            fixture.Execute().Subscribe();

            Assert.Equal(3, results.Count);
            Assert.Equal(1, results[0]);
            Assert.Equal(10, results[1]);
            Assert.Equal(30, results[2]);
        }

        [Fact]
        public void ExecuteCanTickThroughMultipleResults()
        {
            var fixture = ReactiveCommand.CreateFromObservable(() => new[] { 1, 2, 3 }.ToObservable());
            var results = fixture
                .CreateCollection();

            fixture.Execute().Subscribe();

            Assert.Equal(3, results.Count);
            Assert.Equal(1, results[0]);
            Assert.Equal(2, results[1]);
            Assert.Equal(3, results[2]);
        }

        [Fact]
        public void ExecuteTicksAnyException()
        {
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()));
            fixture
                .ThrownExceptions
                .Subscribe();
            Exception exception = null;
            fixture
                .Execute()
                .Subscribe(
                    _ => { },
                    ex => exception = ex,
                    () => { });

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void ExecuteTicksAnyLambdaException()
        {
            var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => { throw new InvalidOperationException(); });
            fixture
                .ThrownExceptions
                .Subscribe();
            Exception exception = null;
            fixture
                .Execute()
                .Subscribe(
                    _ => { },
                    ex => exception = ex,
                    () => { });

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void ExecuteCanBeCancelled()
        {
            (new TestScheduler()).With(sched => {
                var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var executed = fixture
                    .CreateCollection();

                var sub1 = fixture.Execute().Subscribe();
                var sub2 = fixture.Execute().Subscribe();
                sched.AdvanceByMs(999);

                Assert.True(fixture.IsExecuting.FirstAsync().Wait());
                Assert.Empty(executed);
                sub1.Dispose();

                sched.AdvanceByMs(2);
                Assert.Equal(1, executed.Count);
                Assert.False(fixture.IsExecuting.FirstAsync().Wait());
            });
        }

        [Fact]
        public void ExceptionsAreDeliveredOnOutputScheduler()
        {
            (new TestScheduler()).With(sched => {
                var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()), outputScheduler: sched);
                Exception exception = null;
                fixture
                    .ThrownExceptions
                    .Subscribe(ex => exception = ex);
                fixture
                    .Execute()
                    .Subscribe(
                        _ => { },
                        _ => { });

                Assert.Null(exception);
                sched.Start();
                Assert.IsType<InvalidOperationException>(exception);
            });
        }

        [Fact]
        public void ExecuteIsAvailableViaICommand()
        {
            var executed = false;
            ICommand fixture = ReactiveCommand.Create(() => {
                    executed = true;
                    return Observables.Unit;
                });

            fixture.Execute(null);
            Assert.True(executed);
        }

        [Fact]
        public void ExecuteViaICommandWorksWithNullableTypes()
        {
            int? value = null;
            ICommand fixture = ReactiveCommand.Create<int?>(param => {
                value = param;
            });

            fixture.Execute(42);
            Assert.Equal(42, value);

            fixture.Execute(null);
            Assert.Null(value);
        }

        [Fact]
        public void ExecuteViaICommandThrowsIfParameterTypeIsIncorrect()
        {
            ICommand fixture = ReactiveCommand.Create<int>(_ => { });
            var ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute("foo"));
            Assert.Equal("Command requires parameters of type System.Int32, but received parameter of type System.String.", ex.Message);

            fixture = ReactiveCommand.Create<string>(_ => { });
            ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute(13));
            Assert.Equal("Command requires parameters of type System.String, but received parameter of type System.Int32.", ex.Message);
        }

        [Fact]
        public void ResultIsTickedThroughSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched => {
                var fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: sched);
                var results = fixture
                    .CreateCollection();

                fixture.Execute().Subscribe();
                Assert.Empty(results);

                sched.AdvanceByMs(1);
                Assert.Equal(1, results.Count);
            });
        }

        [Fact]
        public void ExecuteTicksErrorsThroughThrownExceptions()
        {
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExecuteTicksLambdaErrorsThroughThrownExceptions()
        {
            var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => { throw new InvalidOperationException("oops"); });
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExecuteReenablesExecutionEvenAfterFailure()
        {
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
            var canExecute = fixture
                .CanExecute
                .CreateCollection();
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);

            Assert.Equal(3, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
            Assert.True(canExecute[2]);
        }

        [Fact]
        public void CreateTaskFacilitatesTPLIntegration()
        {
            var fixture = ReactiveCommand.CreateFromTask(() => Task.FromResult(13));
            var results = fixture
                .CreateCollection();

            fixture.Execute().Subscribe();

            Assert.Equal(1, results.Count);
            Assert.Equal(13, results[0]);
        }

        [Fact]
        public void CreateTaskFacilitatesTPLIntegrationWithParameter()
        {
            var fixture = ReactiveCommand.CreateFromTask<int, int>(param => Task.FromResult(param + 1));
            var results = fixture
                .CreateCollection();

            fixture.Execute(3).Subscribe();
            fixture.Execute(41).Subscribe();

            Assert.Equal(2, results.Count);
            Assert.Equal(4, results[0]);
            Assert.Equal(42, results[1]);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandInvokesTheCommand()
        {
            var executionCount = 0;
            var fixture = ReactiveCommand.Create(() => ++executionCount);
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.Equal(1, executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(2, executionCount);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandPassesTheSpecifiedValueToExecute()
        {
            var executeReceived = 0;
            var fixture = ReactiveCommand.Create<int>(x => executeReceived = x);
            var source = new Subject<int>();
            source.InvokeCommand(fixture);

            source.OnNext(42);
            Assert.Equal(42, executeReceived);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandRespectsCanExecute()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = ReactiveCommand.Create(() => executed = true, canExecute);
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(Unit.Default);
            Assert.True(executed);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandRespectsCanExecuteWindow()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = ReactiveCommand.Create(() => executed = true, canExecute);
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandSwallowsExceptions()
        {
            var count = 0;
            var fixture = ReactiveCommand.Create(
                () => {
                    ++count;
                    throw new InvalidOperationException();
                });
            fixture.ThrownExceptions.Subscribe();
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            source.OnNext(Unit.Default);

            Assert.Equal(2, count);
        }

        [Fact]
        public void InvokeCommandAgainstICommandInvokesTheCommand()
        {
            var executionCount = 0;
            var fixture = (ICommand)ReactiveCommand.Create(() => ++executionCount);
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.Equal(1, executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(2, executionCount);
        }

        [Fact]
        public void InvokeCommandAgainstICommandPassesTheSpecifiedValueToCanExecuteAndExecute()
        {
            var fixture = new FakeCommand();
            var source = new Subject<int>();
            source.InvokeCommand(fixture);

            source.OnNext(42);
            Assert.Equal(42, fixture.CanExecuteParameter);
            Assert.Equal(42, fixture.ExecuteParameter);
        }

        [Fact]
        public void InvokeCommandAgainstICommandRespectsCanExecute()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = (ICommand)ReactiveCommand.Create(() => executed = true, canExecute);
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(Unit.Default);
            Assert.True(executed);
        }

        [Fact]
        public void InvokeCommandAgainstICommandRespectsCanExecuteWindow()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = (ICommand)ReactiveCommand.Create(() => executed = true, canExecute);
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        [Fact]
        public void InvokeCommandAgainstICommandSwallowsExceptions()
        {
            var count = 0;
            var fixture = ReactiveCommand.Create(
                () => {
                    ++count;
                    throw new InvalidOperationException();
                });
            fixture.ThrownExceptions.Subscribe();
            var source = new Subject<Unit>();
            source.InvokeCommand((ICommand)fixture);

            source.OnNext(Unit.Default);
            source.OnNext(Unit.Default);

            Assert.Equal(2, count);
        }

        [Fact]
        public void InvokeCommandAgainstICommandInTargetInvokesTheCommand()
        {
            var executionCount = 0;
            var fixture = new ICommandHolder();
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create(() => ++executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(1, executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(2, executionCount);
        }

        [Fact]
        public void InvokeCommandAgainstICommandInTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
        {
            var fixture = new ICommandHolder();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            var command = new FakeCommand();
            fixture.TheCommand = command;

            source.OnNext(42);
            Assert.Equal(42, command.CanExecuteParameter);
            Assert.Equal(42, command.ExecuteParameter);
        }

        [Fact]
        public void InvokeCommandAgainstICommandInTargetRespectsCanExecute()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ICommandHolder();
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(Unit.Default);
            Assert.True(executed);
        }

        [Fact]
        public void InvokeCommandAgainstICommandInTargetRespectsCanExecuteWindow()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ICommandHolder();
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        [Fact]
        public void InvokeCommandAgainstICommandInTargetSwallowsExceptions()
        {
            var count = 0;
            var fixture = new ICommandHolder();
            var command = ReactiveCommand.Create(
                () => {
                    ++count;
                    throw new InvalidOperationException();
                });
            command.ThrownExceptions.Subscribe();
            fixture.TheCommand = command;
            var source = new Subject<Unit>();
            source.InvokeCommand(fixture, x => x.TheCommand);

            source.OnNext(Unit.Default);
            source.OnNext(Unit.Default);

            Assert.Equal(2, count);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetInvokesTheCommand()
        {
            var executionCount = 0;
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create<int>(_ => { ++executionCount; });

            source.OnNext(0);
            Assert.Equal(1, executionCount);

            source.OnNext(0);
            Assert.Equal(2, executionCount);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetPassesTheSpecifiedValueToExecute()
        {
            var executeReceived = 0;
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create<int>(x => executeReceived = x);

            source.OnNext(42);
            Assert.Equal(42, executeReceived);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecute()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create<int>(_ => executed = true, canExecute);

            source.OnNext(0);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(0);
            Assert.True(executed);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecuteWindow()
        {
            var executed = false;
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create<int>(_ => executed = true, canExecute);

            source.OnNext(0);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetSwallowsExceptions()
        {
            var count = 0;
            var fixture = new ReactiveCommandHolder();
            fixture.TheCommand = ReactiveCommand.Create<int>(
                _ => {
                    ++count;
                    throw new InvalidOperationException();
                });
            fixture.TheCommand.ThrownExceptions.Subscribe();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x.TheCommand);

            source.OnNext(0);
            source.OnNext(0);

            Assert.Equal(2, count);
        }

        private class FakeCommand : ICommand
        {
            public object CanExecuteParameter
            {
                get;
                private set;
            }

            public object ExecuteParameter
            {
                get;
                private set;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                this.CanExecuteParameter = parameter;
                return true;
            }

            public void Execute(object parameter)
            {
                this.ExecuteParameter = parameter;
            }
        }

        private class ICommandHolder : ReactiveObject
        {
            private ICommand theCommand;

            public ICommand TheCommand
            {
                get { return this.theCommand; }
                set { this.RaiseAndSetIfChanged(ref this.theCommand, value); }
            }
        }

        private class ReactiveCommandHolder : ReactiveObject
        {
            private ReactiveCommand<int, Unit> theCommand;

            public ReactiveCommand<int, Unit> TheCommand
            {
                get { return this.theCommand; }
                set { this.RaiseAndSetIfChanged(ref this.theCommand, value); }
            }
        }
    }

    public class CombinedReactiveCommandTest
    {
        [Fact]
        public void CanExecuteIsFalseIfAnyChildCannotExecute()
        {
            var child1 = ReactiveCommand.Create(() => Observables.Unit);
            var child2 = ReactiveCommand.Create(() => Observables.Unit, Observables.False);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            Assert.Equal(1, canExecute.Count);
            Assert.False(canExecute[0]);
        }

        [Fact]
        public void CanExecuteIsFalseIfParentCanExecuteIsFalse()
        {
            var child1 = ReactiveCommand.Create(() => Observables.Unit);
            var child2 = ReactiveCommand.Create(() => Observables.Unit);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, Observables.False);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            Assert.Equal(1, canExecute.Count);
            Assert.False(canExecute[0]);
        }

        [Fact]
        public void CanExecuteTicksFailuresThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var child1 = ReactiveCommand.Create(() => Observables.Unit);
            var child2 = ReactiveCommand.Create(() => Observables.Unit);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, canExecuteSubject);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void CanExecuteTicksFailuresInChildCanExecuteThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var child1 = ReactiveCommand.Create(() => Observables.Unit);
            var child2 = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExecuteExecutesAllChildCommands()
        {
            var child1 = ReactiveCommand.Create(() => Observables.Unit);
            var child2 = ReactiveCommand.Create(() => Observables.Unit);
            var child3 = ReactiveCommand.Create(() => Observables.Unit);
            var childCommands = new[] { child1, child2, child3 };
            var fixture = ReactiveCommand.CreateCombined(childCommands);

            var isExecuting = fixture
                .IsExecuting
                .CreateCollection();
            var child1IsExecuting = child1
                .IsExecuting
                .CreateCollection();
            var child2IsExecuting = child2
                .IsExecuting
                .CreateCollection();
            var child3IsExecuting = child3
                .IsExecuting
                .CreateCollection();

            fixture.Execute().Subscribe();

            Assert.Equal(3, isExecuting.Count);
            Assert.False(isExecuting[0]);
            Assert.True(isExecuting[1]);
            Assert.False(isExecuting[2]);

            Assert.Equal(3, child1IsExecuting.Count);
            Assert.False(child1IsExecuting[0]);
            Assert.True(child1IsExecuting[1]);
            Assert.False(child1IsExecuting[2]);

            Assert.Equal(3, child2IsExecuting.Count);
            Assert.False(child2IsExecuting[0]);
            Assert.True(child2IsExecuting[1]);
            Assert.False(child2IsExecuting[2]);

            Assert.Equal(3, child3IsExecuting.Count);
            Assert.False(child3IsExecuting[0]);
            Assert.True(child3IsExecuting[1]);
            Assert.False(child3IsExecuting[2]);
        }

        [Fact]
        public void ExecuteTicksThroughTheResults()
        {
            var child1 = ReactiveCommand.CreateFromObservable(() => Observable.Return(1));
            var child2 = ReactiveCommand.CreateFromObservable(() => Observable.Return(2));
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands);

            var results = fixture
                .CreateCollection();

            fixture.Execute().Subscribe();

            Assert.Equal(1, results.Count);
            Assert.Equal(2, results[0].Count);
            Assert.Equal(1, results[0][0]);
            Assert.Equal(2, results[0][1]);
        }

        [Fact]
        public void ResultIsTickedThroughSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched => {
                var child1 = ReactiveCommand.Create(() => Observable.Return(1));
                var child2 = ReactiveCommand.Create(() => Observable.Return(2));
                var childCommands = new[] { child1, child2 };
                var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: sched);
                var results = fixture
                    .CreateCollection();

                fixture.Execute().Subscribe();
                Assert.Empty(results);

                sched.AdvanceByMs(1);
                Assert.Equal(1, results.Count);
            });
        }

        [Fact]
        public void ExecuteTicksErrorsInAnyChildCommandThroughThrownExceptions()
        {
            var child1 = ReactiveCommand.CreateFromObservable(() => Observables.Unit);
            var child2 = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture
                .Execute()
                .Subscribe(
                    _ => { },
                    _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExceptionsAreDeliveredOnOutputScheduler()
        {
            (new TestScheduler()).With(sched => {
                var child = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
                var childCommands = new[] { child };
                var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: sched);
                Exception exception = null;
                fixture
                    .ThrownExceptions
                    .Subscribe(ex => exception = ex);
                fixture
                   .Execute()
                   .Subscribe(
                       _ => { },
                       _ => { });

                Assert.Null(exception);
                sched.Start();
                Assert.IsType<InvalidOperationException>(exception);
            });
        }
    }
}