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
[TestFixture]
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
        ICommand fixture =
            ReactiveCommand.Create(
                                   () => Observables.Unit,
                                   canExecuteSubject,
                                   ImmediateScheduler.Instance);
        var canExecuteChanged = new List<bool>();
        fixture.CanExecuteChanged += (_, __) => canExecuteChanged.Add(fixture.CanExecute(null));

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        canExecuteChanged,
                        Has.Count.EqualTo(2));
            Assert.That(
                        canExecuteChanged[0],
                        Is.True);
            Assert.That(
                        canExecuteChanged[1],
                        Is.False);
        }
    }

    /// <summary>
    /// A test that determines whether this instance [can execute is available via ICommand].
    /// </summary>
    [Test]
    public void CanExecuteIsAvailableViaICommand()
    {
        var canExecuteSubject = new Subject<bool>();
        ICommand fixture =
            ReactiveCommand.Create(
                                   static () => Observables.Unit,
                                   canExecuteSubject,
                                   ImmediateScheduler.Instance);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        fixture.CanExecute(null),
                        Is.False);

            canExecuteSubject.OnNext(true);
            Assert.That(
                        fixture.CanExecute(null),
                        Is.True);

            canExecuteSubject.OnNext(false);
            Assert.That(
                        fixture.CanExecute(null),
                        Is.False);
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is behavioral].
    /// </summary>
    [Test]
    public void CanExecuteIsBehavioral()
    {
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             outputScheduler: ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        canExecute,
                        Has.Count.EqualTo(1));
            Assert.That(
                        canExecute[0],
                        Is.True);
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is false if already executing].
    /// </summary>
    [Test]
    public void CanExecuteIsFalseIfAlreadyExecuting() =>
        new TestScheduler().With(scheduler =>
        {
            var execute = Observables.Unit.Delay(
                                                 TimeSpan.FromSeconds(1),
                                                 scheduler);
            var fixture = ReactiveCommand.CreateFromObservable(
                                                               () => execute,
                                                               outputScheduler: scheduler);
            fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

            fixture.Execute().Subscribe();
            scheduler.AdvanceByMs(100);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            canExecute,
                            Has.Count.EqualTo(2));
                Assert.That(
                            canExecute[1],
                            Is.False);
            }

            scheduler.AdvanceByMs(901);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            canExecute,
                            Has.Count.EqualTo(3));
                Assert.That(
                            canExecute[2],
                            Is.True);
            }
        });

    /// <summary>
    /// Test that determines whether this instance [can execute is false if caller dictates as such].
    /// </summary>
    [Test]
    public void CanExecuteIsFalseIfCallerDictatesAsSuch()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             canExecuteSubject,
                                             ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        canExecute,
                        Has.Count.EqualTo(3));
            Assert.That(
                        canExecute[0],
                        Is.False);
            Assert.That(
                        canExecute[1],
                        Is.True);
            Assert.That(
                        canExecute[2],
                        Is.False);
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is unsubscribed after command disposal].
    /// </summary>
    [Test]
    public void CanExecuteIsUnsubscribedAfterCommandDisposal()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             canExecuteSubject,
                                             ImmediateScheduler.Instance);

        Assert.That(
                    canExecuteSubject.HasObservers,
                    Is.True);

        fixture.Dispose();

        Assert.That(
                    canExecuteSubject.HasObservers,
                    Is.False);
    }

    /// <summary>
    /// Test that determines whether this instance [can execute only ticks distinct values].
    /// </summary>
    [Test]
    public void CanExecuteOnlyTicksDistinctValues()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             canExecuteSubject,
                                             ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(false);
        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        canExecute,
                        Has.Count.EqualTo(2));
            Assert.That(
                        canExecute[0],
                        Is.False);
            Assert.That(
                        canExecute[1],
                        Is.True);
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute ticks failures through thrown exceptions].
    /// </summary>
    [Test]
    public void CanExecuteTicksFailuresThroughThrownExceptions()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             canExecuteSubject,
                                             ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions)
               .Subscribe();

        canExecuteSubject.OnError(new InvalidOperationException("oops"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        thrownExceptions,
                        Has.Count.EqualTo(1));
            Assert.That(
                        thrownExceptions[0].Message,
                        Is.EqualTo("oops"));
        }
    }

    /// <summary>
    /// Creates the task facilitates TPL integration.
    /// </summary>
    [Test]
    public void CreateTaskFacilitatesTPLIntegration()
    {
        var fixture =
            ReactiveCommand.CreateFromTask(
                                           static () => Task.FromResult(13),
                                           outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute().Subscribe();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        results,
                        Has.Count.EqualTo(1));
            Assert.That(
                        results[0],
                        Is.EqualTo(13));
        }
    }

    /// <summary>
    /// Creates the task facilitates TPL integration with parameter.
    /// </summary>
    [Test]
    public void CreateTaskFacilitatesTPLIntegrationWithParameter()
    {
        var fixture =
            ReactiveCommand.CreateFromTask<int, int>(
                                                     static param => Task.FromResult(param + 1),
                                                     outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute(3).Subscribe();
        fixture.Execute(41).Subscribe();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        results,
                        Has.Count.EqualTo(2));
            Assert.That(
                        results[0],
                        Is.EqualTo(4));
            Assert.That(
                        results[1],
                        Is.EqualTo(42));
        }
    }

    /// <summary>
    /// Creates the throws if execution parameter is null.
    /// </summary>
    [Test]
    public void CreateThrowsIfExecutionParameterIsNull()
    {
#pragma warning disable CS8625
#pragma warning disable CS8600
        Assert.That(
                    static () => ReactiveCommand.Create(null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.Create((Func<Unit>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.Create((Action<Unit>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.Create((Func<Unit, Unit>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.Create((Func<IObservable<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.Create((Func<Task<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.Create((Func<Unit, IObservable<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.Create((Func<Unit, Task<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
#pragma warning restore CS8600
#pragma warning restore CS8625
    }

    /// <summary>
    /// Creates the throws if execution parameter is null (RunInBackground).
    /// </summary>
    [Test]
    public void CreateRunInBackgroundThrowsIfExecutionParameterIsNull()
    {
#pragma warning disable CS8625
#pragma warning disable CS8600
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground(null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground((Func<Unit>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground((Action<Unit>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground((Func<Unit, Unit>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground((Func<IObservable<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground((Func<Task<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground((Func<Unit, IObservable<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
        Assert.That(
                    static () => ReactiveCommand.CreateRunInBackground((Func<Unit, Task<Unit>>)null),
                    Throws.TypeOf<ArgumentNullException>());
#pragma warning restore CS8600
#pragma warning restore CS8625
    }

    /// <summary>
    /// Exceptions are delivered on output scheduler.
    /// </summary>
    [Test]
    public void ExceptionsAreDeliveredOnOutputScheduler() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture =
                ReactiveCommand.CreateFromObservable(
                                                     () => Observable.Throw<Unit>(new InvalidOperationException()),
                                                     outputScheduler: scheduler);
            Exception? exception = null;
            fixture.ThrownExceptions.Subscribe(ex => exception = ex);
            fixture.Execute().Subscribe(
                                        _ => { },
                                        _ => { });

            Assert.That(
                        exception,
                        Is.Null);
            scheduler.Start();
            Assert.That(
                        exception,
                        Is.TypeOf<InvalidOperationException>());
        });

    /// <summary>
    /// Executes can be cancelled.
    /// </summary>
    [Test]
    public void ExecuteCanBeCancelled() =>
        new TestScheduler().With(scheduler =>
        {
            var execute = Observables.Unit.Delay(
                                                 TimeSpan.FromSeconds(1),
                                                 scheduler);
            var fixture = ReactiveCommand.CreateFromObservable(
                                                               () => execute,
                                                               outputScheduler: scheduler);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

            var sub1 = fixture.Execute().Subscribe();
            var sub2 = fixture.Execute().Subscribe();
            scheduler.AdvanceByMs(999);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            fixture.IsExecuting.FirstAsync().Wait(),
                            Is.True);
                Assert.That(
                            executed,
                            Is.Empty);
            }

            sub1.Dispose();

            scheduler.AdvanceByMs(2);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            executed,
                            Has.Count.EqualTo(1));
                Assert.That(
                            fixture.IsExecuting.FirstAsync().Wait(),
                            Is.False);
            }
        });

    /// <summary>
    /// Executes can tick through multiple results.
    /// </summary>
    [Test]
    public void ExecuteCanTickThroughMultipleResults()
    {
        var fixture = ReactiveCommand.CreateFromObservable(
                                                           static () => new[] { 1, 2, 3 }.ToObservable(),
                                                           outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute().Subscribe();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        results,
                        Has.Count.EqualTo(3));
            Assert.That(
                        results[0],
                        Is.EqualTo(1));
            Assert.That(
                        results[1],
                        Is.EqualTo(2));
            Assert.That(
                        results[2],
                        Is.EqualTo(3));
        }
    }

    /// <summary>
    /// Executes facilitates any number of in flight executions.
    /// </summary>
    [Test]
    public void ExecuteFacilitatesAnyNumberOfInFlightExecutions() =>
        new TestScheduler().With(scheduler =>
        {
            var execute = Observables.Unit.Delay(
                                                 TimeSpan.FromMilliseconds(500),
                                                 scheduler);
            var fixture = ReactiveCommand.CreateFromObservable(
                                                               () => execute,
                                                               outputScheduler: scheduler);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var executed).Subscribe();

            var sub1 = fixture.Execute().Subscribe();
            var sub2 = fixture.Execute().Subscribe();
            scheduler.AdvanceByMs(100);

            var sub3 = fixture.Execute().Subscribe();
            scheduler.AdvanceByMs(200);
            var sub4 = fixture.Execute().Subscribe();
            scheduler.AdvanceByMs(100);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            fixture.IsExecuting.FirstAsync().Wait(),
                            Is.True);
                Assert.That(
                            executed,
                            Is.Empty);
            }

            scheduler.AdvanceByMs(101);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            executed,
                            Has.Count.EqualTo(2));
                Assert.That(
                            fixture.IsExecuting.FirstAsync().Wait(),
                            Is.True);
            }

            scheduler.AdvanceByMs(200);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            executed,
                            Has.Count.EqualTo(3));
                Assert.That(
                            fixture.IsExecuting.FirstAsync().Wait(),
                            Is.True);
            }

            scheduler.AdvanceByMs(100);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            executed,
                            Has.Count.EqualTo(4));
                Assert.That(
                            fixture.IsExecuting.FirstAsync().Wait(),
                            Is.False);
            }
        });

    /// <summary>
    /// Execute is available via ICommand.
    /// </summary>
    [Test]
    public void ExecuteIsAvailableViaICommand()
    {
        var executed = false;
        ICommand fixture = ReactiveCommand.Create(
                                                  () =>
                                                  {
                                                      executed = true;
                                                      return Observables.Unit;
                                                  },
                                                  outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute(null);
        Assert.That(
                    executed,
                    Is.True);
    }

    /// <summary>
    /// Execute passes through parameter.
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        parameters,
                        Has.Count.EqualTo(3));
            Assert.That(
                        parameters[0],
                        Is.EqualTo(1));
            Assert.That(
                        parameters[1],
                        Is.EqualTo(42));
            Assert.That(
                        parameters[2],
                        Is.EqualTo(348));
        }
    }

    /// <summary>
    /// Execute re-enables execution even after failure.
    /// </summary>
    [Test]
    public void ExecuteReenablesExecutionEvenAfterFailure()
    {
        var fixture =
            ReactiveCommand.CreateFromObservable(
                                                 static () => Observable.Throw<Unit>(new InvalidOperationException("oops")),
                                                 outputScheduler: ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions)
               .Subscribe();

        fixture.Execute().Subscribe(
                                    static _ => { },
                                    static _ => { });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        thrownExceptions,
                        Has.Count.EqualTo(1));
            Assert.That(
                        thrownExceptions[0].Message,
                        Is.EqualTo("oops"));
            Assert.That(
                        canExecute,
                        Has.Count.EqualTo(3));
            Assert.That(
                        canExecute[0],
                        Is.True);
            Assert.That(
                        canExecute[1],
                        Is.False);
            Assert.That(
                        canExecute[2],
                        Is.True);
        }
    }

    /// <summary>
    /// Execute result is delivered on specified scheduler.
    /// </summary>
    [Test]
    public void ExecuteResultIsDeliveredOnSpecifiedScheduler() =>
        new TestScheduler().With(scheduler =>
        {
            var execute = Observables.Unit;
            var fixture = ReactiveCommand.CreateFromObservable(
                                                               () => execute,
                                                               outputScheduler: scheduler);
            var executed = false;

            fixture.Execute().ObserveOn(scheduler).Subscribe(_ => executed = true);

            Assert.That(
                        executed,
                        Is.False);
            scheduler.AdvanceByMs(1);
            Assert.That(
                        executed,
                        Is.True);
        });

    /// <summary>
    /// Execute ticks any exception.
    /// </summary>
    [Test]
    public void ExecuteTicksAnyException()
    {
        var fixture =
            ReactiveCommand.CreateFromObservable(
                                                 () => Observable.Throw<Unit>(new InvalidOperationException()),
                                                 outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.Subscribe();
        Exception? exception = null;
        fixture.Execute().Subscribe(
                                    _ => { },
                                    ex => exception = ex,
                                    () => { });

        Assert.That(
                    exception,
                    Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Execute ticks any lambda exception.
    /// </summary>
    [Test]
    public void ExecuteTicksAnyLambdaException()
    {
        var fixture = ReactiveCommand.CreateFromObservable<Unit>(
                                                                 () => throw new InvalidOperationException(),
                                                                 outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.Subscribe();
        Exception? exception = null;
        fixture.Execute().Subscribe(
                                    _ => { },
                                    ex => exception = ex,
                                    () => { });

        Assert.That(
                    exception,
                    Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Execute ticks errors through thrown exceptions.
    /// </summary>
    [Test]
    public void ExecuteTicksErrorsThroughThrownExceptions()
    {
        var fixture =
            ReactiveCommand.CreateFromObservable(
                                                 static () => Observable.Throw<Unit>(new InvalidOperationException("oops")),
                                                 outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions)
               .Subscribe();

        fixture.Execute().Subscribe(
                                    static _ => { },
                                    static _ => { });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        thrownExceptions,
                        Has.Count.EqualTo(1));
            Assert.That(
                        thrownExceptions[0].Message,
                        Is.EqualTo("oops"));
        }
    }

    /// <summary>
    /// Execute ticks lambda errors through thrown exceptions.
    /// </summary>
    [Test]
    public void ExecuteTicksLambdaErrorsThroughThrownExceptions()
    {
        var fixture = ReactiveCommand.CreateFromObservable<Unit>(
                                                                 static () => throw new InvalidOperationException("oops"),
                                                                 outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions)
               .Subscribe();

        fixture.Execute().Subscribe(
                                    static _ => { },
                                    static _ => { });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        thrownExceptions,
                        Has.Count.EqualTo(1));
            Assert.That(
                        thrownExceptions[0].Message,
                        Is.EqualTo("oops"));
            Assert.That(
                        fixture.CanExecute.FirstAsync().Wait(),
                        Is.True);
        }
    }

    /// <summary>
    /// Execute ticks through the result.
    /// </summary>
    [Test]
    public void ExecuteTicksThroughTheResult()
    {
        var num = 0;
        var fixture =
            ReactiveCommand.CreateFromObservable(
                                                 () => Observable.Return(num),
                                                 outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        num = 1;
        fixture.Execute().Subscribe();
        num = 10;
        fixture.Execute().Subscribe();
        num = 30;
        fixture.Execute().Subscribe();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        results,
                        Has.Count.EqualTo(3));
            Assert.That(
                        results[0],
                        Is.EqualTo(1));
            Assert.That(
                        results[1],
                        Is.EqualTo(10));
            Assert.That(
                        results[2],
                        Is.EqualTo(30));
        }
    }

    /// <summary>
    /// Execute via ICommand throws if parameter type is incorrect.
    /// </summary>
    [Test]
    public void ExecuteViaICommandThrowsIfParameterTypeIsIncorrect()
    {
        ICommand fixture = ReactiveCommand.Create<int>(
                                                       _ => { },
                                                       outputScheduler: ImmediateScheduler.Instance);
        var ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute("foo"));
        Assert.That(
                    ex!.Message,
                    Is.EqualTo(
                               "Command requires parameters of type System.Int32, but received parameter of type System.String."));

        fixture = ReactiveCommand.Create<string>(_ => { });
        ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute(13));
        Assert.That(
                    ex!.Message,
                    Is.EqualTo(
                               "Command requires parameters of type System.String, but received parameter of type System.Int32."));
    }

    /// <summary>
    /// Execute via ICommand works with nullable types.
    /// </summary>
    [Test]
    public void ExecuteViaICommandWorksWithNullableTypes()
    {
        int? value = null;
        ICommand fixture =
            ReactiveCommand.Create<int?>(
                                         param => value = param,
                                         outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute(42);
        Assert.That(
                    value,
                    Is.EqualTo(42));

        fixture.Execute(null);
        Assert.That(
                    value,
                    Is.Null);
    }

    /// <summary>
    /// Invoke command against ICommand in target invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetInvokesTheCommand()
    {
        var executionCount = 0;
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);
        fixture.TheCommand =
            ReactiveCommand.Create(
                                   () => ++executionCount,
                                   outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against ICommand in target passes the specified value.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new ICommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             static x => x!.TheCommand!);
        var command = new FakeCommand();
        fixture.TheCommand = command;

        source.OnNext(42);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        command.CanExecuteParameter,
                        Is.EqualTo(42));
            Assert.That(
                        command.ExecuteParameter,
                        Is.EqualTo(42));
        }
    }

    /// <summary>
    /// Invoke command against nullable ICommand in target passes the specified value.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInNullableTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new ICommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             static x => x.TheCommand);
        var command = new FakeCommand();
        fixture.TheCommand = command;

        source.OnNext(42);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        command.CanExecuteParameter,
                        Is.EqualTo(42));
            Assert.That(
                        command.ExecuteParameter,
                        Is.EqualTo(42));
        }
    }

    /// <summary>
    /// Invoke command against ICommand in target respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create(
                                                    () => executed = true,
                                                    canExecute,
                                                    ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.True);
    }

    /// <summary>
    /// Invoke command against nullable target respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInNullableTargetRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand);
        fixture.TheCommand = ReactiveCommand.Create(
                                                    () => executed = true,
                                                    canExecute,
                                                    ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.True);
    }

    /// <summary>
    /// Invoke command against ICommand in target respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInTargetRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ICommandHolder();
        var source = new Subject<Unit>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create(
                                                    () => executed = true,
                                                    canExecute,
                                                    ImmediateScheduler.Instance);

        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.False);

        // When the window reopens, previous requests should NOT execute.
        canExecute.OnNext(true);
        Assert.That(
                    executed,
                    Is.False);
    }

    /// <summary>
    /// Invoke command against ICommand in target swallows exceptions.
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
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);

        source.OnNext(Unit.Default);
        source.OnNext(Unit.Default);

        Assert.That(
                    count,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against ICommand invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandInvokesTheCommand()
    {
        var executionCount = 0;
        ICommand fixture = ReactiveCommand.Create(
                                                  () => ++executionCount,
                                                  outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against nullable ICommand invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstNullableICommandInvokesTheCommand()
    {
        var executionCount = 0;
        ICommand fixture =
            ReactiveCommand.Create(
                                   () => ++executionCount,
                                   outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against ICommand passes the specified value.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new FakeCommand();
        var source = new Subject<int>();
        source.InvokeCommand(fixture);

        source.OnNext(42);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        fixture.CanExecuteParameter,
                        Is.EqualTo(42));
            Assert.That(
                        fixture.ExecuteParameter,
                        Is.EqualTo(42));
        }
    }

    /// <summary>
    /// Invoke command against ICommand respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        ICommand fixture = ReactiveCommand.Create(
                                                  () => executed = true,
                                                  canExecute,
                                                  ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.True);
    }

    /// <summary>
    /// Invoke command against ICommand respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstICommandRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        ICommand fixture = ReactiveCommand.Create(
                                                  () => executed = true,
                                                  canExecute,
                                                  ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.False);

        // When the window reopens, previous requests should NOT execute.
        canExecute.OnNext(true);
        Assert.That(
                    executed,
                    Is.False);
    }

    /// <summary>
    /// Invoke command against ICommand swallows exceptions.
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

        Assert.That(
                    count,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against reactive command in target invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetInvokesTheCommand()
    {
        var executionCount = 0;
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);
        fixture.TheCommand =
            ReactiveCommand.Create<int>(
                                        _ => ++executionCount,
                                        outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(0);
        Assert.That(
                    executionCount,
                    Is.EqualTo(1));

        source.OnNext(0);
        Assert.That(
                    executionCount,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against reactive command in target passes the specified value to execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetPassesTheSpecifiedValueToExecute()
    {
        var executeReceived = 0;
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);
        fixture.TheCommand =
            ReactiveCommand.Create<int>(
                                        x => executeReceived = x,
                                        outputScheduler: ImmediateScheduler.Instance);

        source.OnNext(42);
        Assert.That(
                    executeReceived,
                    Is.EqualTo(42));
    }

    /// <summary>
    /// Invoke command against reactive command in target respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create<int>(
                                                         _ => executed = true,
                                                         canExecute,
                                                         ImmediateScheduler.Instance);

        source.OnNext(0);
        Assert.That(
                    executed,
                    Is.False);

        canExecute.OnNext(true);
        source.OnNext(0);
        Assert.That(
                    executed,
                    Is.True);
    }

    /// <summary>
    /// Invoke command against reactive command in target respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = new ReactiveCommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);
        fixture.TheCommand = ReactiveCommand.Create<int>(
                                                         _ => executed = true,
                                                         canExecute,
                                                         ImmediateScheduler.Instance);

        source.OnNext(0);
        Assert.That(
                    executed,
                    Is.False);

        // When the window reopens, previous requests should NOT execute.
        canExecute.OnNext(true);
        Assert.That(
                    executed,
                    Is.False);
    }

    /// <summary>
    /// Invoke command against reactive command in target swallows exceptions.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInTargetSwallowsExceptions()
    {
        var count = 0;
        var fixture = new ReactiveCommandHolder
        {
            TheCommand = ReactiveCommand.Create<int>(
                                                     _ =>
                                                     {
                                                         ++count;
                                                         throw new InvalidOperationException();
                                                     },
                                                     outputScheduler: ImmediateScheduler.Instance)
        };
        fixture.TheCommand!.ThrownExceptions.Subscribe();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             x => x.TheCommand!);

        source.OnNext(0);
        source.OnNext(0);

        Assert.That(
                    count,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against reactive command invokes the command.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandInvokesTheCommand()
    {
        var executionCount = 0;
        var fixture = ReactiveCommand.Create(
                                             () => ++executionCount,
                                             outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(1));

        source.OnNext(Unit.Default);
        Assert.That(
                    executionCount,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command against reactive command passes the specified value.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandPassesTheSpecifiedValueToExecute()
    {
        var executeReceived = 0;
        var fixture =
            ReactiveCommand.Create<int>(
                                        x => executeReceived = x,
                                        outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<int>();
        source.InvokeCommand(fixture);

        source.OnNext(42);
        Assert.That(
                    executeReceived,
                    Is.EqualTo(42));
    }

    /// <summary>
    /// Invoke command against reactive command respects can execute.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandRespectsCanExecute()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = ReactiveCommand.Create(
                                             () => executed = true,
                                             canExecute,
                                             ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.False);

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.True);
    }

    /// <summary>
    /// Invoke command against reactive command respects can execute window.
    /// </summary>
    [Test]
    public void InvokeCommandAgainstReactiveCommandRespectsCanExecuteWindow()
    {
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var fixture = ReactiveCommand.Create(
                                             () => executed = true,
                                             canExecute,
                                             outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        Assert.That(
                    executed,
                    Is.False);

        // When the window reopens, previous requests should NOT execute.
        canExecute.OnNext(true);
        Assert.That(
                    executed,
                    Is.False);
    }

    /// <summary>
    /// Invoke command against reactive command swallows exceptions.
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

        Assert.That(
                    count,
                    Is.EqualTo(2));
    }

    /// <summary>
    /// Invoke command works even if the source is cold.
    /// </summary>
    [Test]
    public void InvokeCommandWorksEvenIfTheSourceIsCold()
    {
        var executionCount = 0;
        var fixture = ReactiveCommand.Create(
                                             () => ++executionCount,
                                             outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return(Unit.Default);
        source.InvokeCommand(fixture);

        Assert.That(
                    executionCount,
                    Is.EqualTo(1));
    }

    /// <summary>
    /// IsExecuting is behavioral.
    /// </summary>
    [Test]
    public void IsExecutingIsBehavioral()
    {
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             outputScheduler: ImmediateScheduler.Instance);
        fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        isExecuting,
                        Has.Count.EqualTo(1));
            Assert.That(
                        isExecuting[0],
                        Is.False);
        }
    }

    /// <summary>
    /// IsExecuting remains true as long as pipeline has not completed.
    /// </summary>
    [Test]
    public void IsExecutingRemainsTrueAsLongAsExecutionPipelineHasNotCompleted()
    {
        var execute = new Subject<Unit>();
        var fixture = ReactiveCommand.CreateFromObservable(
                                                           () => execute,
                                                           outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute().Subscribe();

        Assert.That(
                    fixture.IsExecuting.FirstAsync().Wait(),
                    Is.True);

        execute.OnNext(Unit.Default);
        Assert.That(
                    fixture.IsExecuting.FirstAsync().Wait(),
                    Is.True);

        execute.OnNext(Unit.Default);
        Assert.That(
                    fixture.IsExecuting.FirstAsync().Wait(),
                    Is.True);

        execute.OnCompleted();
        Assert.That(
                    fixture.IsExecuting.FirstAsync().Wait(),
                    Is.False);
    }

    /// <summary>
    /// IsExecuting ticks as executions progress.
    /// </summary>
    [Test]
    public void IsExecutingTicksAsExecutionsProgress() =>
        new TestScheduler().With(scheduler =>
        {
            var execute = Observables.Unit.Delay(
                                                 TimeSpan.FromSeconds(1),
                                                 scheduler);
            var fixture = ReactiveCommand.CreateFromObservable(
                                                               () => execute,
                                                               outputScheduler: scheduler);
            fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting)
                   .Subscribe();

            fixture.Execute().Subscribe();
            scheduler.AdvanceByMs(100);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            isExecuting,
                            Has.Count.EqualTo(2));
                Assert.That(
                            isExecuting[0],
                            Is.False);
                Assert.That(
                            isExecuting[1],
                            Is.True);
            }

            scheduler.AdvanceByMs(901);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            isExecuting,
                            Has.Count.EqualTo(3));
                Assert.That(
                            isExecuting[2],
                            Is.False);
            }
        });

    /// <summary>
    /// Result is ticked through specified scheduler.
    /// </summary>
    [Test]
    public void ResultIsTickedThroughSpecifiedScheduler() =>
        new TestScheduler().WithAsync(static scheduler =>
        {
            var fixture = ReactiveCommand.CreateRunInBackground(
                                                                static () => Observables.Unit,
                                                                outputScheduler: scheduler);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

            fixture.Execute().Subscribe();
            Assert.That(
                        results,
                        Is.Empty);

            scheduler.AdvanceByMs(1);
            Assert.That(
                        results,
                        Has.Count.EqualTo(1));
            return Task.CompletedTask;
        });

    /// <summary>
    /// Synchronous command executes lazily.
    /// </summary>
    [Test]
    public void SynchronousCommandExecuteLazily()
    {
        var executionCount = 0;
        var fixture1 =
            ReactiveCommand.Create(
                                   () => { ++executionCount; },
                                   outputScheduler: ImmediateScheduler.Instance);
        var fixture2 =
            ReactiveCommand.Create<int>(
                                        _ => { ++executionCount; },
                                        outputScheduler: ImmediateScheduler.Instance);
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

        Assert.That(
                    executionCount,
                    Is.Zero);

        execute1.Subscribe();
        Assert.That(
                    executionCount,
                    Is.EqualTo(1));

        execute2.Subscribe();
        Assert.That(
                    executionCount,
                    Is.EqualTo(2));

        execute3.Subscribe();
        Assert.That(
                    executionCount,
                    Is.EqualTo(3));

        execute4.Subscribe();
        Assert.That(
                    executionCount,
                    Is.EqualTo(4));
    }

    /// <summary>
    /// Synchronous commands fail correctly.
    /// </summary>
    [Test]
    public void SynchronousCommandsFailCorrectly()
    {
        var fixture1 = ReactiveCommand.Create(
                                              () => throw new InvalidOperationException(),
                                              outputScheduler: ImmediateScheduler.Instance);
        var fixture2 = ReactiveCommand.Create<int>(
                                                   _ => throw new InvalidOperationException(),
                                                   outputScheduler: ImmediateScheduler.Instance);
        var fixture3 = ReactiveCommand.Create(
                                              () => throw new InvalidOperationException(),
                                              outputScheduler: ImmediateScheduler.Instance);
        var fixture4 = ReactiveCommand.Create<int, int>(
                                                        _ => throw new InvalidOperationException(),
                                                        outputScheduler: ImmediateScheduler.Instance);

        var failureCount = 0;
        Observable.Merge(
                         fixture1.ThrownExceptions,
                         fixture2.ThrownExceptions,
                         fixture3.ThrownExceptions,
                         fixture4.ThrownExceptions).Subscribe(_ => ++failureCount);

        fixture1.Execute().Subscribe(
                                     _ => { },
                                     _ => { });
        Assert.That(
                    failureCount,
                    Is.EqualTo(1));

        fixture2.Execute().Subscribe(
                                     _ => { },
                                     _ => { });
        Assert.That(
                    failureCount,
                    Is.EqualTo(2));

        fixture3.Execute().Subscribe(
                                     _ => { },
                                     _ => { });
        Assert.That(
                    failureCount,
                    Is.EqualTo(3));

        fixture4.Execute().Subscribe(
                                     _ => { },
                                     _ => { });
        Assert.That(
                    failureCount,
                    Is.EqualTo(4));
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

        fixture.IsExecuting.Subscribe(async void (x) =>
        {
            isExecuting = x;
            await testSequencer.AdvancePhaseAsync("Executing {false, true, false}");
        });
        fixture.ThrownExceptions.Subscribe(async void (ex) =>
        {
            fail = ex;
            await testSequencer.AdvancePhaseAsync("Exception");
        });

        await testSequencer.AdvancePhaseAsync("Executing {false}");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        isExecuting,
                        Is.False);
            Assert.That(
                        fail,
                        Is.Null);
        }

        fixture.Execute().Subscribe();
        await testSequencer.AdvancePhaseAsync("Executing {true}");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        isExecuting,
                        Is.True);
            Assert.That(
                        fail,
                        Is.Null);
        }

        subj.OnNext(Unit.Default);

        // Wait to allow execution to complete
        await testSequencer.AdvancePhaseAsync("Executing {false}");
        await testSequencer.AdvancePhaseAsync("Exception");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        isExecuting,
                        Is.False);
            Assert.That(
                        fail?.Message,
                        Is.EqualTo("break execution"));
        }

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
            // Phase 1
            await testSequencer.AdvancePhaseAsync("Phase 1");
            statusTrail.Add((position++, "started command"));
            try
            {
                await Task.Delay(
                                 10000,
                                 token);
            }
            catch (OperationCanceledException)
            {
                // Phase 2: cancellation observed
                await testSequencer.AdvancePhaseAsync("Phase 2");

                // Phase 3: test observed IsExecuting while cancelling
                await testSequencer.AdvancePhaseAsync("Phase 3");
                throw;
            }
        });

        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((position++, $"command executing = {isExec}"));
            Volatile.Write(
                           ref latestIsExecutingValue,
                           isExec);
        });

        var disposable = fixture.Execute().Subscribe();

        // Phase 1
        await testSequencer.AdvancePhaseAsync("Phase 1");
        Assert.That(
                    Volatile.Read(ref latestIsExecutingValue),
                    Is.True,
                    "IsExecuting should be true when execution is underway");

        disposable.Dispose();

        // Phase 2
        await testSequencer.AdvancePhaseAsync("Phase 2");
        Assert.That(
                    Volatile.Read(ref latestIsExecutingValue),
                    Is.True,
                    "IsExecuting should remain true while cancellation is in progress");

        // Phase 3
        await testSequencer.AdvancePhaseAsync("Phase 3");

        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        Volatile.Read(ref latestIsExecutingValue),
                        Is.False,
                        "IsExecuting should be false once cancellation completes");
            Assert.That(
                        statusTrail,
                        Is.EqualTo(
                        [
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "command executing = False")
                        ]));
        }
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
        Assert.That(
                    result,
                    Is.EqualTo(3));

        testSequencer.Dispose();
    }

    [Test]
    public void ShouldCallAsyncMethodOnSettingReactiveSetpoint() =>
        new TestScheduler().WithAsync(static async scheduler =>
        {
            // set
            var fooVm = new Mocks.FooViewModel(new());

            Assert.That(
                        fooVm.Foo.Value,
                        Is.EqualTo(42),
                        "initial value unchanged");

            // act
            scheduler.AdvanceByMs(11); // async processing
            Assert.That(
                        fooVm.Foo.Value,
                        Is.Zero,
                        "value set to default Setpoint value");

            fooVm.Setpoint = 123;
            scheduler.AdvanceByMs(5); // async task processing

            // assert
            Assert.That(
                        fooVm.Foo.Value,
                        Is.Zero,
                        "value unchanged as async task still processing");
            scheduler.AdvanceByMs(6); // process async setpoint setting

            Assert.That(
                        fooVm.Foo.Value,
                        Is.EqualTo(123),
                        "value set to Setpoint value");
            await Task.CompletedTask;
        });

    [Test]
    [Ignore("Flakey on some platforms, ignore for the moment")]
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
                                                             await Task.Delay(
                                                                              10000,
                                                                              cts);
                                                         }
                                                         catch (OperationCanceledException)
                                                         {
                                                             statusTrail.Add(
                                                                             (position++,
                                                                                 "starting cancelling command"));
                                                             await testSequencer.AdvancePhaseAsync("Phase 2"); // #2
                                                             await testSequencer.AdvancePhaseAsync("Phase 3"); // #3
                                                             statusTrail.Add(
                                                                             (position++,
                                                                                 "finished cancelling command"));
                                                             throw;
                                                         }

                                                         return Unit.Default;
                                                     },
                                                     outputScheduler: ImmediateScheduler.Instance);

        Exception? fail = null;
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);
        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((position++, $"command executing = {isExec}"));
            Volatile.Write(
                           ref latestIsExecutingValue,
                           isExec);
        });

        Assert.That(
                    fail,
                    Is.Null);
        var result = false;
        var disposable = fixture.Execute().Subscribe(_ => result = true);
        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                            Volatile.Read(ref latestIsExecutingValue),
                            Is.True);
            Assert.That(
                        statusTrail.Any(x => x.Status == "started command"),
                        Is.True);
        }

        disposable.Dispose();
        await testSequencer.AdvancePhaseAsync("Phase 2"); // #2
        Assert.That(
                    Volatile.Read(ref latestIsExecutingValue),
                    Is.True);
        await testSequencer.AdvancePhaseAsync("Phase 3"); // #3

        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        result,
                        Is.False,
                        "No result expected as cancelled");
            Assert.That(
                        statusTrail,
                        Is.EqualTo(
                        [
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "starting cancelling command"),
                            (4, "finished cancelling command"),
                            (5, "command executing = False")
                        ]));
            Assert.That(
                        fail,
                        Is.TypeOf<OperationCanceledException>().Or.TypeOf<TaskCanceledException>());
        }
    }

    [Test]
    public void ReactiveCommandCreateFromTaskHandlesTaskException() =>
        new TestScheduler().With(async scheduler =>
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

            fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting)
                   .Subscribe();
            fixture.ThrownExceptions.Subscribe(ex => fail = ex);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            isExecuting[0],
                            Is.False);
                Assert.That(
                            fail,
                            Is.Null);
            }

            fixture.Execute().Subscribe();

            scheduler.AdvanceByMs(10);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            isExecuting[1],
                            Is.True);
                Assert.That(
                            fail,
                            Is.Null);
            }

            scheduler.AdvanceByMs(10);
            subj.OnNext(Unit.Default);

            scheduler.AdvanceByMs(10);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(
                            isExecuting[2],
                            Is.False);
                Assert.That(
                            fail?.Message,
                            Is.EqualTo("break execution"));
            }

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
                                                             await Task.Delay(
                                                                              10000,
                                                                              cts);
                                                         }
                                                         catch (OperationCanceledException)
                                                         {
                                                             statusTrail.Add(
                                                                             (position++,
                                                                                 "starting cancelling command"));
                                                             await testSequencer.AdvancePhaseAsync("Phase 2"); // #2
                                                             statusTrail.Add(
                                                                             (position++,
                                                                                 "finished cancelling command"));
                                                             await testSequencer.AdvancePhaseAsync("Phase 3"); // #3
                                                             throw;
                                                         }

                                                         return Unit.Default;
                                                     },
                                                     outputScheduler: ImmediateScheduler.Instance);

        Exception? fail = null;
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);
        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((position++, $"command executing = {isExec}"));
            Volatile.Write(
                           ref latestIsExecutingValue,
                           isExec);
        });

        Assert.That(
                    fail,
                    Is.Null);
        var result = false;
        var disposable = fixture.Execute().Subscribe(_ => result = true);
        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                            Volatile.Read(ref latestIsExecutingValue),
                            Is.True);
            Assert.That(
                        statusTrail.Any(x => x.Status == "started command"),
                        Is.True);
        }

        disposable.Dispose();
        await testSequencer.AdvancePhaseAsync("Phase 2"); // #2
        Assert.That(
                    Volatile.Read(ref latestIsExecutingValue),
                    Is.True);
        await testSequencer.AdvancePhaseAsync("Phase 3"); // #3

        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        result,
                        Is.False,
                        "No result expected as cancelled");
            Assert.That(
                        statusTrail,
                        Is.EqualTo(
                        [
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "starting cancelling command"),
                            (4, "finished cancelling command"),
                            (5, "command executing = False")
                        ]));
            Assert.That(
                        fail,
                        Is.TypeOf<OperationCanceledException>().Or.TypeOf<TaskCanceledException>());
        }
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
                                                             await Task.Delay(
                                                                              1000,
                                                                              cts);
                                                         }
                                                         catch (OperationCanceledException)
                                                         {
                                                             statusTrail.Add(
                                                                             (position++,
                                                                                 "starting cancelling command"));
                                                             await Task.Delay(
                                                                              5000,
                                                                              CancellationToken.None);
                                                             statusTrail.Add(
                                                                             (position++,
                                                                                 "finished cancelling command"));
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
        fixture.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((position++, $"command executing = {isExec}"));
            Volatile.Write(
                           ref latestIsExecutingValue,
                           isExec);
        });

        Assert.That(
                    fail,
                    Is.Null);
        var result = false;
        fixture.Execute().Subscribe(_ => result = true);
        await testSequencer.AdvancePhaseAsync("Phase 1"); // #1
        Assert.That(
                    Volatile.Read(ref latestIsExecutingValue),
                    Is.True);
        await testSequencer.AdvancePhaseAsync("Phase 2"); // #2

        var start = Environment.TickCount;
        while (unchecked(Environment.TickCount - start) < 1000 && Volatile.Read(ref latestIsExecutingValue))
        {
            await Task.Yield();
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        result,
                        Is.True);
            Assert.That(
                        statusTrail,
                        Is.EqualTo(
                        [
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "finished command"),
                            (4, "command executing = False")
                        ]));
            Assert.That(
                        fail,
                        Is.Null);
            Assert.That(
                        Volatile.Read(ref latestIsExecutingValue),
                        Is.False,
                        "execution should be completed");
        }
    }
}
