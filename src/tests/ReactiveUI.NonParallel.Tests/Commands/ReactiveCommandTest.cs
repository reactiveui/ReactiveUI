// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;
using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests.Core;

[NotInParallel]
public class ReactiveCommandTest : IDisposable
{
    private RxAppSchedulersScope? _schedulersScope;

    public ReactiveCommandTest()
    {
        RxApp.EnsureInitialized();
    }

    [Before(HookType.Test)]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
    }

    [After(HookType.Test)]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }

    /// <summary>
    /// A test that determines whether this instance [can execute changed is available via ICommand].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteChangedIsAvailableViaICommand()
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

        using (Assert.Multiple())
        {
            await Assert.That(canExecuteChanged).Count().IsEqualTo(2);
            await Assert.That(canExecuteChanged[0]).IsTrue();
            await Assert.That(canExecuteChanged[1]).IsFalse();
        }
    }

    /// <summary>
    /// A test that determines whether this instance [can execute is available via ICommand].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsAvailableViaICommand()
    {
        var canExecuteSubject = new Subject<bool>();
        ICommand fixture =
            ReactiveCommand.Create(
                                   static () => Observables.Unit,
                                   canExecuteSubject,
                                   ImmediateScheduler.Instance);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.CanExecute(null)).IsFalse();

            canExecuteSubject.OnNext(true);
            await Assert.That(fixture.CanExecute(null)).IsTrue();

            canExecuteSubject.OnNext(false);
            await Assert.That(fixture.CanExecute(null)).IsFalse();
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is behavioral].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsBehavioral()
    {
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             outputScheduler: ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(1);
            await Assert.That(canExecute[0]).IsTrue();
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is false if already executing].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsFalseIfAlreadyExecuting() =>
        await new TestScheduler().With(async scheduler =>
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

            using (Assert.Multiple())
            {
                await Assert.That(canExecute).Count().IsEqualTo(2);
                await Assert.That(canExecute[1]).IsFalse();
            }

            scheduler.AdvanceByMs(901);

            using (Assert.Multiple())
            {
                await Assert.That(canExecute).Count().IsEqualTo(3);
                await Assert.That(canExecute[2]).IsTrue();
            }
        });

    /// <summary>
    /// Test that determines whether this instance [can execute is false if caller dictates as such].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsFalseIfCallerDictatesAsSuch()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             canExecuteSubject,
                                             ImmediateScheduler.Instance);
        fixture.CanExecute.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var canExecute).Subscribe();

        canExecuteSubject.OnNext(true);
        canExecuteSubject.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(3);
            await Assert.That(canExecute[0]).IsFalse();
            await Assert.That(canExecute[1]).IsTrue();
            await Assert.That(canExecute[2]).IsFalse();
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute is unsubscribed after command disposal].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteIsUnsubscribedAfterCommandDisposal()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             canExecuteSubject,
                                             ImmediateScheduler.Instance);

        await Assert.That(canExecuteSubject.HasObservers).IsTrue();

        fixture.Dispose();

        await Assert.That(canExecuteSubject.HasObservers).IsFalse();
    }

    /// <summary>
    /// Test that determines whether this instance [can execute only ticks distinct values].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteOnlyTicksDistinctValues()
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

        using (Assert.Multiple())
        {
            await Assert.That(canExecute).Count().IsEqualTo(2);
            await Assert.That(canExecute[0]).IsFalse();
            await Assert.That(canExecute[1]).IsTrue();
        }
    }

    /// <summary>
    /// Test that determines whether this instance [can execute ticks failures through thrown exceptions].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteTicksFailuresThroughThrownExceptions()
    {
        var canExecuteSubject = new Subject<bool>();
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             canExecuteSubject,
                                             ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions)
               .Subscribe();

        canExecuteSubject.OnError(new InvalidOperationException("oops"));

        using (Assert.Multiple())
        {
            await Assert.That(thrownExceptions).Count().IsEqualTo(1);
            await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
        }
    }

    /// <summary>
    /// Creates the task facilitates TPL integration.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateTaskFacilitatesTPLIntegration()
    {
        var fixture =
            ReactiveCommand.CreateFromTask(
                                           static () => Task.FromResult(13),
                                           outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute().Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(results[0]).IsEqualTo(13);
        }
    }

    /// <summary>
    /// Creates the task facilitates TPL integration with parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateTaskFacilitatesTPLIntegrationWithParameter()
    {
        var fixture =
            ReactiveCommand.CreateFromTask<int, int>(
                                                     static param => Task.FromResult(param + 1),
                                                     outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute(3).Subscribe();
        fixture.Execute(41).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(2);
            await Assert.That(results[0]).IsEqualTo(4);
            await Assert.That(results[1]).IsEqualTo(42);
        }
    }

    /// <summary>
    /// Creates the throws if execution parameter is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateThrowsIfExecutionParameterIsNull()
    {
#pragma warning disable CS4014 // Because this call is not awaited
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create(null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<Unit>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Action<Unit>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<Unit, Unit>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<IObservable<Unit>>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<Task<Unit>>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<Unit, IObservable<Unit>>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.Create((Func<Unit, Task<Unit>>)null!);
            await Task.CompletedTask;
        });
