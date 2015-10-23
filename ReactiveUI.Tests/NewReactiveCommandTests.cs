using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class SynchronousReactiveCommandTests
    {
        [Fact]
        public void ConstructorThrowsIfExecuteIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => NewReactiveCommand.CreateSynchronous<Unit>(null));
            Assert.Throws<ArgumentNullException>(() => NewReactiveCommand.CreateSynchronous<Unit, Unit>(null));
        }

        [Fact]
        public void CanExecuteIsBehavioral()
        {
            var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default);
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
            var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default, canExecuteSubject);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(true);

            Assert.Equal(3, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
            Assert.True(canExecute[2]);
        }

        [Fact]
        public void CanExecuteIsFalseIfCallerDictatesAsSuch()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default, canExecuteSubject);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
        }
        
        [Fact]
        public void CanExecuteTicksFailuresThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default, canExecuteSubject);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void IsExecutingIsBehavioral()
        {
            var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default);
            var isExecuting = fixture
                .IsExecuting
                .CreateCollection();

            Assert.Equal(1, isExecuting.Count);
            Assert.False(isExecuting[0]);
        }

        [Fact]
        public void IsExecutingIsCorrectForLateSubscriber()
        {
            using (var inFlightHandle = new ManualResetEventSlim())
            using (var waitHandle = new ManualResetEventSlim())
            {
                var fixture = NewReactiveCommand.CreateSynchronous(
                    () =>
                    {
                        inFlightHandle.Set();
                        waitHandle.Wait(TimeSpan.FromSeconds(3));
                        return Unit.Default;
                    });

                TaskPoolScheduler.Default.Schedule(() => fixture.Execute());
                inFlightHandle.Wait(TimeSpan.FromSeconds(3));
                var isExecuting = fixture
                    .IsExecuting
                    .CreateCollection();

                Assert.Equal(1, isExecuting.Count);
                Assert.True(isExecuting[0]);
            }
        }

        [Fact]
        public void IsExecutingTicksBeforeAndAfterExecution()
        {
            var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default);
            var isExecuting = fixture
                .IsExecuting
                .CreateCollection();

            fixture.Execute();

            Assert.Equal(3, isExecuting.Count);
            Assert.False(isExecuting[0]);
            Assert.True(isExecuting[1]);
            Assert.False(isExecuting[2]);
        }

        [Fact]
        public void ExecutePassesThroughParameter()
        {
            var parameters = new List<int>();
            var fixture = NewReactiveCommand.CreateSynchronous<int, Unit>(
                param =>
                {
                    parameters.Add(param);
                    return Unit.Default;
                });

            fixture.Execute(1);
            fixture.Execute(42);
            fixture.Execute(348);

            Assert.Equal(3, parameters.Count);
            Assert.Equal(1, parameters[0]);
            Assert.Equal(42, parameters[1]);
            Assert.Equal(348, parameters[2]);
        }

        [Fact]
        public void ExecuteFailsIfCanExecuteIsFalse()
        {
            var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default, Observable.Return(false));
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.Execute();

            Assert.Equal(1, thrownExceptions.Count);
            Assert.IsType<InvalidOperationException>(thrownExceptions[0]);
            Assert.Equal("Command cannot currently execute.", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExecuteTicksThroughTheResult()
        {
            var num = 0;
            var fixture = NewReactiveCommand.CreateSynchronous(() => num);
            var results = fixture
                .CreateCollection();

            num = 1;
            fixture.Execute();
            num = 10;
            fixture.Execute();
            num = 30;
            fixture.Execute();

            Assert.Equal(3, results.Count);
            Assert.Equal(1, results[0]);
            Assert.Equal(10, results[1]);
            Assert.Equal(30, results[2]);
        }

        [Fact]
        public void ResultIsTickedThroughSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched =>
            {
                var fixture = NewReactiveCommand.CreateSynchronous(() => Unit.Default, scheduler: sched);
                var results = fixture
                    .CreateCollection();

                fixture.Execute();
                Assert.Empty(results);

                sched.AdvanceByMs(1);
                Assert.Equal(1, results.Count);
            });
        }

        [Fact]
        public void ExecuteTicksErrorsThroughThrownExceptions()
        {
            var fixture = NewReactiveCommand.CreateSynchronous<Unit>(() => { throw new InvalidOperationException("oops"); });
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.Execute();

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void ExecuteReenablesExecutionEvenAfterFailure()
        {
            var fixture = NewReactiveCommand.CreateSynchronous<Unit>(() => { throw new InvalidOperationException("oops"); });
            var canExecute = fixture
                .CanExecute
                .CreateCollection();
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.Execute();

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);

            Assert.Equal(3, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
            Assert.True(canExecute[2]);
        }
    }

    public class AsynchronousReactiveCommandTests
    {
        [Fact]
        public void ConstructorThrowsIfExecuteAsyncIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => NewReactiveCommand.CreateAsynchronous<Unit>(null));
            Assert.Throws<ArgumentNullException>(() => NewReactiveCommand.CreateAsynchronous<Unit, Unit>(null));
        }

        [Fact]
        public void ConstructorThrowsIfMaxInFlightExecutionsIsInvalid()
        {
            Assert.Throws<ArgumentException>(() => NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default), maxInFlightExecutions: 0));
            Assert.Throws<ArgumentException>(() => NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default), maxInFlightExecutions: -1));
            Assert.Throws<ArgumentException>(() => NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default), maxInFlightExecutions: -21));
        }

        [Fact]
        public void CanExecuteIsBehavioral()
        {
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default));
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
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default), canExecuteSubject);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(true);

            Assert.Equal(3, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
            Assert.True(canExecute[2]);
        }

        [Fact]
        public void CanExecuteIsFalseIfCallerDictatesAsSuch()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default), canExecuteSubject);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
        }

        [Fact]
        public void CanExecuteIsFalseIfMaxInFlightExecutionsIsReached()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.CreateAsynchronous(() => execute, scheduler: sched, maxInFlightExecutions: 2);
                var canExecute = fixture
                    .CanExecute
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Equal(1, canExecute.Count);
                Assert.True(canExecute[0]);

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Equal(2, canExecute.Count);
                Assert.False(canExecute[1]);

                sched.AdvanceByMs(900);

                Assert.Equal(3, canExecute.Count);
                Assert.True(canExecute[2]);
            });
        }

        [Fact]
        public void CanExecuteTicksFailuresThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default), canExecuteSubject);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        [Fact]
        public void MaxInFlightExecutionsDefaultsToOne()
        {
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default));
            Assert.Equal(1, fixture.MaxInFlightExecutions);
        }

        [Fact]
        public void InFlightExecutionsIsBehavioral()
        {
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default));
            var inFlightExecutions = fixture
                .InFlightExecutions
                .CreateCollection();

            Assert.Equal(1, inFlightExecutions.Count);
            Assert.Equal(0, inFlightExecutions[0]);
        }

        [Fact]
        public void InFlightExecutionsTicksAsExecutionsStartAndEnd()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.CreateAsynchronous(() => execute, scheduler: sched, maxInFlightExecutions: 2);
                var inFlightExecutions = fixture
                    .InFlightExecutions
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Equal(2, inFlightExecutions.Count);
                Assert.Equal(0, inFlightExecutions[0]);
                Assert.Equal(1, inFlightExecutions[1]);

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Equal(3, inFlightExecutions.Count);
                Assert.Equal(2, inFlightExecutions[2]);

                sched.AdvanceByMs(900);

                Assert.Equal(4, inFlightExecutions.Count);
                Assert.Equal(1, inFlightExecutions[3]);

                sched.AdvanceByMs(900);

                Assert.Equal(5, inFlightExecutions.Count);
                Assert.Equal(0, inFlightExecutions[4]);
            });
        }

        [Fact]
        public void IsExecutingIsBehavioral()
        {
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(Unit.Default));
            var isExecuting = fixture
                .IsExecuting
                .CreateCollection();

            Assert.Equal(1, isExecuting.Count);
            Assert.False(isExecuting[0]);
        }

        [Fact]
        public void IsExecutingTicksAsExecutionsProgress()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.CreateAsynchronous(() => execute, scheduler: sched, maxInFlightExecutions: 2);
                var isExecuting = fixture
                    .IsExecuting
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Equal(2, isExecuting.Count);
                Assert.False(isExecuting[0]);
                Assert.True(isExecuting[1]);

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Equal(2, isExecuting.Count);

                sched.AdvanceByMs(900);

                Assert.Equal(2, isExecuting.Count);

                sched.AdvanceByMs(900);

                Assert.Equal(3, isExecuting.Count);
                Assert.False(isExecuting[2]);
            });
        }

        [Fact]
        public void ExecuteAsyncPassesThroughParameter()
        {
            var parameters = new List<int>();
            var fixture = NewReactiveCommand.CreateAsynchronous<int, Unit>(
                param =>
                {
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
        public void ExecuteAsyncFailsIfCanExecuteIsFalse()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default);
                var fixture = NewReactiveCommand.CreateAsynchronous(() => execute, Observable.Return(false), sched);
                var thrownExceptions = fixture
                    .ThrownExceptions
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(1);

                Assert.Equal(1, thrownExceptions.Count);
                Assert.IsType<InvalidOperationException>(thrownExceptions[0]);
                Assert.Equal("Command cannot currently execute.", thrownExceptions[0].Message);
            });
        }

        [Fact]
        public void ExecuteAsyncFailsIfThereAreTooManyExecutionsInFlight()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.CreateAsynchronous(() => execute, scheduler: sched, maxInFlightExecutions: 2);
                var thrownExceptions = fixture
                    .ThrownExceptions
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Empty(thrownExceptions);

                fixture.ExecuteAsync();
                sched.AdvanceByMs(1);

                Assert.Equal(1, thrownExceptions.Count);
                Assert.IsType<InvalidOperationException>(thrownExceptions[0]);
                Assert.Equal("No more executions can be performed because the maximum number of in-flight executions (2) has been reached.", thrownExceptions[0].Message);
            });
        }

        [Fact]
        public void ExecuteAsyncExecutesOnTheSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.CreateAsynchronous(() => execute, scheduler: sched);
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
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.CreateAsynchronous(() => execute, scheduler: sched);
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
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Return(num));
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
        public void ExecuteAsyncTicksErrorsThroughThrownExceptions()
        {
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
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
            var fixture = NewReactiveCommand.CreateAsynchronous(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
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
    }
}