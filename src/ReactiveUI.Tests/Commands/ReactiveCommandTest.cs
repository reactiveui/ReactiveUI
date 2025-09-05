// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

using DynamicData;
using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ReactiveCommand class.
/// </summary>
public class ReactiveCommandTest
{
    public ReactiveCommandTest()
    {
        RxApp.EnsureInitialized();
    }

    /// <summary>
    /// A test that determines whether this instance [can execute changed is available via ICommand].
    /// </summary>
    [Test]
    public void CanExecuteChangedIsAvailableViaICommand()
    {
        var canExecuteSubject = new Subject<bool>();
        ICommand? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
        var canExecuteChanged = new List<bool>();
        fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        Assert.That(canExecuteChanged.Count, Is.EqualTo(2));
        Assert.That(canExecuteChanged[0], Is.True);
        Assert.That(canExecuteChanged[1], Is.False);
    }

    /// <summary>
    /// A test that determines whether this instance [can execute is available via ICommand].
    /// </summary>
    [Test]
    public void CanExecuteIsAvailableViaICommand()
    {
        var canExecuteSubject = new Subject<bool>();
        ICommand? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);

        Assert.That(fixture.CanExecute(null, Is.False));

        canExecuteSubject.OnNext(true);
        Assert.That(fixture.CanExecute(null, Is.True));