#pragma warning restore CS4014
    }

    /// <summary>
    /// Creates the throws if execution parameter is null (RunInBackground).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CreateRunInBackgroundThrowsIfExecutionParameterIsNull()
    {
#pragma warning disable CS4014 // Because this call is not awaited
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground(null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<Unit>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Action<Unit>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<Unit, Unit>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<IObservable<Unit>>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<Task<Unit>>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<Unit, IObservable<Unit>>)null!);
            await Task.CompletedTask;
        });
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            ReactiveCommand.CreateRunInBackground((Func<Unit, Task<Unit>>)null!);
            await Task.CompletedTask;
        });
#pragma warning restore CS4014
    }

    /// <summary>
    /// Exceptions are delivered on output scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExceptionsAreDeliveredOnOutputScheduler() =>
        await new TestScheduler().With(async scheduler =>
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

            await Assert.That(exception).IsNull();
            scheduler.Start();
            await Assert.That(exception).IsTypeOf<InvalidOperationException>();
        });

    /// <summary>
    /// Executes can be cancelled.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteCanBeCancelled() =>
        await new TestScheduler().With(async scheduler =>
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

            using (Assert.Multiple())
            {
                await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsTrue();
                await Assert.That(executed).IsEmpty();
            }

            sub1.Dispose();

            scheduler.AdvanceByMs(2);

            using (Assert.Multiple())
            {
                await Assert.That(executed).Count().IsEqualTo(1);
                await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsFalse();
            }
        });

    /// <summary>
    /// Executes can tick through multiple results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteCanTickThroughMultipleResults()
    {
        var fixture = ReactiveCommand.CreateFromObservable(
                                                           static () => new[] { 1, 2, 3 }.ToObservable(),
                                                           outputScheduler: ImmediateScheduler.Instance);
        fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

        fixture.Execute().Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(3);
            await Assert.That(results[0]).IsEqualTo(1);
            await Assert.That(results[1]).IsEqualTo(2);
            await Assert.That(results[2]).IsEqualTo(3);
        }
    }

    /// <summary>
    /// Executes facilitates any number of in flight executions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteFacilitatesAnyNumberOfInFlightExecutions() =>
        await new TestScheduler().With(async scheduler =>
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

            using (Assert.Multiple())
            {
                await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsTrue();
                await Assert.That(executed).IsEmpty();
            }

            scheduler.AdvanceByMs(101);

            using (Assert.Multiple())
            {
                await Assert.That(executed).Count().IsEqualTo(2);
                await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsTrue();
            }

            scheduler.AdvanceByMs(200);

            using (Assert.Multiple())
            {
                await Assert.That(executed).Count().IsEqualTo(3);
                await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsTrue();
            }

            scheduler.AdvanceByMs(100);

            using (Assert.Multiple())
            {
                await Assert.That(executed).Count().IsEqualTo(4);
                await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsFalse();
            }
        });

    /// <summary>
    /// Execute is available via ICommand.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteIsAvailableViaICommand()
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
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Execute passes through parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecutePassesThroughParameter()
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

        using (Assert.Multiple())
        {
            await Assert.That(parameters).Count().IsEqualTo(3);
            await Assert.That(parameters[0]).IsEqualTo(1);
            await Assert.That(parameters[1]).IsEqualTo(42);
            await Assert.That(parameters[2]).IsEqualTo(348);
        }
    }

    /// <summary>
    /// Execute re-enables execution even after failure.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteReenablesExecutionEvenAfterFailure()
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

        using (Assert.Multiple())
        {
            await Assert.That(thrownExceptions).Count().IsEqualTo(1);
            await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
            await Assert.That(canExecute).Count().IsEqualTo(3);
            await Assert.That(canExecute[0]).IsTrue();
            await Assert.That(canExecute[1]).IsFalse();
            await Assert.That(canExecute[2]).IsTrue();
        }
    }

    /// <summary>
    /// Execute result is delivered on specified scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteResultIsDeliveredOnSpecifiedScheduler() =>
        await new TestScheduler().With(async scheduler =>
        {
            var execute = Observables.Unit;
            var fixture = ReactiveCommand.CreateFromObservable(
                                                               () => execute,
                                                               outputScheduler: scheduler);
            var executed = false;

            fixture.Execute().ObserveOn(scheduler).Subscribe(_ => executed = true);

            await Assert.That(executed).IsFalse();
            scheduler.AdvanceByMs(1);
            await Assert.That(executed).IsTrue();
        });

    /// <summary>
    /// Execute ticks any exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksAnyException()
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

        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>
    /// Execute ticks any lambda exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksAnyLambdaException()
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

        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>
    /// Execute ticks errors through thrown exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksErrorsThroughThrownExceptions()
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

        using (Assert.Multiple())
        {
            await Assert.That(thrownExceptions).Count().IsEqualTo(1);
            await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
        }
    }

    /// <summary>
    /// Execute ticks lambda errors through thrown exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksLambdaErrorsThroughThrownExceptions()
    {
        var fixture = ReactiveCommand.CreateFromObservable<Unit>(
                                                                 static () => throw new InvalidOperationException("oops"),
                                                                 outputScheduler: ImmediateScheduler.Instance);
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var thrownExceptions)
               .Subscribe();

        fixture.Execute().Subscribe(
                                    static _ => { },
                                    static _ => { });

        using (Assert.Multiple())
        {
            await Assert.That(thrownExceptions).Count().IsEqualTo(1);
            await Assert.That(thrownExceptions[0].Message).IsEqualTo("oops");
            await Assert.That(fixture.CanExecute.FirstAsync().Wait()).IsTrue();
        }
    }

    /// <summary>
    /// Execute ticks through the result.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteTicksThroughTheResult()
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

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(3);
            await Assert.That(results[0]).IsEqualTo(1);
            await Assert.That(results[1]).IsEqualTo(10);
            await Assert.That(results[2]).IsEqualTo(30);
        }
    }

    /// <summary>
    /// Execute via ICommand throws if parameter type is incorrect.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteViaICommandThrowsIfParameterTypeIsIncorrect()
    {
        ICommand fixture = ReactiveCommand.Create<int>(
                                                       _ => { },
                                                       outputScheduler: ImmediateScheduler.Instance);
        var ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute("foo"));
        await Assert.That(ex!.Message).IsEqualTo("Command requires parameters of type System.Int32, but received parameter of type System.String.");

        fixture = ReactiveCommand.Create<string>(_ => { });
        ex = Assert.Throws<InvalidOperationException>(() => fixture.Execute(13));
        await Assert.That(ex!.Message).IsEqualTo("Command requires parameters of type System.String, but received parameter of type System.Int32.");
    }

    /// <summary>
    /// Execute via ICommand works with nullable types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteViaICommandWorksWithNullableTypes()
    {
        int? value = null;
        ICommand fixture =
            ReactiveCommand.Create<int?>(
                                         param => value = param,
                                         outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute(42);
        await Assert.That(value).IsEqualTo(42);

        fixture.Execute(null);
        await Assert.That(value).IsNull();
    }

    /// <summary>
    /// Invoke command against ICommand in target invokes the command.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInTargetInvokesTheCommand()
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
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command against ICommand in target passes the specified value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new ICommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             static x => x!.TheCommand!);
        var command = new FakeCommand();
        fixture.TheCommand = command;

        source.OnNext(42);
        using (Assert.Multiple())
        {
            await Assert.That(command.CanExecuteParameter).IsEqualTo(42);
            await Assert.That(command.ExecuteParameter).IsEqualTo(42);
        }
    }

    /// <summary>
    /// Invoke command against nullable ICommand in target passes the specified value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInNullableTargetPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new ICommandHolder();
        var source = new Subject<int>();
        source.InvokeCommand(
                             fixture,
                             static x => x.TheCommand);
        var command = new FakeCommand();
        fixture.TheCommand = command;

        source.OnNext(42);
        using (Assert.Multiple())
        {
            await Assert.That(command.CanExecuteParameter).IsEqualTo(42);
            await Assert.That(command.ExecuteParameter).IsEqualTo(42);
        }
    }

    /// <summary>
    /// Invoke command against ICommand in target respects can execute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInTargetRespectsCanExecute()
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
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Invoke command against nullable target respects can execute window.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInNullableTargetRespectsCanExecute()
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
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Invoke command against ICommand in target respects can execute window.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInTargetRespectsCanExecuteWindow()
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
        await Assert.That(executed).IsFalse();

        // When the window reopens, previous requests should NOT execute.
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

    /// <summary>
    /// Invoke command against ICommand in target swallows exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInTargetSwallowsExceptions()
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

        await Assert.That(count).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command against ICommand invokes the command.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandInvokesTheCommand()
    {
        var executionCount = 0;
        ICommand fixture = ReactiveCommand.Create(
                                                  () => ++executionCount,
                                                  outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command against nullable ICommand invokes the command.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstNullableICommandInvokesTheCommand()
    {
        var executionCount = 0;
        ICommand fixture =
            ReactiveCommand.Create(
                                   () => ++executionCount,
                                   outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command against ICommand passes the specified value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandPassesTheSpecifiedValueToCanExecuteAndExecute()
    {
        var fixture = new FakeCommand();
        var source = new Subject<int>();
        source.InvokeCommand(fixture);

        source.OnNext(42);
        using (Assert.Multiple())
        {
            await Assert.That(fixture.CanExecuteParameter).IsEqualTo(42);
            await Assert.That(fixture.ExecuteParameter).IsEqualTo(42);
        }
    }

    /// <summary>
    /// Invoke command against ICommand respects can execute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstICommandRespectsCanExecute()
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
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Invoke command against reactive command in target invokes the command.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandInTargetInvokesTheCommand()
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
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(0);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command against reactive command in target passes the specified value to execute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandInTargetPassesTheSpecifiedValueToExecute()
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
        await Assert.That(executeReceived).IsEqualTo(42);
    }

    /// <summary>
    /// Invoke command against reactive command in target respects can execute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecute()
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
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(0);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Invoke command against reactive command in target respects can execute window.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandInTargetRespectsCanExecuteWindow()
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
        await Assert.That(executed).IsFalse();

        // When the window reopens, previous requests should NOT execute.
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

    /// <summary>
    /// Invoke command against reactive command in target swallows exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandInTargetSwallowsExceptions()
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

        await Assert.That(count).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command against reactive command invokes the command.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandInvokesTheCommand()
    {
        var executionCount = 0;
        var fixture = ReactiveCommand.Create(
                                             () => ++executionCount,
                                             outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<Unit>();
        source.InvokeCommand(fixture);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(1);

        source.OnNext(Unit.Default);
        await Assert.That(executionCount).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command against reactive command passes the specified value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandPassesTheSpecifiedValueToExecute()
    {
        var executeReceived = 0;
        var fixture =
            ReactiveCommand.Create<int>(
                                        x => executeReceived = x,
                                        outputScheduler: ImmediateScheduler.Instance);
        var source = new Subject<int>();
        source.InvokeCommand(fixture);

        source.OnNext(42);
        await Assert.That(executeReceived).IsEqualTo(42);
    }

    /// <summary>
    /// Invoke command against reactive command respects can execute.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandRespectsCanExecute()
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
        await Assert.That(executed).IsFalse();

        canExecute.OnNext(true);
        source.OnNext(Unit.Default);
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Invoke command against reactive command respects can execute window.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandRespectsCanExecuteWindow()
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
        await Assert.That(executed).IsFalse();

        // When the window reopens, previous requests should NOT execute.
        canExecute.OnNext(true);
        await Assert.That(executed).IsFalse();
    }

    /// <summary>
    /// Invoke command against reactive command swallows exceptions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandAgainstReactiveCommandSwallowsExceptions()
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

        await Assert.That(count).IsEqualTo(2);
    }

    /// <summary>
    /// Invoke command works even if the source is cold.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task InvokeCommandWorksEvenIfTheSourceIsCold()
    {
        var executionCount = 0;
        var fixture = ReactiveCommand.Create(
                                             () => ++executionCount,
                                             outputScheduler: ImmediateScheduler.Instance);
        var source = Observable.Return(Unit.Default);
        source.InvokeCommand(fixture);

        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>
    /// IsExecuting is behavioral.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsExecutingIsBehavioral()
    {
        var fixture = ReactiveCommand.Create(
                                             static () => Observables.Unit,
                                             outputScheduler: ImmediateScheduler.Instance);
        fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting).Count().IsEqualTo(1);
            await Assert.That(isExecuting[0]).IsFalse();
        }
    }

    /// <summary>
    /// IsExecuting remains true as long as pipeline has not completed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsExecutingRemainsTrueAsLongAsExecutionPipelineHasNotCompleted()
    {
        var execute = new Subject<Unit>();
        var fixture = ReactiveCommand.CreateFromObservable(
                                                           () => execute,
                                                           outputScheduler: ImmediateScheduler.Instance);

        fixture.Execute().Subscribe();

        await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsTrue();

        execute.OnNext(Unit.Default);
        await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsTrue();

        execute.OnNext(Unit.Default);
        await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsTrue();

        execute.OnCompleted();
        await Assert.That(fixture.IsExecuting.FirstAsync().Wait()).IsFalse();
    }

    /// <summary>
    /// IsExecuting ticks as executions progress.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task IsExecutingTicksAsExecutionsProgress() =>
        await new TestScheduler().With(async scheduler =>
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

            using (Assert.Multiple())
            {
                await Assert.That(isExecuting).Count().IsEqualTo(2);
                await Assert.That(isExecuting[0]).IsFalse();
                await Assert.That(isExecuting[1]).IsTrue();
            }

            scheduler.AdvanceByMs(901);

            using (Assert.Multiple())
            {
                await Assert.That(isExecuting).Count().IsEqualTo(3);
                await Assert.That(isExecuting[2]).IsFalse();
            }
        });

    /// <summary>
    /// Result is ticked through specified scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResultIsTickedThroughSpecifiedScheduler() =>
        await new TestScheduler().WithAsync(static async scheduler =>
        {
            var fixture = ReactiveCommand.CreateRunInBackground(
                                                                static () => Observables.Unit,
                                                                outputScheduler: scheduler);
            fixture.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var results).Subscribe();

            fixture.Execute().Subscribe();
            await Assert.That(results).IsEmpty();

            scheduler.AdvanceByMs(1);
            await Assert.That(results).Count().IsEqualTo(1);
            return Task.CompletedTask;
        });

    /// <summary>
    /// Synchronous command executes lazily.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SynchronousCommandExecuteLazily()
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

        await Assert.That(executionCount).IsEqualTo(0);

        execute1.Subscribe();
        await Assert.That(executionCount).IsEqualTo(1);

        execute2.Subscribe();
        await Assert.That(executionCount).IsEqualTo(2);

        execute3.Subscribe();
        await Assert.That(executionCount).IsEqualTo(3);

        execute4.Subscribe();
        await Assert.That(executionCount).IsEqualTo(4);
    }

    /// <summary>
    /// Synchronous commands fail correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SynchronousCommandsFailCorrectly()
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
        await Assert.That(failureCount).IsEqualTo(1);

        fixture2.Execute().Subscribe(
                                     _ => { },
                                     _ => { });
        await Assert.That(failureCount).IsEqualTo(2);

        fixture3.Execute().Subscribe(
                                     _ => { },
                                     _ => { });
        await Assert.That(failureCount).IsEqualTo(3);

        fixture4.Execute().Subscribe(
                                     _ => { },
                                     _ => { });
        await Assert.That(failureCount).IsEqualTo(4);
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesTaskExceptionAsync()
    {
        var tcsStart = new TaskCompletionSource<Unit>();
        var isExecutingList = new List<bool>();
        Exception? fail = null;

        var fixture = ReactiveCommand.CreateFromTask(
                                                     async _ =>
                                                     {
                                                         await tcsStart.Task;
                                                         throw new Exception("break execution");
                                                     },
                                                     outputScheduler: ImmediateScheduler.Instance);

        fixture.IsExecuting.Subscribe(x => isExecutingList.Add(x));
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);

        await Assert.That(isExecutingList.LastOrDefault()).IsFalse();
        await Assert.That(fail).IsNull();

        fixture.Execute().Subscribe();

        // Wait for execution to start (IsExecuting should become true)
        await Task.Delay(100);
        await Assert.That(isExecutingList.LastOrDefault()).IsTrue();
        await Assert.That(fail).IsNull();

        // Signal task to proceed and throw
        tcsStart.SetResult(Unit.Default);

        // Wait for completion (IsExecuting should become false)
        await Task.Delay(100);
        await Assert.That(isExecutingList.LastOrDefault()).IsFalse();

        using (Assert.Multiple())
        {
            await Assert.That(fail?.Message).IsEqualTo("break execution");
        }
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskThenCancelSetsIsExecutingFalseOnlyAfterCancellationCompleteAsync()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsCaught = new TaskCompletionSource<Unit>();
        var tcsFinish = new TaskCompletionSource<Unit>();

        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;

        var fixture = ReactiveCommand.CreateFromTask(
            async (token) =>
            {
                statusTrail.Add((Interlocked.Increment(ref position) - 1, "started command"));
                tcsStarted.TrySetResult(Unit.Default);
                try
                {
                    await Task.Delay(
                                     10000,
                                     token);
                }
                catch (OperationCanceledException)
                {
                    tcsCaught.TrySetResult(Unit.Default);
                    await tcsFinish.Task;
                    throw;
                }
            },
            outputScheduler: ImmediateScheduler.Instance);

        // Subscribe to ThrownExceptions so RxApp.DefaultExceptionHandler doesn't terminate the test host
        fixture.ThrownExceptions.Subscribe(_ => { });

        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((Interlocked.Increment(ref position) - 1, $"command executing = {isExec}"));
            Volatile.Write(ref latestIsExecutingValue, isExec);
        });

        // IsExecuting subscription should emit initial false value immediately with ImmediateScheduler
        await Assert.That(latestIsExecutingValue).IsFalse();

        var disposable = fixture.Execute().Subscribe();

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        disposable.Dispose();

        await tcsCaught.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        tcsFinish.TrySetResult(Unit.Default);

        // Wait a bit for the cancellation to complete and IsExecuting to become false
        await Task.Delay(100);
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsFalse();

        using (Assert.Multiple())
        {
            await Assert.That(statusTrail).IsEquivalentTo([
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "command executing = False")
                        ]);
        }
    }

    [Test]
    public async Task ReactiveCommandExecutesFromInvokeCommand()
    {
        var tcs = new TaskCompletionSource<Unit>();
        var command = ReactiveCommand.Create(() => tcs.TrySetResult(Unit.Default));
        var result = 0;

        // False, True, False
        command.IsExecuting.Subscribe(_ => result++);

        Observable.Return(Unit.Default)
                  .InvokeCommand(command);

        await tcs.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(result).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task ShouldCallAsyncMethodOnSettingReactiveSetpoint() =>
        await new TestScheduler().WithAsync(static async scheduler =>
        {
            // set
            var fooVm = new Mocks.FooViewModel(new());

            await Assert.That(fooVm.Foo.Value).IsEqualTo(42);

            // act
            scheduler.AdvanceByMs(11); // async processing
            await Assert.That(fooVm.Foo.Value).IsEqualTo(0);

            fooVm.Setpoint = 123;
            scheduler.AdvanceByMs(5); // async task processing

            // assert
            await Assert.That(fooVm.Foo.Value).IsEqualTo(0);
            scheduler.AdvanceByMs(6); // process async setpoint setting

            await Assert.That(fooVm.Foo.Value).IsEqualTo(123);
            await Task.CompletedTask;
        });

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesExecuteCancellation()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsCaught = new TaskCompletionSource<Unit>();
        var tcsFinish = new TaskCompletionSource<Unit>();

        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;
        var fixture = ReactiveCommand.CreateFromTask(
                                                     async cts =>
                                                     {
                                                         statusTrail.Add((Interlocked.Increment(ref position) - 1, "started command"));
                                                         tcsStarted.TrySetResult(Unit.Default);
                                                         try
                                                         {
                                                             await Task.Delay(
                                                                              10000,
                                                                              cts);
                                                         }
                                                         catch (OperationCanceledException)
                                                         {
                                                             statusTrail.Add(
                                                                             (Interlocked.Increment(ref position) - 1,
                                                                                 "starting cancelling command"));
                                                             tcsCaught.TrySetResult(Unit.Default);
                                                             await tcsFinish.Task;
                                                             statusTrail.Add(
                                                                             (Interlocked.Increment(ref position) - 1,
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
            statusTrail.Add((Interlocked.Increment(ref position) - 1, $"command executing = {isExec}"));
            Volatile.Write(
                           ref latestIsExecutingValue,
                           isExec);
        });

        await Assert.That(fail).IsNull();
        var result = false;
        var disposable = fixture.Execute().Subscribe(_ => result = true);

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));
        using (Assert.Multiple())
        {
            await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();
            await Assert.That(statusTrail.Any(x => x.Status == "started command")).IsTrue();
        }

        disposable.Dispose();

        await tcsCaught.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();
        tcsFinish.TrySetResult(Unit.Default);

        await Task.Delay(100);
        await Assert.That(fail).IsNotNull();

        using (Assert.Multiple())
        {
            await Assert.That(result).IsFalse();
            await Assert.That(statusTrail).IsEquivalentTo([
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "starting cancelling command"),
                            (4, "finished cancelling command"),
                            (5, "command executing = False")
                        ]);
            await Assert.That(fail).IsTypeOf<TaskCanceledException>();
        }
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesTaskException()
    {
        var subj = new Subject<Unit>();
        Exception? fail = null;
        var fixture = ReactiveCommand.CreateFromTask(
                                                     async cts =>
                                                     {
                                                         await subj.Take(1);
                                                         throw new Exception("break execution");
                                                     },
                                                     outputScheduler: ImmediateScheduler.Instance);

        fixture.IsExecuting.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var isExecuting)
               .Subscribe();
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting[0]).IsFalse();
            await Assert.That(fail).IsNull();
        }

        fixture.Execute().Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting[1]).IsTrue();
            await Assert.That(fail).IsNull();
        }

        subj.OnNext(Unit.Default);

        // Required for correct async / await task handling
        await Task.Delay(10);

        using (Assert.Multiple())
        {
            await Assert.That(isExecuting).Count().IsGreaterThanOrEqualTo(3);
            await Assert.That(isExecuting[2]).IsFalse();
            await Assert.That(fail?.Message).IsEqualTo("break execution");
        }
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesCancellation()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsCaught = new TaskCompletionSource<Unit>();
        var tcsFinish = new TaskCompletionSource<Unit>();

        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;
        var fixture = ReactiveCommand.CreateFromTask(
                                                     async cts =>
                                                     {
                                                         statusTrail.Add((Interlocked.Increment(ref position) - 1, "started command"));
                                                         tcsStarted.TrySetResult(Unit.Default);
                                                         try
                                                         {
                                                             await Task.Delay(
                                                                              10000,
                                                                              cts);
                                                         }
                                                         catch (OperationCanceledException)
                                                         {
                                                             statusTrail.Add(
                                                                             (Interlocked.Increment(ref position) - 1,
                                                                                 "starting cancelling command"));
                                                             tcsCaught.TrySetResult(Unit.Default);
                                                             statusTrail.Add(
                                                                             (Interlocked.Increment(ref position) - 1,
                                                                                 "finished cancelling command"));
                                                             await tcsFinish.Task;
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
            statusTrail.Add((Interlocked.Increment(ref position) - 1, $"command executing = {isExec}"));
            Volatile.Write(
                           ref latestIsExecutingValue,
                           isExec);
        });

        await Assert.That(fail).IsNull();
        var result = false;
        var disposable = fixture.Execute().Subscribe(_ => result = true);

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));
        using (Assert.Multiple())
        {
            await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();
            await Assert.That(statusTrail.Any(x => x.Status == "started command")).IsTrue();
        }

        disposable.Dispose();

        await tcsCaught.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();
        tcsFinish.TrySetResult(Unit.Default);

        await Task.Delay(100);
        await Assert.That(fail).IsNotNull();

        using (Assert.Multiple())
        {
            await Assert.That(result).IsFalse();
            await Assert.That(statusTrail).IsEquivalentTo([
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "starting cancelling command"),
                            (4, "finished cancelling command"),
                            (5, "command executing = False")
                        ]);
            await Assert.That(fail).IsTypeOf<TaskCanceledException>();
        }
    }

    [Test]
    public async Task ReactiveCommandCreateFromTaskHandlesCompletion()
    {
        var tcsStarted = new TaskCompletionSource<Unit>();
        var tcsFinished = new TaskCompletionSource<Unit>();
        var tcsContinue = new TaskCompletionSource<Unit>();

        var statusTrail = new List<(int Position, string Status)>();
        var position = 0;
        var fixture = ReactiveCommand.CreateFromTask(
                                                     async cts =>
                                                     {
                                                         statusTrail.Add((Interlocked.Increment(ref position) - 1, "started command"));
                                                         tcsStarted.TrySetResult(Unit.Default);
                                                         try
                                                         {
                                                             await Task.Delay(
                                                                              1000,
                                                                              cts);
                                                         }
                                                         catch (OperationCanceledException)
                                                         {
                                                             statusTrail.Add(
                                                                             (Interlocked.Increment(ref position) - 1,
                                                                                 "starting cancelling command"));
                                                             await Task.Delay(
                                                                              5000,
                                                                              CancellationToken.None);
                                                             statusTrail.Add(
                                                                             (Interlocked.Increment(ref position) - 1,
                                                                                 "finished cancelling command"));
                                                             throw;
                                                         }

                                                         statusTrail.Add((Interlocked.Increment(ref position) - 1, "finished command"));
                                                         tcsFinished.TrySetResult(Unit.Default);
                                                         await tcsContinue.Task;
                                                         return Unit.Default;
                                                     },
                                                     outputScheduler: ImmediateScheduler.Instance);

        Exception? fail = null;
        fixture.ThrownExceptions.Subscribe(ex => fail = ex);
        var latestIsExecutingValue = false;
        fixture.IsExecuting.Subscribe(isExec =>
        {
            statusTrail.Add((Interlocked.Increment(ref position) - 1, $"command executing = {isExec}"));
            Volatile.Write(
                           ref latestIsExecutingValue,
                           isExec);
        });

        await Assert.That(fail).IsNull();
        var result = false;
        fixture.Execute().Subscribe(_ => result = true);

        await tcsStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();

        await tcsFinished.Task.WaitAsync(TimeSpan.FromSeconds(2));
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsTrue();
        tcsContinue.TrySetResult(Unit.Default);

        await Task.Delay(100);
        await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsFalse();

        using (Assert.Multiple())
        {
            await Assert.That(result).IsTrue();
            await Assert.That(statusTrail).IsEquivalentTo([
                            (0, "command executing = False"),
                            (1, "command executing = True"),
                            (2, "started command"),
                            (3, "finished command"),
                            (4, "command executing = False")
                        ]);
            await Assert.That(fail).IsNull();
            await Assert.That(Volatile.Read(ref latestIsExecutingValue)).IsFalse();
        }
    }

    public void Dispose()
    {
        _schedulersScope?.Dispose();
        _schedulersScope = null;
    }
}
