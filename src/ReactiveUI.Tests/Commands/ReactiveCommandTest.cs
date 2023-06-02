// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
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
            var canExecuteSubject = new Subject<bool>();
            ICommand? fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
            var canExecuteChanged = new List<bool>();
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
            var canExecuteSubject = new Subject<bool>();
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
            var fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
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
                    var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
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
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
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
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);

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
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
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
            var canExecuteSubject = new Subject<bool>();
            var fixture = ReactiveCommand.Create(() => Observables.Unit, canExecuteSubject, ImmediateScheduler.Instance);
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
            var fixture = ReactiveCommand.CreateFromTask(() => Task.FromResult(13), outputScheduler: ImmediateScheduler.Instance);
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
            var fixture = ReactiveCommand.CreateFromTask<int, int>(param => Task.FromResult(param + 1), outputScheduler: ImmediateScheduler.Instance);
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
        /// Creates the throws if execution parameter is null.
        /// </summary>
        [Fact]
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
        [Fact]
        public void ExceptionsAreDeliveredOnOutputScheduler() =>
            new TestScheduler().With(
                scheduler =>
                {
                    var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()), outputScheduler: scheduler);
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
                    var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
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
            var fixture = ReactiveCommand.CreateFromObservable(() => new[] { 1, 2, 3 }.ToObservable(), outputScheduler: ImmediateScheduler.Instance);
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
                    var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
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
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
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
                    var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
                    var executed = false;

                    fixture.Execute().ObserveOn(scheduler).Subscribe(_ => executed = true);

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
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException()), outputScheduler: ImmediateScheduler.Instance);
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
            var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
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
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Throw<Unit>(new InvalidOperationException("oops")), outputScheduler: ImmediateScheduler.Instance);
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
            var fixture = ReactiveCommand.CreateFromObservable<Unit>(() => throw new InvalidOperationException("oops"), outputScheduler: ImmediateScheduler.Instance);
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
            var fixture = ReactiveCommand.CreateFromObservable(() => Observable.Return(num), outputScheduler: ImmediateScheduler.Instance);
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
            var fixture = new ICommandHolder();
            var source = new Subject<Unit>();
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
            var fixture = new ICommandHolder();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x!.TheCommand!);
            var command = new FakeCommand();
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
            var fixture = new ICommandHolder();
            var source = new Subject<int>();
            source.InvokeCommand(fixture, x => x.TheCommand);
            var command = new FakeCommand();
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
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ICommandHolder();
            var source = new Subject<Unit>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ICommandHolder();
            var source = new Subject<Unit>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ICommandHolder();
            var source = new Subject<Unit>();
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
            var source = new Subject<Unit>();
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
            var source = new Subject<Unit>();
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
            var fixture = new FakeCommand();
            var source = new Subject<int>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            ICommand fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
            var source = new Subject<Unit>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            ICommand fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
            var source = new Subject<Unit>();
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

            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test that invokes the command against reactive command in target invokes the command.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInTargetInvokesTheCommand()
        {
            var executionCount = 0;
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
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
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = new ReactiveCommandHolder();
            var source = new Subject<int>();
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

            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test that invokes the command against reactive command invokes the command.
        /// </summary>
        [Fact]
        public void InvokeCommandAgainstReactiveCommandInvokesTheCommand()
        {
            var executionCount = 0;
            var fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
            var source = new Subject<Unit>();
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
            var fixture = ReactiveCommand.Create<int>(x => executeReceived = x, outputScheduler: ImmediateScheduler.Instance);
            var source = new Subject<int>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = ReactiveCommand.Create(() => executed = true, canExecute, ImmediateScheduler.Instance);
            var source = new Subject<Unit>();
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
            var canExecute = new BehaviorSubject<bool>(false);
            var fixture = ReactiveCommand.Create(() => executed = true, canExecute, outputScheduler: ImmediateScheduler.Instance);
            var source = new Subject<Unit>();
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

            Assert.Equal(2, count);
        }

        /// <summary>
        /// Test that invokes the command works even if the source is cold.
        /// </summary>
        [Fact]
        public void InvokeCommandWorksEvenIfTheSourceIsCold()
        {
            var executionCount = 0;
            var fixture = ReactiveCommand.Create(() => ++executionCount, outputScheduler: ImmediateScheduler.Instance);
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
            var fixture = ReactiveCommand.Create(() => Observables.Unit, outputScheduler: ImmediateScheduler.Instance);
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
            var execute = new Subject<Unit>();
            var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: ImmediateScheduler.Instance);

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
                    var fixture = ReactiveCommand.CreateFromObservable(() => execute, outputScheduler: scheduler);
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
            new TestScheduler().WithAsync(
                scheduler =>
                {
                    var fixture = ReactiveCommand.CreateRunInBackground(() => Observables.Unit, outputScheduler: scheduler);
                    fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

                    fixture.Execute().Subscribe();
                    Assert.Empty(results);

                    scheduler.AdvanceByMs(1);
                    Assert.Equal(1, results.Count);
                    return Task.CompletedTask;
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
            var fixture1 = ReactiveCommand.Create(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
            var fixture2 = ReactiveCommand.Create<int>(_ => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
            var fixture3 = ReactiveCommand.Create(() => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);
            var fixture4 = ReactiveCommand.Create<int, int>(_ => throw new InvalidOperationException(), outputScheduler: ImmediateScheduler.Instance);

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

        [Fact]
        public async Task ReactiveCommandCreateFromTaskHandlesTaskExceptionAsync()
        {
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

            fixture.IsExecuting.Subscribe(x => isExecuting = x);
            fixture.ThrownExceptions.Subscribe(ex => fail = ex);

            Assert.False(isExecuting);
            Assert.Null(fail);

            fixture.Execute().Subscribe();
            Assert.True(isExecuting);
            Assert.Null(fail);

            subj.OnNext(Unit.Default);

            // Wait 10 ms to allow execution to complete
            await Task.Delay(500).ConfigureAwait(false);

            Assert.False(isExecuting);
            Assert.Equal("break execution", fail?.Message);
        }

        [Fact]
        public async Task ReactiveCommandCreateFromTaskThenCancelSetsIsExecutingFalseOnlyAfterCancellationCompleteAsync()
        {
            // This tests for the problem described at https://github.com/reactiveui/ReactiveUI/issues/2153
            // The exact sequence of events is important here. In particular, we need the test to be able
            // to make observations while a task is in progress, while it is in the process of being cancelled,
            // and after it has finished. This requires some careful sequencing. The System.Threading.Barrier
            // class is designed for managing precisely this kind lock-step progress. Unfortunately, it
            // doesn't directly intrinsically support async/await. Its SignalAndWait blocks the calling
            // thread, which is a problem for async UI code, since that typically uses a single thread for
            // most work. Calling SignalAndWait on the UI thread (or in this case, the test thread, which
            // is effectively a stand-in for the UI thread) deadlocks, because the matching call to
            // SignalAndWait that it's waiting can't happen until the UI thread becomes available.
            // So we wrap the use of this in an async-friendly helper that calls SignalAndWait on a
            // thread pool thread.
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.threading.asyncbarrier
            // would arguably be a better solution, but due to some slightly unfortunate accidents of
            // history, it and a whole load of other highly useful async synchronization primitives
            // ended up in a DLL whose name makes it sound a lot like it will only work in Visual Studio.
            // I didn't want to be the one to introduce a dependency on that component, hence this
            // ad hoc wrapper instead, but I would recommend at least considering using the very
            // misleadingly-named https://www.nuget.org/packages/Microsoft.VisualStudio.Threading.
            using var phaseSync = new Barrier(2);
            Task AwaitTestPhaseAsync() => Task.Run(() => phaseSync.SignalAndWait(CancellationToken.None));

            var fixture = ReactiveCommand.CreateFromTask(async (token) =>
            {
                // Phase 1: command execution has begun.
                await AwaitTestPhaseAsync();

                Debug.WriteLine("started command");
                try
                {
                    await Task.Delay(10000, token);
                }
                catch (OperationCanceledException)
                {
                    // Phase 2: command task has detected cancellation request.
                    await AwaitTestPhaseAsync();

                    // Phase 3: test has observed IsExecuting while cancellation is in progress.
                    await AwaitTestPhaseAsync();

                    ////Debug.WriteLine("starting cancelling command");
                    ////await Task.Delay(5000, CancellationToken.None);
                    ////Debug.WriteLine("finished cancelling  command");
                    throw;
                }

                Debug.WriteLine("finished command");
            });

            // This test needs to check the latest value emitted by IsExecuting at various points.
            // The obvious way to do this would be with "await fixture.IsExecuting", but that ends
            // up involving various bits of Rx scheduling machinery, which can interfere with the
            // sequencing this test requires. (For example, "await fixture.IsExecuting" can end up
            // waiting until after the entire Task we're testing here has actually completed!)
            // So we just keep a variable up to date with the most recently observed value, enabling
            // the test to inspect that at any time without an await.
            var latestIsExecutingValue = false;
            fixture.IsExecuting.Subscribe(isExecuting =>
            {
                Debug.WriteLine($"command executing = {isExecuting}");
                Volatile.Write(ref latestIsExecutingValue, isExecuting);
            });

            var disposable = fixture.Execute().Subscribe();

            // Phase 1: command execution has begun.
            await AwaitTestPhaseAsync();

            Assert.True(Volatile.Read(ref latestIsExecutingValue), "IsExecuting should be true when execution is underway");

            disposable.Dispose();

            // Phase 2: command task has detected cancellation request.
            await AwaitTestPhaseAsync();

            Assert.True(Volatile.Read(ref latestIsExecutingValue), "IsExecuting should remain true while cancellation is in progress");

            // Phase 3: test has observed IsExecuting while cancellation is in progress.
            await AwaitTestPhaseAsync();

            // Finally, we need to wait for the task to complete. We can't directly observe this,
            // because once the task has actually completed, it can't give us any sort of notification.
            // If it were able to do something to notify us, then that would mean it was still
            // running.
            // So instead, we're just going to wait for IsExecuting to become false.
            var start = Environment.TickCount;
            while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
            {
                await Task.Yield();
            }

            Assert.False(Volatile.Read(ref latestIsExecutingValue), "IsExecuting should be false once cancellation completes");
        }

        [Fact]
        public async Task ReactiveCommandExecutesFromInvokeCommand()
        {
            var semaphore = new SemaphoreSlim(0);
            var command = ReactiveCommand.Create(() => semaphore.Release());

            Observable.Return(Unit.Default)
                      .InvokeCommand(command);

            var result = 0;
            var task = semaphore.WaitAsync();
            if (await Task.WhenAny(Task.Delay(TimeSpan.FromMilliseconds(100)), task).ConfigureAwait(true) == task)
            {
                result = 1;
            }
            else
            {
                result = -1;
            }

            await Task.Delay(200).ConfigureAwait(false);
            Assert.Equal(1, result);
        }
    }
}
