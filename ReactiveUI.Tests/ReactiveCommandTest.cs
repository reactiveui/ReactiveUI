﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI.Testing;
using Microsoft.Reactive.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ReactiveCommandInterfaceTest
    {
        protected ReactiveCommand<object> createCommand(IObservable<bool> canExecute, IScheduler scheduler = null)
        {
            return ReactiveCommand.Create(canExecute, scheduler);
        }

        [Fact]
        public void CompletelyDefaultReactiveCommandShouldFire()
        {
            var sched = new TestScheduler();
            var fixture = createCommand(null, sched);
            Assert.True(fixture.CanExecute(null));

            string result = null;
            fixture.Subscribe(x => result = x as string);

            fixture.Execute("Test");
            sched.Start();
            Assert.Equal("Test", result);
            fixture.Execute("Test2");
            sched.Start();
            Assert.Equal("Test2", result);
        }

        [Fact]
        public void ObservableCanExecuteShouldShowUpInCommand()
        {
            var input = new[] {true, false, false, true, false, true};
            var result = (new TestScheduler()).With(sched => {
                var can_execute = new Subject<bool>();
                var fixture = createCommand(can_execute, sched);
                var changes_as_observable = fixture.CanExecuteObservable.CreateCollection();

                int change_event_count = 0;
                fixture.CanExecuteChanged += (o, e) => { change_event_count++; };
                input.Run(x => {
                    can_execute.OnNext(x);
                    sched.Start();
                    Assert.Equal(x, fixture.CanExecute(null));
                });

                // N.B. We check against '5' instead of 6 because we're supposed to 
                // suppress changes that aren't actually changes i.e. false => false
                sched.AdvanceToMs(10*1000);
                return changes_as_observable;
            });

            // NB: Skip(1) is because CanExecuteObservable should have
            // BehaviorSubject Nature(tm)
            input.DistinctUntilChanged().AssertAreEqual(result.Skip(1).ToList());
        }

        [Fact]
        public void ObservableExecuteFuncShouldBeObservableAndAct()
        {
            var executed_params = new List<object>();
            var fixture = createCommand(null);
            fixture.Subscribe(x => executed_params.Add(x));

            var observed_params = new ReplaySubject<object>();
            fixture.Subscribe(observed_params.OnNext, observed_params.OnError, observed_params.OnCompleted);

            var range = Enumerable.Range(0, 5);
            range.Run(x => fixture.Execute(x));

            range.AssertAreEqual(executed_params.OfType<int>());

            range.ToObservable()
                .Zip(observed_params, (expected, actual) => new {expected, actual})
                .Do(Console.WriteLine)
                .Subscribe(x => Assert.Equal(x.expected, x.actual));
        }

        [Fact]
        public void ObservableCanExecuteIsNotNullAfterCanExecuteCalled()
        {
            var fixture = createCommand(null);

            fixture.CanExecute(null);

            Assert.NotNull(fixture.CanExecuteObservable);
        }

        [Fact]
        public void ObservableCanExecuteIsNotNullAfterCanExecuteChangedEventAdded()
        {
            var fixture = createCommand(null);

            fixture.CanExecuteChanged += (sender, args) => { };

            Assert.NotNull(fixture.CanExecuteObservable);
        }

        [Fact]
        public void MultipleSubscribesShouldntResultInMultipleNotifications()
        {
            var input = new[] {1, 2, 1, 2};
            var sched = new TestScheduler();
            var fixture = createCommand(null, sched);

            var odd_list = new List<int>();
            var even_list = new List<int>();
            fixture.Where(x => ((int)x)%2 != 0).Subscribe(x => odd_list.Add((int)x));
            fixture.Where(x => ((int)x)%2 == 0).Subscribe(x => even_list.Add((int)x));

            input.Run(x => fixture.Execute(x));
            sched.AdvanceToMs(1000);

            new[] {1, 1}.AssertAreEqual(odd_list);
            new[] {2, 2}.AssertAreEqual(even_list);
        }

        [Fact]
        public void CanExecuteExceptionShouldntPermabreakCommands()
        {
            var canExecute = new Subject<bool>();
            var fixture = createCommand(canExecute);

            var exceptions = new List<Exception>();
            var canExecuteStates = new List<bool>();
            fixture.CanExecuteObservable.Subscribe(canExecuteStates.Add);
            fixture.ThrownExceptions.Subscribe(exceptions.Add);

            canExecute.OnNext(false);
            Assert.False(fixture.CanExecute(null));

            canExecute.OnNext(true);
            Assert.True(fixture.CanExecute(null));

            canExecute.OnError(new Exception("Aieeeee!"));

            // The command should latch to false forever
            Assert.False(fixture.CanExecute(null));

            Assert.Equal(1, exceptions.Count);
            Assert.Equal("Aieeeee!", exceptions[0].Message);

            Assert.Equal(false, canExecuteStates[canExecuteStates.Count - 1]);
            Assert.Equal(true, canExecuteStates[canExecuteStates.Count - 2]);
        }

        [Fact]
        public void NoSubscriberOfThrownExceptionsEqualsDeath()
        {
            (new TestScheduler()).With(sched => {
                var canExecute = new Subject<bool>();
                var fixture = createCommand(canExecute, sched);
                var result = fixture.CanExecuteObservable.CreateCollection();

                bool failed = true;
                try {
                    sched.AdvanceByMs(10);
                    canExecute.OnNext(true);
                    canExecute.OnError(new Exception("Aieeeee!"));
                    sched.AdvanceByMs(10);

                    // NB: canExecute failing should bring us down
                    Assert.True(false);
                } catch (Exception ex) {
                    failed = (ex.InnerException.Message != "Aieeeee!");
                }

                Assert.False(failed);
            });
        }

        [Fact]
        public async Task ExecuteAsyncThrowsExceptionOnError()
        {
            var command = ReactiveCommand.CreateAsyncObservable(_ =>
                Observable.Throw<Unit>(new Exception("Aieeeee!")));

            var exceptions = command.ThrownExceptions.CreateCollection();

            bool failed = false;
            try {
                await command.ExecuteAsync();
            } catch (Exception ex) {
                failed = ex.Message == "Aieeeee!";
            }

            Assert.True(failed);
            Assert.Equal(1, exceptions.Count);
            Assert.Equal("Aieeeee!", exceptions[0].Message);
        }

        [Fact]
        public void ExecuteDoesntThrowOnError()
        {
            var command = ReactiveCommand.CreateAsyncObservable(_ =>
            Observable.Throw<Unit>(new Exception("Aieeeee!")));

            command.ThrownExceptions.Subscribe();

            command.Execute(null);
        }
    }

    public class ReactiveAsyncCommandTest
    {
        [Fact]
        public void RegisterAsyncFunctionSmokeTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = ReactiveCommand.CreateAsyncObservable(Observable.Return(true),
                    _ => Observable.Return(5).Delay(TimeSpan.FromSeconds(5), sched));

                IReactiveDerivedList<int> results;

                results = fixture.CreateCollection();

                var inflightResults = fixture.IsExecuting.CreateCollection();
                sched.AdvanceToMs(10);
                Assert.True(fixture.CanExecute(null));

                fixture.Execute(null);
                sched.AdvanceToMs(1005);
                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(5100);
                Assert.True(fixture.CanExecute(null));

                new[] {false, true, false}.AssertAreEqual(inflightResults);
                new[] {5}.AssertAreEqual(results);
            });
        }

        [Fact]
        public void MultipleSubscribersShouldntDecrementRefcountBelowZero()
        {
            (new TestScheduler()).With(sched => {
                var fixture = ReactiveCommand.CreateAsyncObservable(Observable.Return(true),
                    _ => Observable.Return(5).Delay(TimeSpan.FromMilliseconds(5000), sched));

                var results = new List<int>();
                bool[] subscribers = new[] {false, false, false, false, false};

                fixture.Subscribe(x => results.Add(x));

                Enumerable.Range(0, 5).Run(x => fixture.Subscribe(_ => subscribers[x] = true));

                Assert.True(fixture.CanExecute(null));

                fixture.Execute(null);
                sched.AdvanceToMs(2000);
                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(6000);
                Assert.True(fixture.CanExecute(null));

                Assert.True(results.Count == 1);
                Assert.True(results[0] == 5);
                Assert.True(subscribers.All(x => x));
            });
        }

        [Fact]
        public void MultipleResultsFromObservableShouldntDecrementRefcountBelowZero()
        {
            (new TestScheduler()).With(sched => {
                bool latestExecuting = false;
                var fixture = ReactiveCommand.CreateAsyncObservable(Observable.Return(true),
                    _ => new[] {1, 2, 3}.ToObservable(),
                    sched);

                var results = fixture.CreateCollection();
                fixture.IsExecuting.Subscribe(x => latestExecuting = x);

                fixture.Execute(1);
                sched.Start();

                Assert.Equal(3, results.Count);
                Assert.Equal(false, latestExecuting);
            });
        }

        [Fact]
        public void CanExecuteShouldChangeOnInflightOp()
        {
            (new TestScheduler()).With(sched => {
                var canExecute = sched.CreateHotObservable(
                    sched.OnNextAt(0, true),
                    sched.OnNextAt(250, false),
                    sched.OnNextAt(500, true),
                    sched.OnNextAt(750, false),
                    sched.OnNextAt(1000, true),
                    sched.OnNextAt(1100, false)
                    );

                var fixture = ReactiveCommand.CreateAsyncObservable(canExecute,
                    x => Observable.Return((int)x * 5).Delay(TimeSpan.FromMilliseconds(900), RxApp.MainThreadScheduler));
                
                int calculatedResult = -1;
                bool latestCanExecute = false;

                fixture.Subscribe(x => calculatedResult = x);

                fixture.CanExecuteObservable.Subscribe(x => latestCanExecute = x);

                // CanExecute should be true, both input observable is true
                // and we don't have anything inflight
                sched.AdvanceToMs(10);
                Assert.True(fixture.CanExecute(1));
                Assert.True(latestCanExecute);

                // Invoke a command 10ms in
                fixture.Execute(1);

                // At 300ms, input is false
                sched.AdvanceToMs(300);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);

                // At 600ms, input is true, but the command is still running
                sched.AdvanceToMs(600);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);

                // After we've completed, we should still be false, since from
                // 750ms-1000ms the input observable is false
                sched.AdvanceToMs(900);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);
                Assert.Equal(-1, calculatedResult);

                sched.AdvanceToMs(1010);
                Assert.True(fixture.CanExecute(1));
                Assert.True(latestCanExecute);
                Assert.Equal(calculatedResult, 5);

                sched.AdvanceToMs(1200);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);
            });
        }

        [Fact]
        public void DisallowConcurrentExecutionTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = ReactiveCommand.CreateAsyncObservable(Observable.Return(true), 
                    _ => Observable.Return(4).Delay(TimeSpan.FromSeconds(5), sched), 
                    sched);

                Assert.True(fixture.CanExecute(null));

                var result = fixture.CreateCollection();
                Assert.Equal(0, result.Count);

                sched.AdvanceToMs(25);
                Assert.Equal(0, result.Count);

                fixture.Execute(null);
                Assert.False(fixture.CanExecute(null));
                Assert.Equal(0, result.Count);

                sched.AdvanceToMs(2500);
                Assert.False(fixture.CanExecute(null));
                Assert.Equal(0, result.Count);

                sched.AdvanceToMs(5500);
                Assert.True(fixture.CanExecute(null));
                Assert.Equal(1, result.Count);
            });
        }

        [Fact]
        public void CombinedCommandsShouldFireChildCommands()
        {
            var cmd1 = ReactiveCommand.Create();
            var cmd2 = ReactiveCommand.Create();
            var cmd3 = ReactiveCommand.Create();

            var output = new[] { cmd1, cmd2, cmd3, }.Merge().CreateCollection();

            var fixture = ReactiveCommand.CreateCombined(cmd1, cmd2, cmd3);
            Assert.True(fixture.CanExecute(null));
            Assert.Equal(0, output.Count);

            fixture.Execute(42);

            Assert.Equal(3, output.Count);
        }

        [Fact]
        public void CombinedCommandsShouldReflectCanExecuteOfChildren()
        {
            var subj1 = new Subject<bool>();
            var cmd1 = ReactiveCommand.Create(subj1);
            var subj2 = new Subject<bool>();
            var cmd2 = ReactiveCommand.Create(subj2);
            var cmd3 = ReactiveCommand.Create();

            // Initial state for ReactiveCommands is to be executable
            var fixture = ReactiveCommand.CreateCombined(cmd1, cmd2, cmd3);
            var canExecuteOutput = fixture.CanExecuteObservable.CreateCollection();

            // cmd1 and cmd2 are ??? so, result is false
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);

            // 1 is false, 2 is true
            subj1.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);
            Assert.Equal(false, canExecuteOutput[0]);

            // 1 is false, 2 is false
            subj2.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);

            // 1 is true, 2 is false
            subj1.OnNext(true);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);
                        
            // 1 is true, 2 is true
            subj2.OnNext(true);
            Assert.True(fixture.CanExecute(null));
            Assert.Equal(2, canExecuteOutput.Count);
            Assert.Equal(true, canExecuteOutput[1]);
        }

        [Fact]
        public void CombinedCommandsShouldBeInactiveOnAsyncInflightOps()
        {
            (new TestScheduler()).With(sched => {
                var cmd1 = ReactiveCommand.CreateAsyncObservable(Observable.Return(true), 
                    x => Observable.Return(x).Delay(TimeSpan.FromMilliseconds(100), sched));
                var cmd2 = ReactiveCommand.CreateAsyncObservable(Observable.Return(true),
                    x => Observable.Return(x).Delay(TimeSpan.FromMilliseconds(300), sched));

                var cmd3 = ReactiveCommand.Create();

                var result1 = cmd1.CreateCollection();

                var result2 = cmd2.CreateCollection();

                var fixture = ReactiveCommand.CreateCombined(cmd1, cmd2, cmd3);
                var canExecuteOutput = fixture.CanExecuteObservable.CreateCollection();

                Assert.True(fixture.CanExecute(null));
                Assert.Equal(0, canExecuteOutput.Count);

                fixture.Execute(42);

                // NB: The first two canExecuteOutputs are because of the initial value
                // that shows up because we finally ran the scheduler
                sched.AdvanceToMs(50.0);
                Assert.Equal(2, canExecuteOutput.Count);
                Assert.Equal(true, canExecuteOutput[0]);
                Assert.Equal(false, canExecuteOutput[1]);
                Assert.Equal(false, fixture.CanExecute(null));
                Assert.Equal(0, result1.Count);
                Assert.Equal(0, result2.Count);

                sched.AdvanceToMs(250.0);
                Assert.Equal(2, canExecuteOutput.Count);
                Assert.Equal(false, fixture.CanExecute(null));
                Assert.Equal(1, result1.Count);
                Assert.Equal(0, result2.Count);
                                
                sched.AdvanceToMs(500.0);
                Assert.Equal(3, canExecuteOutput.Count);
                Assert.Equal(true, canExecuteOutput[2]);
                Assert.Equal(true, fixture.CanExecute(null));
                Assert.Equal(1, result1.Count);
                Assert.Equal(1, result2.Count);
            });
        }
                
        [Fact]
        public void CombinedCommandsShouldReflectParentCanExecute()
        {
            var subj1 = new Subject<bool>();
            var cmd1 = ReactiveCommand.Create(subj1);
            var subj2 = new Subject<bool>();
            var cmd2 = ReactiveCommand.Create(subj2);
            var cmd3 = ReactiveCommand.Create();
            var parentSubj = new Subject<bool>();

            // Initial state for ReactiveCommands is to be executable
            var fixture = ReactiveCommand.CreateCombined(parentSubj, cmd1, cmd2, cmd3);
            var canExecuteOutput = fixture.CanExecuteObservable.CreateCollection();
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);

            parentSubj.OnNext(false);

            // 1 is false, 2 is true
            subj1.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);
            Assert.Equal(false, canExecuteOutput[0]);

            // 1 is false, 2 is false
            subj2.OnNext(false);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);

            // 1 is true, 2 is false
            subj1.OnNext(true);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);
                        
            // 1 is true, 2 is true, but it doesn't matter because
            // parent is still false
            subj2.OnNext(true);
            Assert.False(fixture.CanExecute(null));
            Assert.Equal(1, canExecuteOutput.Count);

            // Parent is finally true, mark it true
            parentSubj.OnNext(true);
            Assert.True(fixture.CanExecute(null));
            Assert.Equal(2, canExecuteOutput.Count);
            Assert.Equal(true, canExecuteOutput[1]);
        }

        [Fact]
        public void TaskExceptionsShouldBeMarshaledToThrownExceptions()
        {
            (new TestScheduler()).With(sched => {
                var fixture = ReactiveCommand.CreateAsyncTask(Observable.Return(true), async _ => {
                    await Observable.Timer(TimeSpan.FromMilliseconds(50), RxApp.TaskpoolScheduler);
                    throw new Exception("Die");
                    return 5;
                }, sched);

                int result = 0;
                fixture.Subscribe(x => result = x);

                var error = default(Exception);
                fixture.ThrownExceptions.Subscribe(ex => error = ex);

                fixture.Execute(null);

                sched.AdvanceByMs(20);
                Assert.Null(error);
                Assert.Equal(0, result);

                // NB: We have to Thread.Sleep here to compensate for not being
                // able to control the concurrency of Task
                sched.AdvanceByMs(100);
                Thread.Sleep(100);

                // NB: Advance it one more so that the scheduled ThrownExceptions
                // end up being dispatched
                sched.AdvanceByMs(10);

                Assert.NotNull(error);
                Assert.Equal(0, result);
            });
        }

        [Fact]
        public async Task IsExecutingIsFalseAfterAwaitingCommand()
        {
            var command = ReactiveCommand.CreateAsyncTask(_ => Task.Run(() => Thread.Sleep(10)));
            var isExecutingStates = new List<bool>();
            command.IsExecuting.Subscribe(isExecutingStates.Add);

            await command.ExecuteAsync();

            Assert.Equal(3, isExecutingStates.Count);
            Assert.Equal(false, isExecutingStates[0]);
            Assert.Equal(true, isExecutingStates[1]);
            Assert.Equal(false, isExecutingStates[2]);
        }
    }
}