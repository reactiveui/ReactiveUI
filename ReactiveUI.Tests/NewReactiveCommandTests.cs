using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class NewReactiveCommandTests
    {
        [Fact]
        public void ConstructorThrowsIfExecuteAsyncIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => NewReactiveCommand.Create<Unit>(null));
            Assert.Throws<ArgumentNullException>(() => NewReactiveCommand.Create<Unit, Unit>(null));
        }

        [Fact]
        public void CanExecuteIsBehavioral()
        {
            var fixture = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
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
            var fixture = NewReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
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
            var fixture = NewReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
        }

        [Fact]
        public void CanExecuteIsFalseIfAlreadyExecuting()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.Create(() => execute, scheduler: sched);
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
            var fixture = NewReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
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
            var fixture = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
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
                var fixture = NewReactiveCommand.Create(() => execute, scheduler: sched);
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
            var fixture = NewReactiveCommand.Create<int, Unit>(
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
                var fixture = NewReactiveCommand.Create(() => execute, Observable.Return(false), sched);
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
        public void ExecuteAsyncFailsIfAlreadyExecuting()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.Create(() => execute, scheduler: sched);
                var thrownExceptions = fixture
                    .ThrownExceptions
                    .CreateCollection();

                fixture.ExecuteAsync();
                sched.AdvanceByMs(100);

                Assert.Empty(thrownExceptions);

                fixture.ExecuteAsync();
                sched.AdvanceByMs(1);

                Assert.Equal(1, thrownExceptions.Count);
                Assert.IsType<InvalidOperationException>(thrownExceptions[0]);
                Assert.Equal("Command cannot currently execute.", thrownExceptions[0].Message);
            });
        }

        [Fact]
        public void ExecuteAsyncExecutesOnTheSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched =>
            {
                var execute = Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(1), sched);
                var fixture = NewReactiveCommand.Create(() => execute, scheduler: sched);
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
                var fixture = NewReactiveCommand.Create(() => execute, scheduler: sched);
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
            var fixture = NewReactiveCommand.Create(() => Observable.Return(num));
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
        public void ResultIsTickedThroughSpecifiedScheduler()
        {
            (new TestScheduler()).With(sched =>
            {
                var fixture = NewReactiveCommand.Create(() => Observable.Return(Unit.Default), scheduler: sched);
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
            var fixture = NewReactiveCommand.Create(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
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
            var fixture = NewReactiveCommand.Create(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
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

    public class CombinedReactiveCommandTests
    {
        [Fact]
        public void CanExecuteIsFalseIfAnyChildCannotExecute()
        {
            var child1 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default), Observable.Return(false));
            var childCommands = new[] { child1, child2 };
            var fixture = NewReactiveCommand.CreateCombined(childCommands);
            var canExecute = fixture
                .CanExecute
                .CreateCollection();

            Assert.Equal(1, canExecute.Count);
            Assert.False(canExecute[0]);
        }

        [Fact]
        public void CanExecuteIsFalseIfParentCanExecuteIsFalse()
        {
            var child1 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var childCommands = new[] { child1, child2 };
            var fixture = NewReactiveCommand.CreateCombined(childCommands, Observable.Return(false));
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
            var child1 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var childCommands = new[] { child1, child2 };
            var fixture = NewReactiveCommand.CreateCombined(childCommands, canExecuteSubject);
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
            var child1 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default), canExecuteSubject);
            var childCommands = new[] { child1, child2 };
            var fixture = NewReactiveCommand.CreateCombined(childCommands);
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
            var child1 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child3 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var childCommands = new[] { child1, child2, child3 };
            var fixture = NewReactiveCommand.CreateCombined(childCommands);

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
            var child1 = NewReactiveCommand.Create(() => Observable.Return(1));
            var child2 = NewReactiveCommand.Create(() => Observable.Return(2));
            var childCommands = new[] { child1, child2 };
            var fixture = NewReactiveCommand.CreateCombined(childCommands);

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
            (new TestScheduler()).With(sched =>
            {
                var child1 = NewReactiveCommand.Create(() => Observable.Return(1));
                var child2 = NewReactiveCommand.Create(() => Observable.Return(2));
                var childCommands = new[] { child1, child2 };
                var fixture = NewReactiveCommand.CreateCombined(childCommands, scheduler: sched);
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
            var child1 = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var child2 = NewReactiveCommand.Create(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
            var childCommands = new[] { child1, child2 };
            var fixture = NewReactiveCommand.CreateCombined(childCommands);
            var thrownExceptions = fixture
                .ThrownExceptions
                .CreateCollection();

            fixture.ExecuteAsync();

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }
    }

    public class ReactiveCommandMixinsTests
    {
        [Fact]
        public void ToPlatformThrowsIfCommandIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((NewReactiveCommand<Unit, Unit>)null).ToPlatform());
            Assert.Throws<ArgumentNullException>(() => ((CombinedReactiveCommand<Unit, Unit>)null).ToPlatform());
        }

        [Fact]
        public void ToPlatformAdaptsExecute()
        {
            var value = 0;
            var command = NewReactiveCommand.Create<int, Unit>(
                param =>
                {
                    value = param;
                    return Observable.Return(Unit.Default);
                });
            var fixture = command.ToPlatform();

            fixture.Execute(5);
            Assert.Equal(5, value);

            // null will convert to default(TParam)
            fixture.Execute(null);
            Assert.Equal(0, value);
        }

        [Fact]
        public void ToPlatformAdaptsCanExecute()
        {
            var canExecuteSubject = new Subject<bool>();
            var command = NewReactiveCommand.Create<Unit>(() => Observable.Return(Unit.Default), canExecuteSubject);
            var fixture = command.ToPlatform();
            var canExecuteChanged = Observable
                .FromEventPattern(fixture, nameof(fixture.CanExecuteChanged))
                .Select(_ => Unit.Default)
                .CreateCollection();

            Assert.True(fixture.CanExecute(null));
            Assert.Empty(canExecuteChanged);

            canExecuteSubject.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteChanged.Count);

            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteChanged.Count);

            canExecuteSubject.OnNext(true);
            Assert.True(fixture.CanExecute(null));
            Assert.Equal(2, canExecuteChanged.Count);
        }

        [Fact]
        public void ToPlatformAdaptsCombinedExecute()
        {
            var value = 0;
            var child = NewReactiveCommand.Create<int, Unit>(
                param =>
                {
                    value = param;
                    return Observable.Return(Unit.Default);
                });
            var command = NewReactiveCommand.CreateCombined(new[]{ child });
            var fixture = command.ToPlatform();

            fixture.Execute(5);
            Assert.Equal(5, value);

            // null will convert to default(TParam)
            fixture.Execute(null);
            Assert.Equal(0, value);
        }

        [Fact]
        public void ToPlatformAdaptsCombinedCanExecute()
        {
            var canExecuteSubject = new Subject<bool>();
            var child = NewReactiveCommand.Create(() => Observable.Return(Unit.Default));
            var command = NewReactiveCommand.CreateCombined(new[] { child }, canExecuteSubject);
            var fixture = command.ToPlatform();
            var canExecuteChanged = Observable
                .FromEventPattern(fixture, nameof(fixture.CanExecuteChanged))
                .Select(_ => Unit.Default)
                .CreateCollection();

            Assert.True(fixture.CanExecute(null));
            Assert.Empty(canExecuteChanged);

            canExecuteSubject.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteChanged.Count);

            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            canExecuteSubject.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteChanged.Count);

            canExecuteSubject.OnNext(true);
            Assert.True(fixture.CanExecute(null));
            Assert.Equal(2, canExecuteChanged.Count);
        }
    }
}