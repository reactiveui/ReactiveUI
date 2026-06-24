// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Commands;

/// <summary>Tests for the ReactiveCommand Combined functionality.</summary>
public class CombinedReactiveCommandTest
{
    /// <summary>The number of execution emissions expected from a combined command run.</summary>
    private const int ExpectedExecutionEmissions = 3;

    /// <summary>The result value produced by the second child command.</summary>
    private const int SecondChildResult = 2;

    /// <summary>The expected number of child command results.</summary>
    private const int ExpectedChildResultCount = 2;

    /// <summary>The index of the second result within an emitted result collection.</summary>
    private const int SecondResultIndex = 2;

    /// <summary>Tests that determines whether this instance [can execute is false if any child cannot execute].</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsFalseIfAnyChildCannotExecute()
    {
        var child1 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            SingleValueObservable.False,
            Sequencer.Immediate);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: Sequencer.Immediate);
        var canExecute = fixture.CanExecute.Collect();

        await Assert.That(canExecute).Count().IsEqualTo(1);
        await Assert.That(canExecute[0]).IsFalse();
    }

    /// <summary>Test that determines whether this instance [can execute is false if parent can execute is false].</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsFalseIfParentCanExecuteIsFalse()
    {
        var child1 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, SingleValueObservable.False, Sequencer.Immediate);
        var canExecute = fixture.CanExecute.Collect();

        await Assert.That(canExecute).Count().IsEqualTo(1);
        await Assert.That(canExecute[0]).IsFalse();
    }

    /// <summary>Test that determines whether this instance [can execute ticks failures in child can execute through thrown exceptions].</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteTicksFailuresInChildCanExecuteThroughThrownExceptions()
    {
        var canExecuteSubject = new Signal<bool>();
        var child1 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            canExecuteSubject,
            Sequencer.Immediate);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: Sequencer.Immediate);
        var thrownExceptions = fixture.ThrownExceptions.Collect();

        canExecuteSubject.OnError(new InvalidOperationException("oops"));

        await Assert.That(thrownExceptions).Count().IsEqualTo(1);
        await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
    }

    /// <summary>Test that determines whether this instance [can execute ticks failures through thrown exceptions].</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteTicksFailuresThroughThrownExceptions()
    {
        var canExecuteSubject = new Signal<bool>();
        var child1 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, canExecuteSubject, Sequencer.Immediate);
        var thrownExceptions = fixture.ThrownExceptions.Collect();

        canExecuteSubject.OnError(new InvalidOperationException("oops"));

        await Assert.That(thrownExceptions).Count().IsEqualTo(1);
        await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
    }

    /// <summary>A test that checks that all the exceptions that were delivered through the output scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ExceptionsAreDeliveredOnOutputScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var child = ReactiveCommand.CreateFromObservable(() =>
            Signal.Fail<RxVoid>(new InvalidOperationException("oops")));
        var childCommands = new[] { child };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: scheduler);
        Exception? exception = null;
        _ = fixture.ThrownExceptions.Subscribe(ex => exception = ex);
        _ = fixture.Execute().Subscribe(_ => { }, _ => { });

        // With ImmediateScheduler, exceptions are delivered immediately
        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>A test that executes the executes all child commands.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteExecutesAllChildCommands()
    {
        var child1 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var child3 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var childCommands = new[] { child1, child2, child3 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: Sequencer.Immediate);

        var isExecuting = fixture.IsExecuting.Collect();
        var child1IsExecuting = child1.IsExecuting.Collect();
        var child2IsExecuting = child2.IsExecuting.Collect();
        var child3IsExecuting = child3.IsExecuting.Collect();

        _ = fixture.Execute().Subscribe();

        await Assert.That(isExecuting).Count().IsEqualTo(ExpectedExecutionEmissions);
        using (Assert.Multiple())
        {
            await Assert.That(isExecuting[0]).IsFalse();
            await Assert.That(isExecuting[1]).IsTrue();
            await Assert.That(isExecuting[SecondResultIndex]).IsFalse();

            await Assert.That(child1IsExecuting).Count().IsEqualTo(ExpectedExecutionEmissions);
        }

        using (Assert.Multiple())
        {
            await Assert.That(child1IsExecuting[0]).IsFalse();
            await Assert.That(child1IsExecuting[1]).IsTrue();
            await Assert.That(child1IsExecuting[SecondResultIndex]).IsFalse();

            await Assert.That(child2IsExecuting).Count().IsEqualTo(ExpectedExecutionEmissions);
        }

        using (Assert.Multiple())
        {
            await Assert.That(child2IsExecuting[0]).IsFalse();
            await Assert.That(child2IsExecuting[1]).IsTrue();
            await Assert.That(child2IsExecuting[SecondResultIndex]).IsFalse();

            await Assert.That(child3IsExecuting).Count().IsEqualTo(ExpectedExecutionEmissions);
        }

        using (Assert.Multiple())
        {
            await Assert.That(child3IsExecuting[0]).IsFalse();
            await Assert.That(child3IsExecuting[1]).IsTrue();
            await Assert.That(child3IsExecuting[SecondResultIndex]).IsFalse();
        }
    }

    /// <summary>Test that executes the ticks errors in any child command through thrown exceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksErrorsInAnyChildCommandThroughThrownExceptions()
    {
        var child1 = ReactiveCommand.CreateFromObservable(
            static () => SingleValueObservable.Void,
            outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.CreateFromObservable(
            static () => Signal.Fail<RxVoid>(new InvalidOperationException("oops")),
            outputScheduler: Sequencer.Immediate);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: Sequencer.Immediate);
        var thrownExceptions = fixture.ThrownExceptions.Collect();

        _ = fixture.Execute().Subscribe(static _ => { }, static _ => { });

        await Assert.That(thrownExceptions).Count().IsEqualTo(1);
        await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
    }

    /// <summary>Test that executes the ticks through the results.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksThroughTheResults()
    {
        var child1 = ReactiveCommand.CreateFromObservable(
            static () => Signal.Emit(1),
            outputScheduler: Sequencer.Immediate);
        var child2 = ReactiveCommand.CreateFromObservable(
            static () => Signal.Emit(SecondChildResult),
            outputScheduler: Sequencer.Immediate);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: Sequencer.Immediate);

        var results = fixture.Collect();

        _ = fixture.Execute().Subscribe();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).Count().IsEqualTo(ExpectedChildResultCount);
        using (Assert.Multiple())
        {
            await Assert.That(results[0][0]).IsEqualTo(1);
            await Assert.That(results[0][1]).IsEqualTo(SecondChildResult);
        }
    }

    /// <summary>Test that checks that results is ticked through specified scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ResultIsTickedThroughSpecifiedScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var child1 = ReactiveCommand.CreateFromObservable(static () => Signal.Emit(1), outputScheduler: scheduler);
        var child2 = ReactiveCommand.CreateFromObservable(static () => Signal.Emit(SecondChildResult), outputScheduler: scheduler);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: scheduler);
        var results = fixture.Collect();

        _ = fixture.Execute().Subscribe();

        // With ImmediateScheduler, results are delivered immediately
        await Assert.That(results).Count().IsEqualTo(1);
    }
}