        canExecuteSubject.OnNext(false);
        Assert.That(fixture.CanExecute(null, Is.False));
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is behavioral].
    /// </summary>
    [Test]
    public void CanExecuteIsBehavioral()
    {
        var fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        Assert.That(canExecute.Count, Is.EqualTo(1));
        Assert.That(canExecute[0], Is.True);
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is false if already executing].
    /// </summary>
    [Test]
    public void CanExecuteIsFalseIfAlreadyExecuting() =>
        new TestScheduler().With(
            scheduler =>
            {
                var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

                fixture.Execute().Subscribe();
                scheduler.AdvanceByMs(100);

                Assert.That(canExecute.Count, Is.EqualTo(2));
                Assert.That(canExecute[1], Is.False);

                scheduler.AdvanceByMs(901);

                Assert.That(canExecute.Count, Is.EqualTo(3));
                Assert.That(canExecute[2], Is.True);
            });

    /// <summary>
    /// Test that determines whether this instance [can execute is false if caller dictates as such].
    /// </summary>
    [Test]
    public void CanExecuteIsFalseIfCallerDictatesAsSuch()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        Assert.That(canExecute.Count, Is.EqualTo(3));
        Assert.That(canExecute[0], Is.False);
        Assert.That(canExecute[1], Is.True);
        Assert.That(canExecute[2], Is.False);
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is unsubscribed after command disposal].
    /// </summary>
    [Test]
    public void CanExecuteIsUnsubscribedAfterCommandDisposal()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);

        Assert.That(canExecuteSubject.HasObservers, Is.True);

        fixture.Dispose();

        Assert.That(canExecuteSubject.HasObservers, Is.False);
    }

    /// <summary>
    /// Test that determines whether this instance [can execute only ticks distinct values].
    /// </summary>
    [Test]
    public void CanExecuteOnlyTicksDistinctValues()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(true);

        Assert.That(canExecute.Count, Is.EqualTo(2));
        Assert.That(canExecute[0], Is.False);
        Assert.That(canExecute[1], Is.True);
    }

    /// <summary>
    /// Test that determines whether this instance [can execute ticks failures through thrown exceptions].
    /// </summary>
    [Test]
    public void CanExecuteTicksFailuresThroughThrownExceptions()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

        canExecuteSubject.OnError(new InvalidOperationException("oops"));

        Assert.That(thrownExceptions.Count, Is.EqualTo(1));
        Assert.That(thrownExceptions[0].Message, Is.EqualTo("oops"));
    }

    /// <summary>
    /// Creates the task facilitates TPL integration.
    /// </summary>
    [Test]
    public void CreateTaskFacilitatesTPLIntegration()
    {
        var fixture = ReactiveCommand.CreateFromTask(() => Task.FromResult(13), outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute().Subscribe();

        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0], Is.EqualTo(13));
    }

    /// <summary>
    /// Creates the task facilitates TPL integration with parameter.
    /// </summary>
    [Test]
    public void CreateTaskFacilitatesTPLIntegrationWithParameter()
    {
        var fixture = ReactiveCommand.CreateFromTask<int, int>(param => Task.FromResult(param + 1), outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute(3).Subscribe();
        fixture.Execute(41).Subscribe();

        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results[0], Is.EqualTo(4));
        Assert.That(results[1], Is.EqualTo(42));
    }

    /// <summary>
    /// Creates the throws if execution parameter is null.
    /// </summary>
    [Test]
    public void CreateThrowsIfExecutionParameterIsNull()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create(null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Action<Unit>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit, Unit>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<IObservable<Unit>>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Task<Unit>>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit, IObservable<Unit>>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.Create((Func<Unit, Task<Unit>>)null));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    /// <summary>
    /// Creates the throws if execution parameter is null.
    /// </summary>
    [Test]
    public void CreateRunInBackgroundThrowsIfExecutionParameterIsNull()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground(null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground((Func<Unit>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground((Action<Unit>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground((Func<Unit, Unit>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground((Func<IObservable<Unit>>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground((Func<Task<Unit>>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground((Func<Unit, IObservable<Unit>>)null));
        Assert.Throws<ArgumentNullException>(() => ReactiveCommand.CreateRunInBackground((Func<Unit, Task<Unit>>)null));
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    /// <summary>
    /// Exceptions the are delivered on output scheduler.
    /// </summary>
    [Test]
    public void ExceptionsAreDeliveredOnOutputScheduler() =>
        new TestScheduler().With(
            scheduler =>
            {
                var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()), outputScheduler: scheduler);
                Exception? exception = null;
                fixture.ThrownExceptions.Subscribe(ex => exception = ex);
                fixture.Execute().Subscribe(_ => { }, _ => { });

                Assert.That(exception, Is.Null);
                scheduler.Start();
                Assert.That(exception, Is.TypeOf<InvalidOperationException>());
            });

    /// <summary>
    /// Executes the can be cancelled.
    /// </summary>
    [Test]
    public void ExecuteCanBeCancelled() =>
        new TestScheduler().With(
            scheduler =>
            {
                var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

                var sub1 = fixture.Execute().Subscribe();
                var sub2 = fixture.Execute().Subscribe();
                scheduler.AdvanceByMs(999);

                Assert.That(fixture.IsExecuting.FirstAsync(, Is.True).Wait());
                Assert.That(executed, Is.Empty);
                sub1.Dispose();

                scheduler.AdvanceByMs(2);
                Assert.That(executed.Count, Is.EqualTo(1));
                Assert.That(fixture.IsExecuting.FirstAsync(, Is.False).Wait());
            });

    /// <summary>
    /// Executes the can tick through multiple results.
    /// </summary>
    [Test]
    public void ExecuteCanTickThroughMultipleResults()
    {
        var fixture = ReactiveCommand.CreateFromObservable(() => new[] { 1, 2, 3 }.ToObservable(), outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute().Subscribe();

        Assert.That(results.Count, Is.EqualTo(3));
        Assert.That(results[0], Is.EqualTo(1));
        Assert.That(results[1], Is.EqualTo(2));
        Assert.That(results[2], Is.EqualTo(3));
    }

    /// <summary>
    /// Executes the facilitates any number of in flight executions.
    /// </summary>
    [Test]
    public void ExecuteFacilitatesAnyNumberOfInFlightExecutions() =>
        new TestScheduler().With(
            scheduler =>
            {
                var execute = Observables.Unit.Delay(TimeSpan.FromMilliseconds(500), scheduler);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

                var sub1 = fixture.Execute().Subscribe();
                var sub2 = fixture.Execute().Subscribe();
                scheduler.AdvanceByMs(100);

                var sub3 = fixture.Execute().Subscribe();
                scheduler.AdvanceByMs(200);
                var sub4 = fixture.Execute().Subscribe();
                scheduler.AdvanceByMs(100);

                Assert.That(fixture.IsExecuting.FirstAsync(, Is.True).Wait());
                Assert.That(executed, Is.Empty);

                scheduler.AdvanceByMs(101);
                Assert.That(executed.Count, Is.EqualTo(2));
                Assert.That(fixture.IsExecuting.FirstAsync(, Is.True).Wait());

                scheduler.AdvanceByMs(200);
                Assert.That(executed.Count, Is.EqualTo(3));
                Assert.That(fixture.IsExecuting.FirstAsync(, Is.True).Wait());

                scheduler.AdvanceByMs(100);
                Assert.That(executed.Count, Is.EqualTo(4));
                Assert.That(fixture.IsExecuting.FirstAsync(, Is.False).Wait());
            });

    /// <summary>
    /// Executes the is available via ICommand.
    /// </summary>
    [Test]
    public void ExecuteIsAvailableViaICommand()
    {
        var executed = false;
        ICommand? fixture = ReactiveCommand.Create(
                                                  () =>
                                                  {
                                                      executed = true;
                                                      return Observables.Unit;
                                                  },
                                                  outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute(null);
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Executes the passes through parameter.
    /// </summary>
    [Test]
    public void ExecutePassesThroughParameter()
    {
        var parameters = new List<int>();
        var fixture = ReactiveCommand.CreateFromObservable<int, Unit>(
                                                                      param =>
                                                                      {
                                                                          parameters.Add(param);
                                                                          return Observables.Unit;
                                                                      },
                                                                      outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute(1).Subscribe();
        fixture.Execute(42).Subscribe();
        fixture.Execute(348).Subscribe();

        Assert.That(parameters.Count, Is.EqualTo(3));
        Assert.That(parameters[0], Is.EqualTo(1));
        Assert.That(parameters[1], Is.EqualTo(42));
        Assert.That(parameters[2], Is.EqualTo(348));
    }

    /// <summary>
    /// Executes the reenables execution even after failure.
    /// </summary>
    [Test]
    public void ExecuteReenablesExecutionEvenAfterFailure()
    {
        var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

        fixture.Execute().Subscribe(_ => { }, _ => { });

        Assert.That(thrownExceptions.Count, Is.EqualTo(1));
        Assert.That(thrownExceptions[0].Message, Is.EqualTo("oops"));

        Assert.That(canExecute.Count, Is.EqualTo(3));
        Assert.That(canExecute[0], Is.True);
        Assert.That(canExecute[1], Is.False);
        Assert.That(canExecute[2], Is.True);
    }

    /// <summary>
    /// Executes the result is delivered on specified scheduler.
    /// </summary>
    [Test]
    public void ExecuteResultIsDeliveredOnSpecifiedScheduler() =>
        new TestScheduler().With(
            scheduler =>
            {
                var execute = Observables.Unit;
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                var executed = false;

                fixture.Execute().ObserveOn(scheduler).Subscribe(_ => executed = true);

                Assert.That(executed, Is.False);
                scheduler.AdvanceByMs(1);
                Assert.That(executed, Is.True);
            });

    /// <summary>
    /// Executes the ticks any exception.
    /// </summary>
    [Test]
    public void ExecuteTicksAnyException()
    {
        var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()), outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.Subscribe();
        Exception? exception = null;
        fixture.Execute().Subscribe(_ => { }, ex => exception = ex, () => { });

        Assert.That(exception, Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Executes the ticks any lambda exception.
    /// </summary>
    [Test]
    public void ExecuteTicksAnyLambdaException()
    {
        var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.Subscribe();
        Exception? exception = null;
        fixture.Execute().Subscribe(_ => { }, ex => exception = ex, () => { });

        Assert.That(exception, Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Executes the ticks errors through thrown exceptions.
    /// </summary>
    [Test]
    public void ExecuteTicksErrorsThroughThrownExceptions()
    {
        var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

        fixture.Execute().Subscribe(_ => { }, _ => { });

        Assert.That(thrownExceptions.Count, Is.EqualTo(1));
        Assert.That(thrownExceptions[0].Message, Is.EqualTo("oops"));
    }

    /// <summary>
    /// Executes the ticks lambda errors through thrown exceptions.
    /// </summary>
    [Test]
    public void ExecuteTicksLambdaErrorsThroughThrownExceptions()
    {
        var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => throw new InvalidOperationException("oops"), outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

        fixture.Execute().Subscribe(_ => { }, _ => { });

        Assert.That(thrownExceptions.Count, Is.EqualTo(1));
        Assert.That(thrownExceptions[0].Message, Is.EqualTo("oops"));
        Assert.That(fixture.CanExecute.FirstAsync(, Is.True).Wait());
    }

    /// <summary>
    /// Executes the ticks through the result.
    /// </summary>
    [Test]
    public void ExecuteTicksThroughTheResult()
    {
        var num = 0;
        var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Return(num), outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        num = 1;
        fixture.Execute().Subscribe();
        num = 10;
        fixture.Execute().Subscribe();
        num = 30;
        fixture.Execute().Subscribe();

        Assert.That(results.Count, Is.EqualTo(3));
        Assert.That(results[0], Is.EqualTo(1));
        Assert.That(results[1], Is.EqualTo(10));
        Assert.That(results[2], Is.EqualTo(30));
    }

    /// <summary>
    /// Executes via ICommand throws if parameter type is incorrect.
    /// </summary>
    [Test]
    public void ExecuteViaICommandThrowsIfParameterTypeIsIncorrect()
    {
        ICommand? fixture = ReactiveCommand.Create<int>(_ => { }, outputScheduler: ImmediateScheduler.Instance);
        var ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute("foo"));
        Assert.That(but received parameter of type System.String.", ex.Message, Is.EqualTo("Command requires parameters of type System.Int32));

        fixture = ReactiveCommand.Create<string>(_ => { });
        ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute(13));
        Assert.That(but received parameter of type System.Int32.", ex.Message, Is.EqualTo("Command requires parameters of type System.String));
    }

    /// <summary>
    /// Executes via ICommand works with nullable types.
    /// </summary>
    [Test]
    public void ExecuteViaICommandWorksWithNullableTypes()
    {
        int? value = null;
        ICommand? fixture = ReactiveCommand.Create<int?>(param => value = param, outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute(42);
        Assert.That(value, Is.EqualTo(42));

        fixture.Execute(null);
        Assert.That(value, Is.Null);
    }

    /// <summary>
    /// Test that invokes the command against ICommand in target invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetInvokesTheCommand()
    {
        var executionCount = 0;
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture, x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against ICommand in target passes the specified value to can execute and execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new ICommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(fixture, x => x!.TheCommand!);
        var command = new FakeCommand();
        fixture.TheCommand = command;

        source.OnNext(42);
        Assert.That(command.CanExecuteParameter, Is.EqualTo(42));
        Assert.That(command.ExecuteParameter, Is.EqualTo(42));
    }

    /// <summary>
    /// Test that invokes the command against ICommand in target passes the specified value to can execute and execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInNullableTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new ICommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(fixture, x => x.TheCommand);
        var command = new FakeCommand();
        fixture.TheCommand = command;

        source.OnNext(42);
        Assert.That(command.CanExecuteParameter, Is.EqualTo(42));
        Assert.That(command.ExecuteParameter, Is.EqualTo(42));
    }

    /// <summary>
    /// Test that invokes the command against i command in target respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture, x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(executed, Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Test that invokes the command against i command in target respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInNullableTargetRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture, x => x.TheCommand);
        fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(executed, Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Test that invokes the command against ICommand in target respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture, x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(executed, Is.False);

        // The execution window re-opens, but the above execution request should not be instigated because
        // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
        canExecute.OnNext(true);
        Assert.That(executed, Is.False);
    }

    /// <summary>
    /// Test that invokes the command against ICommand in target swallows exceptions.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetSwallowsExceptions()
    {
        var count = 0;
        var fixture = new ICommandHolder();
        var command = ReactiveCommand.Create(
                                             () =>
                                             {
                                                 ++count;
                                                 throw new InvalidOperationException();
                                             },
                                             outputScheduler: ImmediateScheduler.Instance);
        command.ThrownExceptions.Subscribe();
        fixture.TheCommand = command;
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture, x => x.TheCommand!);

        source.OnNext(Unit.Default);
        source.OnNext(Unit.Default);

        Assert.That(count, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against ICommand invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInvokesTheCommand()
    {
        var executionCount = 0;
        ICommand fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against ICommand invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstNullableICommandInvokesTheCommand()
    {
        var executionCount = 0;
        ICommand? fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against ICommand passes the specified value to can execute and execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new FakeCommand();
        var source = new Subject<int>();
        source.InvokeCommand(fixture);

        source.OnNext(42);
        Assert.That(fixture.CanExecuteParameter, Is.EqualTo(42));
        Assert.That(fixture.ExecuteParameter, Is.EqualTo(42));
    }

    /// <summary>
    /// Test that invokes the command against ICommand respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        ICommand fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(executed, Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Test that invokes the command against ICommand respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        ICommand fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(executed, Is.False);

        // The execution window re-opens, but the above execution request should not be instigated because
        // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
        canExecute.OnNext(true);
        Assert.That(executed, Is.False);
    }

    /// <summary>
    /// Test that invokes the command against ICommand swallows exceptions.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandSwallowsExceptions()
    {
        var count = 0;
        var fixture = ReactiveCommand.Create(
                                             () =>
                                             {
                                                 ++count;
                                                 throw new InvalidOperationException();
                                             },
                                             outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.Subscribe();
        var source = new Subject<Unit>();
        source.InvokeCommand((ICommand)fixture);

        source.OnNext(Unit.Default);
        source.OnNext(Unit.Default);

        Assert.That(count, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against reactive command in target invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetInvokesTheCommand()
    {
        var executionCount = 0;
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(fixture, x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create<int>(_ => ++executionCount, outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(0);
        Assert.That(executionCount, Is.EqualTo(1));

        source.OnNext(0);
        Assert.That(executionCount, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against reactive command in target passes the specified value to execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetPassesTheSpecifiedValueToExecute()
    {
        var executeReceived = 0;
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(fixture, x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create<int>(x => executeReceived = x, outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(42);
        Assert.That(executeReceived, Is.EqualTo(42));
    }

    /// <summary>
    /// Test that invokes the command against reactive command in target respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(fixture, x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create<int>(_ => executed = true, canExecute, ImmediateScheduler.Instance);

        source.OnNext(0);
        Assert.That(executed, Is.False);

        canExecute.OnNext(true);
        source.OnNext(0);
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Test that invokes the command against reactive command in target respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(fixture, x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create<int>(_ => executed = true, canExecute, ImmediateScheduler.Instance);

        source.OnNext(0);
        Assert.That(executed, Is.False);

        // The execution window re-opens, but the above execution request should not be instigated because
        // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
        canExecute.OnNext(true);
        Assert.That(executed, Is.False);
    }

    /// <summary>
    /// Test that invokes the command against reactive command in target swallows exceptions.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetSwallowsExceptions()
    {
        var count = 0;
        var fixture = new ReactiveCommandHolder()
        {
            TheCommand = ReactiveCommand.Create<int>(
                                                     _ =>
                                                     {
                                                         ++count;
                                                         throw new InvalidOperationException();
                                                     },
                                                     outputScheduler: ImmediateScheduler.Instance)
        };
        fixture.TheCommand.ThrownExceptions.Subscribe();
        var source = new Subject<int>();
        source.InvokeCommand(fixture, x => x.TheCommand!);

        source.OnNext(0);
        source.OnNext(0);

        Assert.That(count, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against reactive command invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInvokesTheCommand()
    {
        var executionCount = 0;
        var fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(executionCount, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command against reactive command passes the specified value to execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandPassesTheSpecifiedValueToExecute()
    {
        var executeReceived = 0;
        var fixture = ReactiveCommand.Create<int>(x => executeReceived = x, outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<int>();
        source.InvokeCommand(fixture);

        source.OnNext(42);
        Assert.That(executeReceived, Is.EqualTo(42));
    }

    /// <summary>
    /// Test that invokes the command against reactive command respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(executed, Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Test that invokes the command against reactive command respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = ReactiveCommand.Create(() => executed = true, canExecute, outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(executed, Is.False);

        // The execution window re-opens, but the above execution request should not be instigated because
        // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
        canExecute.OnNext(true);
        Assert.That(executed, Is.False);
    }

    /// <summary>
    /// Test that invokes the command against reactive command swallows exceptions.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandSwallowsExceptions()
    {
        var count = 0;
        var fixture = ReactiveCommand.Create(
                                             () =>
                                             {
                                                 ++count;
                                                 throw new InvalidOperationException();
                                             },
                                             outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.Subscribe();
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        source.OnNext(Unit.Default);

        Assert.That(count, Is.EqualTo(2));
    }

    /// <summary>
    /// Test that invokes the command works even if the source is cold.
    /// </summary>
    [Test]
    public void InvokeCommandWorksEvenIfTheSourceIsCold()
    {
        var executionCount = 0;
        var fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return(Unit.Default);
        source.InvokeCommand(fixture);

        Assert.That(executionCount, Is.EqualTo(1));
    }

    /// <summary>
    /// Test that determines whether [is executing is behavioral].
    /// </summary>
    [Test]
    public void IsExecutingIsBehavioral()
    {
        var fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
        fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

        Assert.That(isExecuting.Count, Is.EqualTo(1));
        Assert.That(isExecuting[0], Is.False);
    }

    /// <summary>
    /// Test that determines whether [is executing remains true as long as execution pipeline has not completed].
    /// </summary>
    [Test]
    public void IsExecutingRemainsTrueAsLongAsExecutionPipelineHasNotCompleted()
    {
        var execute = new Subject<Unit>();
        var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute().Subscribe();

        Assert.That(fixture.IsExecuting.FirstAsync(, Is.True).Wait());

        execute.OnNext(Unit.Default);
        Assert.That(fixture.IsExecuting.FirstAsync(, Is.True).Wait());

        execute.OnNext(Unit.Default);
        Assert.That(fixture.IsExecuting.FirstAsync(, Is.True).Wait());

        execute.OnCompleted();
        Assert.That(fixture.IsExecuting.FirstAsync(, Is.False).Wait());
    }

    /// <summary>
    /// Test that determines whether [is executing ticks as executions progress].
    /// </summary>
    [Test]
    public void IsExecutingTicksAsExecutionsProgress() =>
        new TestScheduler().With(
            scheduler =>
            {
                var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
                var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

                fixture.Execute().Subscribe();
                scheduler.AdvanceByMs(100);

                Assert.That(isExecuting.Count, Is.EqualTo(2));
                Assert.That(isExecuting[0], Is.False);
                Assert.That(isExecuting[1], Is.True);

                scheduler.AdvanceByMs(901);

                Assert.That(isExecuting.Count, Is.EqualTo(3));
                Assert.That(isExecuting[2], Is.False);
            });

    /// <summary>
    /// Results the is ticked through specified scheduler.
    /// </summary>
    [Test]
    public void ResultIsTickedThroughSpecifiedScheduler() =>
        new TestScheduler().WithAsync(
            scheduler =>
            {
                var fixture = ReactiveCommand.CreateRunInBackground(() => Observables.Unit, outputScheduler: scheduler);
                fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

                fixture.Execute().Subscribe();
                Assert.That(results, Is.Empty);

                scheduler.AdvanceByMs(1);
                Assert.That(results.Count, Is.EqualTo(1));
                return Task.CompletedTask;
            });

    /// <summary>
    /// Synchronouses the command execute lazily.
    /// </summary>
    [Test]
    public void SynchronousCommandExecuteLazily()
    {
        var executionCount = 0;
#pragma warning disable IDE0053 // Use expression body for lambda expressions
#pragma warning disable RCS1021 // Convert lambda expression body to expression-body.
        var fixture1 = ReactiveCommand.Create(() => { ++executionCount; }, outputScheduler: ImmediateScheduler.Instance);
        var fixture2 = ReactiveCommand.Create<int>(_ => { ++executionCount; }, outputScheduler: ImmediateScheduler.Instance);
#pragma warning restore RCS1021 // Convert lambda expression body to expression-body.
#pragma warning restore IDE0053 // Use expression body for lambda expressions
        var fixture3 = ReactiveCommand.Create(
                                              () =>
                                              {
                                                  ++executionCount;
                                                  return 42;
                                              },
                                              outputScheduler: ImmediateScheduler.Instance);
        var fixture4 = ReactiveCommand.Create<int, int>(
                                                        _ =>
                                                        {
                                                            ++executionCount;
                                                            return 42;
                                                        },
                                                        outputScheduler: ImmediateScheduler.Instance);
        var execute1 = fixture1.Execute();
        var execute2 = fixture2.Execute();
        var execute3 = fixture3.Execute();
        var execute4 = fixture4.Execute();

        Assert.That(executionCount, Is.EqualTo(0));

        execute1.Subscribe();
        Assert.That(executionCount, Is.EqualTo(1));

        execute2.Subscribe();
        Assert.That(executionCount, Is.EqualTo(2));

        execute3.Subscribe();
        Assert.That(executionCount, Is.EqualTo(3));

        execute4.Subscribe();
        Assert.That(executionCount, Is.EqualTo(4));
    }

    /// <summary>
    /// Synchronouses the commands fail correctly.
    /// </summary>
    [Test]
    public void SynchronousCommandsFailCorrectly()
    {
        var fixture1 = ReactiveCommand.Create(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
        var fixture2 = ReactiveCommand.Create<int>(_ => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
        var fixture3 = ReactiveCommand.Create(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
        var fixture4 = ReactiveCommand.Create<int, int>(_ => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);

        var failureCount = 0;
        Observable.Merge(fixture1.ThrownExceptions, fixture2.ThrownExceptions, fixture3.ThrownExceptions, fixture4.ThrownExceptions).Subscribe(_ => ++failureCount);

        fixture1.Execute().Subscribe(_ => { }, _ => { });
        Assert.That(failureCount, Is.EqualTo(1));

        fixture2.Execute().Subscribe(_ => { }, _ => { });
        Assert.That(failureCount, Is.EqualTo(2));

        fixture3.Execute().Subscribe(_ => { }, _ => { });
        Assert.That(failureCount, Is.EqualTo(3));

        fixture4.Execute().Subscribe(_ => { }, _ => { });
        Assert.That(failureCount, Is.EqualTo(4));
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesTaskExceptionAsync()
    {
        using var testSequencer = new TestSequencer();
        var subj = new Subject<Unit>();
        var isExecuting = false;
        Exception? fail = null;
        var fixture = ReactiveCommand.CreateFromTask(
            async _ =>
            {
                await subj.Take(1);
                throw new Exception("break execution");
            },
            outputScheduler: ImmediateScheduler.Instance);

        fixture.IsExecuting.Subscribe(async x =>
        {
            isExecuting = x;
            await testSequencer.AdvancePhaseAsync("Executing {false, true, false}");
        });
        fixture.ThrownExceptions.Subscribe(async ex =>
        {
            fail = ex;
            await testSequencer.AdvancePhaseAsync("Exception");
        });

        await testSequencer.AdvancePhaseAsync("Executing {false}");
        Assert.That(isExecuting, Is.False);
        Assert.That(fail, Is.Null);

        fixture.Execute().Subscribe();
        await testSequencer.AdvancePhaseAsync("Executing {true}");
        Assert.That(isExecuting, Is.True);
        Assert.That(fail, Is.Null);

        subj.OnNext(Unit.Default);

        // Wait to allow execution to complete
        await testSequencer.AdvancePhaseAsync("Executing {false}");
        await testSequencer.AdvancePhaseAsync("Exception");
        Assert.That(isExecuting, Is.False);
        Assert.That(fail?.Message, Is.EqualTo("break execution"));
        testSequencer.Dispose();
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskThenCancelSetsIsExecutingFalseOnlyAfterCancellationCompleteAsync()
    {
        using var testSequencer = new TestSequencer();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;

        var fixture = ReactiveCommand.CreateFromTask(async (token) =>
        {
            // Phase 1: command execution has begun.
            await testSequencer.AdvancePhaseAsync("Phase 1");
            statusTrail.Add((position++, "started command"));
            try
            {
                await Task.Delay(10000, token);
            }
            catch (OperationCanceledException)
            {
                // Phase 2: command task has detected cancellation request.
                await testSequencer.AdvancePhaseAsync("Phase 2");

                // Phase 3: test has observed IsExecuting while cancellation is in progress.
                await testSequencer.AdvancePhaseAsync("Phase 3");
                throw;
            }

            statusTrail.Add((position++, "finished command"));
        });

        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExecuting =>
        {
            statusTrail.Add((position++, $"command executing = {isExecuting}"));
            Volatile.Write(ref latestIsExecutingValue, isExecuting);
        });

        var disposable = fixture.Execute().Subscribe();

        // Phase 1: command execution has begun.
        await testSequencer.AdvancePhaseAsync("Phase 1");

        Volatile.Read(ref latestIsExecutingValue).Should().BeTrue("IsExecuting should be true when execution is underway");

        disposable.Dispose();

        // Phase 2: command task has detected cancellation request.
        await testSequencer.AdvancePhaseAsync("Phase 2");

        Volatile.Read(ref latestIsExecutingValue).Should().BeTrue("IsExecuting should remain true while cancellation is in progress");

        // Phase 3: test has observed IsExecuting while cancellation is in progress.
        await testSequencer.AdvancePhaseAsync("Phase 3");

        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        Volatile.Read(ref latestIsExecutingValue).Should().BeFalse("IsExecuting should be false once cancellation completes");
        statusTrail.Should().Equal(
                           (0, "command executing = False"),
                           (1, "command executing = True"),
                           (2, "started command"),
                           (3, "command executing = False"));
    }

    [Test]
    public async Task ReactiveCommandExecutesFromInvokeCommand()
    {
        using var testSequencer = new TestSequencer();

        var command = ReactiveCommand.Create(async () => await testSequencer.AdvancePhaseAsync("Phase 1"));
        var result = 0;

        // False, True, False
        command.IsExecuting.Subscribe(_ => result++);

        Observable.Return(Unit.Default)
                  .InvokeCommand(command);

        await testSequencer.AdvancePhaseAsync("Phase 1");
        Assert.That(result, Is.EqualTo(3));

        testSequencer.Dispose();
    }

    [Test]
    public void ShouldCallAsyncMethodOnSettingReactiveSetpoint() =>
        new TestScheduler().WithAsync(scheduler =>
        {
            // set
            var fooVm = new Mocks.FooViewModel(new());

            fooVm.Foo.Value.Should().Be(42, "initial value unchanged");

            // act
            scheduler.AdvanceByMs(11); // async processing
            fooVm.Foo.Value.Should().Be(0, "value set to default Setpoint value");

            fooVm.Setpoint = 123;
            scheduler.AdvanceByMs(5); // async task processing

            // assert
            fooVm.Foo.Value.Should().Be(0, "value unchanged as async task still processing");
            scheduler.AdvanceByMs(6); // process async setpoint setting

            fooVm.Foo.Value.Should().Be(123, "value set to Setpoint value");
            return Task.CompletedTask;
        });

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesExecuteCancellation()
    {
        using var testSequencer = new TestSequencer();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;
        var fixture = ReactiveCommand.CreateFromTask(
                    async cts =>
                    {
                        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
                        statusTrail.Add((position++, "started command"));
                        try
                        {
                            await Task.Delay(10000, cts);
                        }
                        catch (OperationCanceledException)
                        {
                            // User Handles cancellation.
                            statusTrail.Add((position++, "starting cancelling command"));
                            await testSequencer.AdvancePhaseAsync("Phase 2"); // #2

                            // dummy cleanup
                            await testSequencer.AdvancePhaseAsync("Phase 3"); // #3
                            statusTrail.Add((position++, "finished cancelling command"));
                            throw;
                        }

                        return Unit.Default;
                    },
                    outputScheduler: ImmediateScheduler.Instance);

        Exception? fail = null;
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);
        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExecuting =>
        {
            statusTrail.Add((position++, $"command executing = {isExecuting}"));
            Volatile.Write(ref latestIsExecutingValue, isExecuting);
        });

        fail.Should().BeNull();
        var result = false;
        var disposable = fixture.Execute().Subscribe(_ => result = true);
        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
        Volatile.Read(ref latestIsExecutingValue).Should().BeTrue();
        statusTrail.Any(x => x.Status == "started command").Should().BeTrue();
        disposable.Dispose();
        await testSequencer.AdvancePhaseAsync("Phase 2"); // #2
        Volatile.Read(ref latestIsExecutingValue).Should().BeTrue();
        await testSequencer.AdvancePhaseAsync("Phase 3"); // #3

        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        // No result expected as cancelled
        result.Should().BeFalse();
        statusTrail.Should().Equal(
                           (0, "command executing = False"),
                           (1, "command executing = True"),
                           (2, "started command"),
                           (3, "starting cancelling command"),
                           (4, "finished cancelling command"),
                           (5, "command executing = False"));
        (fail as OperationCanceledException).Should().NotBeNull();
    }

    [Test]
    public void ReactiveCommandCreateFromTaskHandlesTaskException() =>
        new TestScheduler().With(
            async scheduler =>
            {
                var subj = new Subject<Unit>();
                Exception? fail = null;
                var fixture = ReactiveCommand.CreateFromTask(
                    async cts =>
                    {
                        await subj.Take(1);
                        throw new Exception("break execution");
                    },
                    outputScheduler: scheduler);
                fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();
                fixture.ThrownExceptions.Subscribe(ex => fail = ex);
                isExecuting[0].Should().BeFalse();
                fail.Should().BeNull();
                fixture.Execute().Subscribe();

                scheduler.AdvanceByMs(10);
                isExecuting[1].Should().BeTrue();
                fail.Should().BeNull();

                scheduler.AdvanceByMs(10);
                subj.OnNext(Unit.Default);

                scheduler.AdvanceByMs(10);
                isExecuting[2].Should().BeFalse();
                fail?.Message.Should().Be("break execution");

                // Required for correct async / await task handling
                await Task.Delay(0);
            });

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesCancellation()
    {
        using var testSequencer = new TestSequencer();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;
        var fixture = ReactiveCommand.CreateFromTask(
                    async cts =>
                    {
                        statusTrail.Add((position++, "started command"));
                        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
                        try
                        {
                            await Task.Delay(10000, cts);
                        }
                        catch (OperationCanceledException)
                        {
                            // User Handles cancellation.
                            statusTrail.Add((position++, "starting cancelling command"));
                            await testSequencer.AdvancePhaseAsync("Phase 2"); // #2

                            // dummy cleanup
                            statusTrail.Add((position++, "finished cancelling command"));
                            await testSequencer.AdvancePhaseAsync("Phase 3"); // #3
                            throw;
                        }

                        return Unit.Default;
                    },
                    outputScheduler: ImmediateScheduler.Instance);

        Exception? fail = null;
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);
        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExecuting =>
        {
            statusTrail.Add((position++, $"command executing = {isExecuting}"));
            Volatile.Write(ref latestIsExecutingValue, isExecuting);
        });

        fail.Should().BeNull();
        var result = false;
        var disposable = fixture.Execute().Subscribe(_ => result = true);
        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
        Volatile.Read(ref latestIsExecutingValue).Should().BeTrue();
        statusTrail.Any(x => x.Status == "started command").Should().BeTrue();
        disposable.Dispose();
        await testSequencer.AdvancePhaseAsync("Phase 2"); // #2
        Volatile.Read(ref latestIsExecutingValue).Should().BeTrue();
        await testSequencer.AdvancePhaseAsync("Phase 3"); // #3
        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        // No result expected as cancelled
        result.Should().BeFalse();
        statusTrail.Should().Equal(
                           (0, "command executing = False"),
                           (1, "command executing = True"),
                           (2, "started command"),
                           (3, "starting cancelling command"),
                           (4, "finished cancelling command"),
                           (5, "command executing = False"));
        (fail as OperationCanceledException).Should().NotBeNull();
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesCompletion()
    {
        using var testSequencer = new TestSequencer();
        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;
        var fixture = ReactiveCommand.CreateFromTask(
                    async cts =>
                    {
                        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
                        statusTrail.Add((position++, "started command"));
                        try
                        {
                            await Task.Delay(1000, cts);
                        }
                        catch (OperationCanceledException)
                        {
                            // User Handles cancellation.
                            statusTrail.Add((position++, "starting cancelling command"));

                            // dummy cleanup
                            await Task.Delay(5000, CancellationToken.None);
                            statusTrail.Add((position++, "finished cancelling command"));
                            throw;
                        }

                        statusTrail.Add((position++, "finished command"));
                        await testSequencer.AdvancePhaseAsync("Phase 2"); // #2
                        return Unit.Default;
                    },
                    outputScheduler: ImmediateScheduler.Instance);

        Exception? fail = null;
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);
        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExecuting =>
        {
            statusTrail.Add((position++, $"command executing = {isExecuting}"));
            Volatile.Write(ref latestIsExecutingValue, isExecuting);
        });

        fail.Should().BeNull();
        var result = false;
        fixture.Execute().Subscribe(_ => result = true);
        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
        Volatile.Read(ref latestIsExecutingValue).Should().BeTrue();
        await testSequencer.AdvancePhaseAsync("Phase 2"); // #2

        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        result.Should().BeTrue();
        statusTrail.Should().Equal(
                           (0, "command executing = False"),
                           (1, "command executing = True"),
                           (2, "started command"),
                           (3, "finished command"),
                           (4, "command executing = False"));
        fail.Should().BeNull();

        // Check execution completed
        Volatile.Read(ref latestIsExecutingValue).Should().BeFalse();
    }
}
