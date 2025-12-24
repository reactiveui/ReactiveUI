// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ReactiveCommand Combined functionality.
/// </summary>
public class CombinedReactiveCommandTest
{
    /// <summary>
    /// Tests that determines whether this instance [can execute is false if any child cannot execute].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsFalseIfAnyChildCannotExecute()
    {
        var child1 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var child2 = ReactiveCommand.Create(static () => Observables.Unit, Observables.False, ImmediateScheduler.Instance);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        await Assert.That(canExecute).Count().IsEqualTo(1);
        await Assert.That(canExecute[0]).IsFalse();
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is false if parent can execute is false].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsFalseIfParentCanExecuteIsFalse()
    {
        var child1 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var child2 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, Observables.False, ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        await Assert.That(canExecute).Count().IsEqualTo(1);
        await Assert.That(canExecute[0]).IsFalse();
    }

    /// <summary>
    /// Test that determines whether this instance [can execute ticks failures in child can execute through thrown exceptions].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteTicksFailuresInChildCanExecuteThroughThrownExceptions()
    {
        var canExecuteSubject = new Subject<bool>();
        var child1 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var child2 = ReactiveCommand.Create(static () => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

        canExecuteSubject.OnError(new InvalidOperationException("oops"));

        await Assert.That(thrownExceptions).Count().IsEqualTo(1);
        await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
    }

    /// <summary>
    /// Test that determines whether this instance [can execute ticks failures through thrown exceptions].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteTicksFailuresThroughThrownExceptions()
    {
        var canExecuteSubject = new Subject<bool>();
        var child1 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var child2 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, canExecuteSubject, ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

        canExecuteSubject.OnError(new InvalidOperationException("oops"));

        await Assert.That(thrownExceptions).Count().IsEqualTo(1);
        await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
    }

    /// <summary>
    /// A test that checks that all the exceptions that were delivered through the output scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExceptionsAreDeliveredOnOutputScheduler() =>
        await new TestScheduler().With(
            async scheduler =>
            {
                var child = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")));
                var childCommands = new[] { child };
                var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: scheduler);
                Exception? exception = null;
                fixture.ThrownExceptions.Subscribe(ex => exception = ex);
                fixture.Execute().Subscribe(_ => { }, _ => { });
                await Assert.That(exception).IsNull();
                scheduler.Start();
                await Assert.That(exception).IsTypeOf<InvalidOperationException>();
            });

    /// <summary>
    /// A test that executes the executes all child commands.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteExecutesAllChildCommands()
    {
        var child1 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var child2 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var child3 = ReactiveCommand.Create(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var childCommands = new[] { child1, child2, child3 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);

        fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();
        child1.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var child1IsExecuting).Subscribe();
        child2.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var child2IsExecuting).Subscribe();
        child3.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var child3IsExecuting).Subscribe();

        fixture.Execute().Subscribe();

        await Assert.That(isExecuting).Count().IsEqualTo(3);
        using (Assert.Multiple())
        {
            await Assert.That(isExecuting[0]).IsFalse();
            await Assert.That(isExecuting[1]).IsTrue();
            await Assert.That(isExecuting[2]).IsFalse();

            await Assert.That(child1IsExecuting).Count().IsEqualTo(3);
        }

        using (Assert.Multiple())
        {
            await Assert.That(child1IsExecuting[0]).IsFalse();
            await Assert.That(child1IsExecuting[1]).IsTrue();
            await Assert.That(child1IsExecuting[2]).IsFalse();

            await Assert.That(child2IsExecuting).Count().IsEqualTo(3);
        }

        using (Assert.Multiple())
        {
            await Assert.That(child2IsExecuting[0]).IsFalse();
            await Assert.That(child2IsExecuting[1]).IsTrue();
            await Assert.That(child2IsExecuting[2]).IsFalse();

            await Assert.That(child3IsExecuting).Count().IsEqualTo(3);
        }

        using (Assert.Multiple())
        {
            await Assert.That(child3IsExecuting[0]).IsFalse();
            await Assert.That(child3IsExecuting[1]).IsTrue();
            await Assert.That(child3IsExecuting[2]).IsFalse();
        }
    }

    /// <summary>
    /// Test that executes the ticks errors in any child command through thrown exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksErrorsInAnyChildCommandThroughThrownExceptions()
    {
        var child1 = ReactiveCommand.CreateFromObservable(static () => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        var child2 = ReactiveCommand.CreateFromObservable(static () => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

        fixture.Execute().Subscribe(static _ => { }, static _ => { });

        await Assert.That(thrownExceptions).Count().IsEqualTo(1);
        await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
    }

    /// <summary>
    /// Test that executes the ticks through the results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksThroughTheResults()
    {
        var child1 = ReactiveCommand.CreateFromObservable(static () => Observable.Return(1), outputScheduler: ImmediateScheduler.Instance);
        var child2 = ReactiveCommand.CreateFromObservable(static () => Observable.Return(2), outputScheduler: ImmediateScheduler.Instance);
        var childCommands = new[] { child1, child2 };
        var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: ImmediateScheduler.Instance);

        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute().Subscribe();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]).Count().IsEqualTo(2);
        using (Assert.Multiple())
        {
            await Assert.That(results[0][0]).IsEqualTo(1);
            await Assert.That(results[0][1]).IsEqualTo(2);
        }
    }

    /// <summary>
    /// Test that checks that results is ticked through specified scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResultIsTickedThroughSpecifiedScheduler() =>
        await new TestScheduler().WithAsync(
static async scheduler =>
            {
                // Allow scheduler to run freely
                var child1 = ReactiveCommand.Create(static () => Observable.Return(1));
                var child2 = ReactiveCommand.CreateRunInBackground(static () => Observable.Return(2));
                var childCommands = new[] { child1, child2 };
                var fixture = ReactiveCommand.CreateCombined(childCommands, outputScheduler: scheduler);
                fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

                fixture.Execute().Subscribe();
                await Assert.That(results).IsEmpty();

                scheduler.AdvanceByMs(1);
                await Assert.That(results).Count().IsEqualTo(1);
                return Task.CompletedTask;
            });
}
