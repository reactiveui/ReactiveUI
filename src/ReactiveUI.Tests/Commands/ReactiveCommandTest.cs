// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests for the ReactiveCommand class.
    /// </summary>
    public class ReactiveCommandTest
    {
        /// <summary>
        /// A test that determines whether this instance [can execute changed is available via ICommand].
        /// </summary>
        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            Subject<bool> canExecuteSubject = new();
            ICommand? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
            List<bool> canExecuteChanged = new();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecuteChanged.Count);
            Assert.True(canExecuteChanged[0]);
            Assert.False(canExecuteChanged[1]);
        }

        /// <summary>
        /// A test that determines whether this instance [can execute is available via ICommand].
        /// </summary>
        [Fact]
        public void CanExecuteIsAvailableViaICommand()
        {
            Subject<bool> canExecuteSubject = new();
            ICommand? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);

            Assert.False(fixture.CanExecute(null));

            canExecuteSubject.OnNext(true);
            Assert.True(fixture.CanExecute(null));

            canExecuteSubject.OnNext(false);
            Assert.False(fixture.CanExecute(null));
        }

        /// <summary>
        /// Test that determines whether this instance [can execute is behavioral].
        /// </summary>
        [Fact]
        public void CanExecuteIsBehavioral()
        {
            ReactiveCommand<Unit, IObservable<Unit>>? fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

            Assert.Equal(1, canExecute.Count);
            Assert.True(canExecute[0]);
        }

        /// <summary>
        /// Test that determines whether this instance [can execute is false if already executing].
        /// </summary>
        [Fact]
        public void CanExecuteIsFalseIfAlreadyExecuting() =>
            new TestScheduler().With(
                scheduler =>
                {
                    var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
                    ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                    fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

                    fixture.Execute().Subscribe();
                    scheduler.AdvanceByMs(100);

                    Assert.Equal(2, canExecute.Count);
                    Assert.False(canExecute[1]);

                    scheduler.AdvanceByMs(901);

                    Assert.Equal(3, canExecute.Count);
                    Assert.True(canExecute[2]);
                });

        /// <summary>
        /// Test that determines whether this instance [can execute is false if caller dictates as such].
        /// </summary>
        [Fact]
        public void CanExecuteIsFalseIfCallerDictatesAsSuch()
        {
            Subject<bool> canExecuteSubject = new();
            ReactiveCommand<Unit, IObservable<Unit>>? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
            fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(false);

            Assert.Equal(3, canExecute.Count);
            Assert.False(canExecute[0]);
            Assert.True(canExecute[1]);
            Assert.False(canExecute[2]);
        }

        /// <summary>
        /// Test that determines whether this instance [can execute is unsubscribed after command disposal].
        /// </summary>
        [Fact]
        public void CanExecuteIsUnsubscribedAfterCommandDisposal()
        {
            Subject<bool> canExecuteSubject = new();
            ReactiveCommand<Unit, IObservable<Unit>>? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);

            Assert.True(canExecuteSubject.HasObservers);

            fixture.Dispose();

            Assert.False(canExecuteSubject.HasObservers);
        }

        /// <summary>
        /// Test that determines whether this instance [can execute only ticks distinct values].
        /// </summary>
        [Fact]
        public void CanExecuteOnlyTicksDistinctValues()
        {
            Subject<bool> canExecuteSubject = new();
            ReactiveCommand<Unit, IObservable<Unit>>? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
            fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

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

        /// <summary>
        /// Test that determines whether this instance [can execute ticks failures through thrown exceptions].
        /// </summary>
        [Fact]
        public void CanExecuteTicksFailuresThroughThrownExceptions()
        {
            Subject<bool> canExecuteSubject = new();
            ReactiveCommand<Unit, IObservable<Unit>>? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

            canExecuteSubject.OnError(new InvalidOperationException("oops"));

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        /// <summary>
        /// Creates the task facilitates TPL integration.
        /// </summary>
        [Fact]
        public void CreateTaskFacilitatesTPLIntegration()
        {
            ReactiveCommand<Unit, int>? fixture = ReactiveCommand.CreateFromTask(() => Task.FromResult(13), outputScheduler: ImmediateScheduler.Instance);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

            fixture.Execute().Subscribe();

            Assert.Equal(1, results.Count);
            Assert.Equal(13, results[0]);
        }

        /// <summary>
        /// Creates the task facilitates TPL integration with parameter.
        /// </summary>
        [Fact]
        public void CreateTaskFacilitatesTPLIntegrationWithParameter()
        {
            ReactiveCommand<int, int>? fixture = ReactiveCommand.CreateFromTask<int, int>(param => Task.FromResult(param + 1), outputScheduler: ImmediateScheduler.Instance);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

            fixture.Execute(3).Subscribe();
            fixture.Execute(41).Subscribe();

            Assert.Equal(2, results.Count);
            Assert.Equal(4, results[0]);
            Assert.Equal(42, results[1]);
        }

        /// <summary>
        /// Creates the throws if execution parameter is null.
        /// </summary>
        [Fact]
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
        /// Exceptions the are delivered on output scheduler.
        /// </summary>
        [Fact]
        public void ExceptionsAreDeliveredOnOutputScheduler() =>
            new TestScheduler().With(
                scheduler =>
                {
                    ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()), outputScheduler: scheduler);
                    Exception? exception = null;
                    fixture.ThrownExceptions.Subscribe(ex => exception = ex);
                    fixture.Execute().Subscribe(_ => { }, _ => { });

                    Assert.Null(exception);
                    scheduler.Start();
                    Assert.IsType<InvalidOperationException>(exception);
                });

        /// <summary>
        /// Executes the can be cancelled.
        /// </summary>
        [Fact]
        public void ExecuteCanBeCancelled() =>
            new TestScheduler().With(
                scheduler =>
                {
                    var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
                    ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                    fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

                    var sub1 = fixture.Execute().Subscribe();
                    var sub2 = fixture.Execute().Subscribe();
                    scheduler.AdvanceByMs(999);

                    Assert.True(fixture.IsExecuting.FirstAsync().Wait());
                    Assert.Empty(executed);
                    sub1.Dispose();

                    scheduler.AdvanceByMs(2);
                    Assert.Equal(1, executed.Count);
                    Assert.False(fixture.IsExecuting.FirstAsync().Wait());
                });

        /// <summary>
        /// Executes the can tick through multiple results.
        /// </summary>
        [Fact]
        public void ExecuteCanTickThroughMultipleResults()
        {
            ReactiveCommand<Unit, int>? fixture = ReactiveCommand.CreateFromObservable(() => new[] { 1, 2, 3 }.ToObservable(), outputScheduler: ImmediateScheduler.Instance);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

            fixture.Execute().Subscribe();

            Assert.Equal(3, results.Count);
            Assert.Equal(1, results[0]);
            Assert.Equal(2, results[1]);
            Assert.Equal(3, results[2]);
        }

        /// <summary>
        /// Executes the facilitates any number of in flight executions.
        /// </summary>
        [Fact]
        public void ExecuteFacilitatesAnyNumberOfInFlightExecutions() =>
            new TestScheduler().With(
                scheduler =>
                {
                    var execute = Observables.Unit.Delay(TimeSpan.FromMilliseconds(500), scheduler);
                    ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                    fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

                    var sub1 = fixture.Execute().Subscribe();
                    var sub2 = fixture.Execute().Subscribe();
                    scheduler.AdvanceByMs(100);

                    var sub3 = fixture.Execute().Subscribe();
                    scheduler.AdvanceByMs(200);
                    var sub4 = fixture.Execute().Subscribe();
                    scheduler.AdvanceByMs(100);

                    Assert.True(fixture.IsExecuting.FirstAsync().Wait());
                    Assert.Empty(executed);

                    scheduler.AdvanceByMs(101);
                    Assert.Equal(2, executed.Count);
                    Assert.True(fixture.IsExecuting.FirstAsync().Wait());

                    scheduler.AdvanceByMs(200);
                    Assert.Equal(3, executed.Count);
                    Assert.True(fixture.IsExecuting.FirstAsync().Wait());

                    scheduler.AdvanceByMs(100);
                    Assert.Equal(4, executed.Count);
                    Assert.False(fixture.IsExecuting.FirstAsync().Wait());
                });

        /// <summary>
        /// Executes the is available via ICommand.
        /// </summary>
        [Fact]
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
            Assert.True(executed);
        }

        /// <summary>
        /// Executes the passes through parameter.
        /// </summary>
        [Fact]
        public void ExecutePassesThroughParameter()
        {
            List<int> parameters = new();
            ReactiveCommand<int, Unit>? fixture = ReactiveCommand.CreateFromObservable<int, Unit>(
                                                                          param =>
                                                                          {
                                                                              parameters.Add(param);
                                                                              return Observables.Unit;
                                                                          },
                                                                          outputScheduler: ImmediateScheduler.Instance);

            fixture.Execute(1).Subscribe();
            fixture.Execute(42).Subscribe();
            fixture.Execute(348).Subscribe();

            Assert.Equal(3, parameters.Count);
            Assert.Equal(1, parameters[0]);
            Assert.Equal(42, parameters[1]);
            Assert.Equal(348, parameters[2]);
        }

        /// <summary>
        /// Executes the reenables execution even after failure.
        /// </summary>
        [Fact]
        public void ExecuteReenablesExecutionEvenAfterFailure()
        {
            ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
            fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

            fixture.Execute().Subscribe(_ => { }, _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);

            Assert.Equal(3, canExecute.Count);
            Assert.True(canExecute[0]);
            Assert.False(canExecute[1]);
            Assert.True(canExecute[2]);
        }

        /// <summary>
        /// Executes the result is delivered on specified scheduler.
        /// </summary>
        [Fact]
        public void ExecuteResultIsDeliveredOnSpecifiedScheduler() =>
            new TestScheduler().With(
                scheduler =>
                {
                    var execute = Observables.Unit;
                    ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                    var executed = false;

                    fixture.Execute().Subscribe(_ => executed = true);

                    Assert.False(executed);
                    scheduler.AdvanceByMs(1);
                    Assert.True(executed);
                });

        /// <summary>
        /// Executes the ticks any exception.
        /// </summary>
        [Fact]
        public void ExecuteTicksAnyException()
        {
            ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()), outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.Subscribe();
            Exception? exception = null;
            fixture.Execute().Subscribe(_ => { }, ex => exception = ex, () => { });

            Assert.IsType<InvalidOperationException>(exception);
        }

        /// <summary>
        /// Executes the ticks any lambda exception.
        /// </summary>
        [Fact]
        public void ExecuteTicksAnyLambdaException()
        {
            ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable<Unit>(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.Subscribe();
            Exception? exception = null;
            fixture.Execute().Subscribe(_ => { }, ex => exception = ex, () => { });

            Assert.IsType<InvalidOperationException>(exception);
        }

        /// <summary>
        /// Executes the ticks errors through thrown exceptions.
        /// </summary>
        [Fact]
        public void ExecuteTicksErrorsThroughThrownExceptions()
        {
            ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

            fixture.Execute().Subscribe(_ => { }, _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
        }

        /// <summary>
        /// Executes the ticks lambda errors through thrown exceptions.
        /// </summary>
        [Fact]
        public void ExecuteTicksLambdaErrorsThroughThrownExceptions()
        {
            ReactiveCommand<Unit, Unit>? fixture = ReactiveCommand.CreateFromObservable<Unit>(() => throw new InvalidOperationException("oops"), outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions).Subscribe();

            fixture.Execute().Subscribe(_ => { }, _ => { });

            Assert.Equal(1, thrownExceptions.Count);
            Assert.Equal("oops", thrownExceptions[0].Message);
            Assert.True(fixture.CanExecute.FirstAsync().Wait());
        }

        /// <summary>
        /// Executes the ticks through the result.
        /// </summary>
        [Fact]
        public void ExecuteTicksThroughTheResult()
        {
            var num = 0;
            ReactiveCommand<Unit, int>? fixture = ReactiveCommand.CreateFromObservable(() => Observable.Return(num), outputScheduler: ImmediateScheduler.Instance);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

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

        /// <summary>
        /// Executes via ICommand throws if parameter type is incorrect.
        /// </summary>
        [Fact]
        public void ExecuteViaICommandThrowsIfParameterTypeIsIncorrect()
        {
            ICommand? fixture = ReactiveCommand.Create<int>(_ => { }, outputScheduler: ImmediateScheduler.Instance);
            var ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute("foo"));
            Assert.Equal("Command requires parameters of type System.Int32, but received parameter of type System.String.", ex.Message);

            fixture = ReactiveCommand.Create<string>(_ => { });
            ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute(13));
            Assert.Equal("Command requires parameters of type System.String, but received parameter of type System.Int32.", ex.Message);
        }

        /// <summary>
        /// Executes via ICommand works with nullable types.
        /// </summary>
        [Fact]
        public void ExecuteViaICommandWorksWithNullableTypes()
        {
            int? value = null;
            ICommand? fixture = ReactiveCommand.Create<int?>(param => value = param, outputScheduler: ImmediateScheduler.Instance);

            fixture.Execute(42);
            Assert.Equal(42, value);

            fixture.Execute(null);
            Assert.Null(value);
        }

        /// <summary>
        /// Test that invokes the command against ICommand in target invokes the command.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInTargetInvokesTheCommand()
        {
            var executionCount = 0;
            ICommandHolder? fixture = new();
            Subject<Unit> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);
            fixture.TheCommand = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);

            source.OnNext(Unit.Default);
            Assert.Equal(1, executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(2, executionCount);
        }

        /// <summary>
        /// Test that invokes the command against ICommand in target passes the specified value to can execute and execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
        {
            ICommandHolder fixture = new();
            Subject<int> source = new();
            source.InvokeCommand(fixture, x => x!.TheCommand!);
            FakeCommand command = new();
            fixture.TheCommand = command;

            source.OnNext(42);
            Assert.Equal(42, command.CanExecuteParameter);
            Assert.Equal(42, command.ExecuteParameter);
        }

        /// <summary>
        /// Test that invokes the command against ICommand in target passes the specified value to can execute and execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInNullableTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
        {
            ICommandHolder? fixture = new();
            Subject<int> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand);
            FakeCommand? command = new();
            fixture.TheCommand = command;

            source.OnNext(42);
            Assert.Equal(42, command.CanExecuteParameter);
            Assert.Equal(42, command.ExecuteParameter);
        }

        /// <summary>
        /// Test that invokes the command against i command in target respects can execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInTargetRespectsCanExecute()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ICommandHolder fixture = new();
            Subject<Unit> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);
            fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(Unit.Default);
            Assert.True(executed);
        }

        /// <summary>
        /// Test that invokes the command against i command in target respects can execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInNullableTargetRespectsCanExecute()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ICommandHolder? fixture = new();
            Subject<Unit> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand);
            fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(Unit.Default);
            Assert.True(executed);
        }

        /// <summary>
        /// Test that invokes the command against ICommand in target respects can execute window.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInTargetRespectsCanExecuteWindow()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ICommandHolder fixture = new();
            Subject<Unit> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);
            fixture.TheCommand = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        /// <summary>
        /// Test that invokes the command against ICommand in target swallows exceptions.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInTargetSwallowsExceptions()
        {
            var count = 0;
            ICommandHolder fixture = new();
            ReactiveCommand<Unit, Unit> command = ReactiveCommand.Create(
                                                 () =>
                                                 {
                                                     ++count;
                                                     throw new InvalidOperationException();
                                                 },
                                                 outputScheduler: ImmediateScheduler.Instance);
            command.ThrownExceptions.Subscribe();
            fixture.TheCommand = command;
            Subject<Unit> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);

            source.OnNext(Unit.Default);
            source.OnNext(Unit.Default);

            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test that invokes the command against ICommand invokes the command.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandInvokesTheCommand()
        {
            var executionCount = 0;
            ICommand fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.Equal(1, executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(2, executionCount);
        }

        /// <summary>
        /// Test that invokes the command against ICommand invokes the command.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstNullableICommandInvokesTheCommand()
        {
            var executionCount = 0;
            ICommand? fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.Equal(1, executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(2, executionCount);
        }

        /// <summary>
        /// Test that invokes the command against ICommand passes the specified value to can execute and execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandPassesTheSpecifiedValueToCanExecuteAndExecute()
        {
            FakeCommand fixture = new();
            Subject<int> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(42);
            Assert.Equal(42, fixture.CanExecuteParameter);
            Assert.Equal(42, fixture.ExecuteParameter);
        }

        /// <summary>
        /// Test that invokes the command against ICommand respects can execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandRespectsCanExecute()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ICommand fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(Unit.Default);
            Assert.True(executed);
        }

        /// <summary>
        /// Test that invokes the command against ICommand respects can execute window.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandRespectsCanExecuteWindow()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ICommand fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        /// <summary>
        /// Test that invokes the command against ICommand swallows exceptions.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstICommandSwallowsExceptions()
        {
            var count = 0;
            ReactiveCommand<Unit, Unit> fixture = ReactiveCommand.Create(
                                                 () =>
                                                 {
                                                     ++count;
                                                     throw new InvalidOperationException();
                                                 },
                                                 outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.Subscribe();
            Subject<Unit> source = new();
            source.InvokeCommand((ICommand)fixture);

            source.OnNext(Unit.Default);
            source.OnNext(Unit.Default);

            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test that invokes the command against reactive command in target invokes the command.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetInvokesTheCommand()
        {
            var executionCount = 0;
            ReactiveCommandHolder fixture = new();
            Subject<int> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);
            fixture.TheCommand = ReactiveCommand.Create<int>(_ => ++executionCount, outputScheduler: ImmediateScheduler.Instance);

            source.OnNext(0);
            Assert.Equal(1, executionCount);

            source.OnNext(0);
            Assert.Equal(2, executionCount);
        }

        /// <summary>
        /// Test that invokes the command against reactive command in target passes the specified value to execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetPassesTheSpecifiedValueToExecute()
        {
            var executeReceived = 0;
            ReactiveCommandHolder fixture = new();
            Subject<int> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);
            fixture.TheCommand = ReactiveCommand.Create<int>(x => executeReceived = x, outputScheduler: ImmediateScheduler.Instance);

            source.OnNext(42);
            Assert.Equal(42, executeReceived);
        }

        /// <summary>
        /// Test that invokes the command against reactive command in target respects can execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecute()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ReactiveCommandHolder fixture = new();
            Subject<int> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);
            fixture.TheCommand = ReactiveCommand.Create<int>(_ => executed = true, canExecute, ImmediateScheduler.Instance);

            source.OnNext(0);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(0);
            Assert.True(executed);
        }

        /// <summary>
        /// Test that invokes the command against reactive command in target respects can execute window.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecuteWindow()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ReactiveCommandHolder fixture = new();
            Subject<int> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);
            fixture.TheCommand = ReactiveCommand.Create<int>(_ => executed = true, canExecute, ImmediateScheduler.Instance);

            source.OnNext(0);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        /// <summary>
        /// Test that invokes the command against reactive command in target swallows exceptions.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetSwallowsExceptions()
        {
            var count = 0;
            ReactiveCommandHolder fixture = new()
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
            Subject<int> source = new();
            source.InvokeCommand(fixture, x => x.TheCommand!);

            source.OnNext(0);
            source.OnNext(0);

            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test that invokes the command against reactive command invokes the command.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInvokesTheCommand()
        {
            var executionCount = 0;
            ReactiveCommand<Unit, int> fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.Equal(1, executionCount);

            source.OnNext(Unit.Default);
            Assert.Equal(2, executionCount);
        }

        /// <summary>
        /// Test that invokes the command against reactive command passes the specified value to execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandPassesTheSpecifiedValueToExecute()
        {
            var executeReceived = 0;
            ReactiveCommand<int, Unit> fixture = ReactiveCommand.Create<int>(x => executeReceived = x, outputScheduler: ImmediateScheduler.Instance);
            Subject<int> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(42);
            Assert.Equal(42, executeReceived);
        }

        /// <summary>
        /// Test that invokes the command against reactive command respects can execute.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandRespectsCanExecute()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ReactiveCommand<Unit, bool> fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            canExecute.OnNext(true);
            source.OnNext(Unit.Default);
            Assert.True(executed);
        }

        /// <summary>
        /// Test that invokes the command against reactive command respects can execute window.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandRespectsCanExecuteWindow()
        {
            var executed = false;
            BehaviorSubject<bool> canExecute = new(false);
            ReactiveCommand<Unit, bool> fixture = ReactiveCommand.Create(() => executed = true, canExecute, outputScheduler: ImmediateScheduler.Instance);
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            Assert.False(executed);

            // The execution window re-opens, but the above execution request should not be instigated because
            // it occurred when the window was closed. Execution requests do not queue up when the window is closed.
            canExecute.OnNext(true);
            Assert.False(executed);
        }

        /// <summary>
        /// Test that invokes the command against reactive command swallows exceptions.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandSwallowsExceptions()
        {
            var count = 0;
            ReactiveCommand<Unit, Unit> fixture = ReactiveCommand.Create(
                                                 () =>
                                                 {
                                                     ++count;
                                                     throw new InvalidOperationException();
                                                 },
                                                 outputScheduler: ImmediateScheduler.Instance);
            fixture.ThrownExceptions.Subscribe();
            Subject<Unit> source = new();
            source.InvokeCommand(fixture);

            source.OnNext(Unit.Default);
            source.OnNext(Unit.Default);

            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test that invokes the command works even if the source is cold.
        /// </summary>
        [Fact]
        public void InvokeCommandWorksEvenIfTheSourceIsCold()
        {
            var executionCount = 0;
            ReactiveCommand<Unit, int> fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
            var source = Observable.Return(Unit.Default);
            source.InvokeCommand(fixture);

            Assert.Equal(1, executionCount);
        }

        /// <summary>
        /// Test that determines whether [is executing is behavioral].
        /// </summary>
        [Fact]
        public void IsExecutingIsBehavioral()
        {
            ReactiveCommand<Unit, IObservable<Unit>> fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
            fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

            Assert.Equal(1, isExecuting.Count);
            Assert.False(isExecuting[0]);
        }

        /// <summary>
        /// Test that determines whether [is executing remains true as long as execution pipeline has not completed].
        /// </summary>
        [Fact]
        public void IsExecutingRemainsTrueAsLongAsExecutionPipelineHasNotCompleted()
        {
            Subject<Unit> execute = new();
            ReactiveCommand<Unit, Unit> fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: ImmediateScheduler.Instance);

            fixture.Execute().Subscribe();

            Assert.True(fixture.IsExecuting.FirstAsync().Wait());

            execute.OnNext(Unit.Default);
            Assert.True(fixture.IsExecuting.FirstAsync().Wait());

            execute.OnNext(Unit.Default);
            Assert.True(fixture.IsExecuting.FirstAsync().Wait());

            execute.OnCompleted();
            Assert.False(fixture.IsExecuting.FirstAsync().Wait());
        }

        /// <summary>
        /// Test that determines whether [is executing ticks as executions progress].
        /// </summary>
        [Fact]
        public void IsExecutingTicksAsExecutionsProgress() =>
            new TestScheduler().With(
                scheduler =>
                {
                    var execute = Observables.Unit.Delay(TimeSpan.FromSeconds(1), scheduler);
                    ReactiveCommand<Unit, Unit> fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                    fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

                    fixture.Execute().Subscribe();
                    scheduler.AdvanceByMs(100);

                    Assert.Equal(2, isExecuting.Count);
                    Assert.False(isExecuting[0]);
                    Assert.True(isExecuting[1]);

                    scheduler.AdvanceByMs(901);

                    Assert.Equal(3, isExecuting.Count);
                    Assert.False(isExecuting[2]);
                });

        /// <summary>
        /// Results the is ticked through specified scheduler.
        /// </summary>
        [Fact]
        public void ResultIsTickedThroughSpecifiedScheduler() =>
            new TestScheduler().With(
                scheduler =>
                {
                    ReactiveCommand<Unit, IObservable<Unit>> fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: scheduler);
                    fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

                    fixture.Execute().Subscribe();
                    Assert.Empty(results);

                    scheduler.AdvanceByMs(1);
                    Assert.Equal(1, results.Count);
                });

        /// <summary>
        /// Synchronouses the command execute lazily.
        /// </summary>
        [Fact]
        public void SynchronousCommandExecuteLazily()
        {
            var executionCount = 0;
#pragma warning disable IDE0053 // Use expression body for lambda expressions
#pragma warning disable RCS1021 // Convert lambda expression body to expression-body.
            ReactiveCommand<Unit, Unit> fixture1 = ReactiveCommand.Create(() => { ++executionCount; }, outputScheduler: ImmediateScheduler.Instance);
            ReactiveCommand<int, Unit> fixture2 = ReactiveCommand.Create<int>(_ => { ++executionCount; }, outputScheduler: ImmediateScheduler.Instance);
#pragma warning restore RCS1021 // Convert lambda expression body to expression-body.
#pragma warning restore IDE0053 // Use expression body for lambda expressions
            ReactiveCommand<Unit, int> fixture3 = ReactiveCommand.Create(
                                                  () =>
                                                  {
                                                      ++executionCount;
                                                      return 42;
                                                  },
                                                  outputScheduler: ImmediateScheduler.Instance);
            ReactiveCommand<int, int> fixture4 = ReactiveCommand.Create<int, int>(
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

        /// <summary>
        /// Synchronouses the commands fail correctly.
        /// </summary>
        [Fact]
        public void SynchronousCommandsFailCorrectly()
        {
            ReactiveCommand<Unit, Unit> fixture1 = ReactiveCommand.Create(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
            ReactiveCommand<int, Unit> fixture2 = ReactiveCommand.Create<int>(_ => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
            ReactiveCommand<Unit, Unit> fixture3 = ReactiveCommand.Create(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
            ReactiveCommand<int, int> fixture4 = ReactiveCommand.Create<int, int>(_ => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);

            var failureCount = 0;
            Observable.Merge(fixture1.ThrownExceptions, fixture2.ThrownExceptions, fixture3.ThrownExceptions, fixture4.ThrownExceptions).Subscribe(_ => ++failureCount);

            fixture1.Execute().Subscribe(_ => { }, _ => { });
            Assert.Equal(1, failureCount);

            fixture2.Execute().Subscribe(_ => { }, _ => { });
            Assert.Equal(2, failureCount);

            fixture3.Execute().Subscribe(_ => { }, _ => { });
            Assert.Equal(3, failureCount);

            fixture4.Execute().Subscribe(_ => { }, _ => { });
            Assert.Equal(4, failureCount);
        }
    }
}
