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
            var fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
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
            var fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
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
            var fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
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
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var canExecute = fixture
                    .CanExecute
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Equal(2, canExecute.Count);
                Assert.False(canExecute[1]);

                sched.AdvanceByMs(901);

                Assert.Equal(3, canExecute.Count);
                Assert.True(canExecute[2]);
            });
        }

        [Fact]
        public void CanExecuteTicksFailuresThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
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
            ICommand fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);

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
            ICommand fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
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
            var fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
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
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var isExecuting = fixture
                    .IsExecuting
                    .CreateCollection();

                fixture.ExecuteAsync();
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
        public void ExecuteAsyncPassesThroughParameter()
        {
            var parameters = new List<int>();
            var fixture = ReactiveCommand.CreateFromObservable<int, Unit>(param => {
                    parameters.Add(param);
                    return Observable.Return(Unit.Default);
                });

            fixture.ExecuteAsync(1);
            fixture.ExecuteAsync(42);
            fixture.ExecuteAsync(348);

            Assert.Equal(3, parameters.Count);
            Assert.Equal(1, parameters[0]);
            Assert.Equal(42, parameters[1]);
            Assert.Equal(348, parameters[2]);
        }

        [Fact]
        public void ExecuteAsyncExecutesOnTheSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched => {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var isExecuting = fixture
                    .IsExecuting
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(999);

                Assert.Equal(2, isExecuting.Count);
                Assert.False(isExecuting[0]);
                Assert.True(isExecuting[1]);

                sched.AdvanceByMs(2);

                Assert.Equal(3, isExecuting.Count);
                Assert.False(isExecuting[2]);
            });
        }

        [Fact]
        public void ExecuteAsyncExecutesEvenWithoutASubscription()
        {
            (new TestScheduler()).With(sched => {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: sched);
                var isExecuting = fixture
                    .IsExecuting
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(1);

                Assert.Equal(2, isExecuting.Count);
                Assert.False(isExecuting[0]);
                Assert.True(isExecuting[1]);
            });
        }

        [Fact]
        public void ExecuteAsyncTicksThroughTheResult()
        {
            var num = 0;
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Return(num));
            var results = fixture
                .CreateCollection();

            num = 1;
            fixture.ExecuteAsync();
            num = 10;
            fixture.ExecuteAsync();
            num = 30;
            fixture.ExecuteAsync();

            Assert.Equal(3, results.Count);
            Assert.Equal(1, results[0]);
            Assert.Equal(10, results[1]);
            Assert.Equal(30, results[2]);
        }

        [Fact]
        public void ExecuteAsyncTicksAnyException()
        {
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()));
            fixture
                .ThrownExceptions
                .Subscribe();
            Exception exception = null;
            fixture
                .ExecuteAsync()
                .Subscribe(
                    _ => { },
                    ex => exception = ex,
                    () => { });

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void ExecuteAsyncTicksAnyLambdaException()
        {
            var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => { throw new InvalidOperationException(); });
            fixture
                .ThrownExceptions
                .Subscribe();
            Exception exception = null;
            fixture
                .ExecuteAsync()
                .Subscribe(
                    _ => { },
                    ex => exception = ex,
                    () => { });

            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public void ExecuteIsAvailableViaICommand()
        {
            var executed = false;
            ICommand fixture = ReactiveCommand.Create(() => {
                    executed = true;
                    return Observable.Return(Unit.Default);
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
                var fixture = ReactiveCommand.Create(() => Observable.Return(Unit.Default), outputScheduler: sched);
                var results = fixture
                    .CreateCollection();

                fixture.ExecuteAsync();
                Assert.Empty(results);

                sched.AdvanceByMs(1);
                Assert.Equal(1, results.Count);
            });
        }

        [Fact]
        public void ExecuteAsyncTicksErrorsThroughThrownExceptions()
        {
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.ExecuteAsync();

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExecuteAsyncTicksLambdaErrorsThroughThrownExceptions()
        {
            var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => { throw new InvalidOperationException("oops"); });
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.ExecuteAsync();

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExecuteAsyncReenablesExecutionEvenAfterFailure()
        {
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
            var canExecute = fixture
                .CanExecute
                .CreateCollection();
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.ExecuteAsync();

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

            fixture.ExecuteAsync();

            Assert.Equal(1, results.Count);
            Assert.Equal(13, results[0]);
        }

        [Fact]
        public void CreateTaskFacilitatesTPLIntegrationWithParameter()
        {
            var fixture = ReactiveCommand.CreateFromTask<int, int>(param => Task.FromResult(param + 1));
            var results = fixture
                .CreateCollection();

            fixture.ExecuteAsync(3);
            fixture.ExecuteAsync(41);

            Assert.Equal(2, results.Count);
            Assert.Equal(4, results[0]);
            Assert.Equal(42, results[1]);
        }

        [Fact]
        public void InvokeCommandInvokesTheCommand()
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
        public void InvokeCommandRespectsCanExecute()
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
    }

    public class CombinedReactiveCommandTest
    {
        [Fact]
        public void CanExecuteIsFalseIfAnyChildCannotExecute()
        {
            var child1 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = ReactiveCommand.Create(() => Observable.Return(Unit.Default), Observable.Return(false));
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
            var child1 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, Observable.Return(false));
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
            var child1 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
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
            var child1 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = ReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
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
        public void ExecuteAsyncExecutesAllChildCommands()
        {
            var child1 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child3 = ReactiveCommand.Create(() => Observable.Return(Unit.Default));
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

            fixture.ExecuteAsync();

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
        public void ExecuteAsyncTicksThroughTheResults()
        {
            var child1 = ReactiveCommand.CreateFromObservable(() => Observable.Return(1));
            var child2 = ReactiveCommand.CreateFromObservable(() => Observable.Return(2));
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands);

            var results = fixture
                .CreateCollection();

            fixture.ExecuteAsync();

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

                fixture.ExecuteAsync();
                Assert.Empty(results);

                sched.AdvanceByMs(1);
                Assert.Equal(1, results.Count);
            });
        }

        [Fact]
        public void ExecuteAsyncTicksErrorsInAnyChildCommandThroughThrownExceptions()
        {
            var child1 = ReactiveCommand.CreateFromObservable(() => Observable.Return(Unit.Default));
            var child2 = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.ExecuteAsync();

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }
    }
}