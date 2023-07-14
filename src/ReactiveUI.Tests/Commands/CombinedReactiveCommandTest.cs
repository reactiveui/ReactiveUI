// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests for the ReactiveCommand Combined functionality.
    /// </summary>
    public class CombinedReactiveCommandTest
    {
        /// <summary>
        /// Tests that determines whether this instance [can execute is false if any child cannot execute].
        /// </summary>
        [Fact]
        public void CanExecuteIsFalseIfAnyChildCannotExecute()
        {
            var child1 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var child2 = ReactiveCommand.Create(() => Observables.Unit, Observables.False, ImmediateScheduler.Instance);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);
            fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

            Assert.Equal(1, canExecute.Count);
            Assert.False(canExecute[0]);
        }

        /// <summary>
        /// Test that determines whether this instance [can execute is false if parent can execute is false].
        /// </summary>
        [Fact]
        public void CanExecuteIsFalseIfParentCanExecuteIsFalse()
        {
            var child1 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var child2 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, Observables.False, ImmediateScheduler.Instance);
            fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

            Assert.Equal(1, canExecute.Count);
            Assert.False(canExecute[0]);
        }

        /// <summary>
        /// Test that determines whether this instance [can execute ticks failures in child can execute through thrown exceptions].
        /// </summary>
        [Fact]
        public void CanExecuteTicksFailuresInChildCanExecuteThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var child1 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var child2 = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        /// <summary>
        /// Test that determines whether this instance [can execute ticks failures through thrown exceptions].
        /// </summary>
        [Fact]
        public void CanExecuteTicksFailuresThroughThrownExceptions()
        {
            var canExecuteSubject = new Subject<bool>();
            var child1 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var child2 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, canExecuteSubject, ImmediateScheduler.Instance);
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        /// <summary>
        /// A test that checks that all the exceptions that were delivered through the output scheduler.
        /// </summary>
        [Fact]
        public void ExceptionsAreDeliveredOnOutputScheduler() =>
            new TestScheduler().With(
                scheduler =>
                {
                    var child = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
                    var childCommands = new[] { child };
                    var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: scheduler);
                    Exception? exception = null;
                    fixture.ThrownExceptions.Subscribe(ex => exception = ex);
                    fixture.Execute().Subscribe(_ => { }, _ => { });
                    Assert.Null(exception);
                    scheduler.Start();
                    Assert.IsType<InvalidOperationException>(exception);
                });

        /// <summary>
        /// A test that executes the executes all child commands.
        /// </summary>
        [Fact]
        public void ExecuteExecutesAllChildCommands()
        {
            var child1 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var child2 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var child3 = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var childCommands = new[] { child1, child2, child3 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);

            fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();
            child1.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var child1IsExecuting).Subscribe();
            child2.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var child2IsExecuting).Subscribe();
            child3.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var child3IsExecuting).Subscribe();

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

        /// <summary>
        /// Test that executes the ticks errors in any child command through thrown exceptions.
        /// </summary>
        [Fact]
        public void ExecuteTicksErrorsInAnyChildCommandThroughThrownExceptions()
        {
            var child1 = ReactiveCommand.CreateFromObservable(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            var child2 = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

            fixture.Execute().Subscribe(_ => { }, _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        /// <summary>
        /// Test that executes the ticks through the results.
        /// </summary>
        [Fact]
        public void ExecuteTicksThroughTheResults()
        {
            var child1 = ReactiveCommand.CreateFromObservable(() => Observable.Return(1), outputScheduler: ImmediateScheduler.Instance);
            var child2 = ReactiveCommand.CreateFromObservable(() => Observable.Return(2), outputScheduler: ImmediateScheduler.Instance);
            var childCommands = new[] { child1, child2 };
            var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);

            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

            fixture.Execute().Subscribe();

            Assert.Equal(1, results.Count);
            Assert.Equal(2, results[0].Count);
            Assert.Equal(1, results[0][0]);
            Assert.Equal(2, results[0][1]);
        }

        /// <summary>
        /// Test that checks that results is ticked through specified scheduler.
        /// </summary>
        [Fact]
        public void ResultIsTickedThroughSpecifiedScheduler() =>
            new TestScheduler().WithAsync(
                scheduler =>
                {
                    // Allow scheduler to run freely
                    var child1 = ReactiveCommand.Create(() => Observable.Return(1));
                    var child2 = ReactiveCommand.CreateRunInBackground(() => Observable.Return(2));
                    var childCommands = new[] { child1, child2 };
                    var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: scheduler);
                    fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

                    fixture.Execute().Subscribe();
                    Assert.Empty(results);

                    scheduler.AdvanceByMs(1);
                    Assert.Equal(1, results.Count);
                    return Task.CompletedTask;
                });
    }
}
