using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;

using Microsoft.Reactive.Testing;
using ReactiveUI.Xaml;
using Xunit;
#if MONO
using Mono.Reactive.Testing;
#else

#endif

namespace ReactiveUI.Tests
{
    public abstract class ReactiveCommandInterfaceTest
    {
        protected abstract IReactiveCommand createCommand(IObservable<bool> canExecute, IScheduler scheduler = null);

        protected abstract IReactiveCommand createRelayCommand(Func<object, bool> canExecute,
            IScheduler scheduler = null);

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

            input.DistinctUntilChanged().AssertAreEqual(result.ToList());
        }

        [Fact]
        public void ObservableCanExecuteFuncShouldShowUpInCommand()
        {
            int counter = 0;
            var fixture = createRelayCommand(_ => (counter%2 == 0));
            var changes_as_observable = fixture.CanExecuteObservable.CreateCollection();

            int change_event_count = 0;
            fixture.CanExecuteChanged += (o, e) => { change_event_count++; };
            Enumerable.Range(0, 6).Run(x => {
                Assert.Equal(x%2 == 0, fixture.CanExecute(null));
                counter++;
            });

            Assert.Equal(6, changes_as_observable.Count);
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

            // The command should just latch at whatever its previous state was
            // before the exception
            Assert.True(fixture.CanExecute(null));

            Assert.Equal(1, exceptions.Count);
            Assert.Equal("Aieeeee!", exceptions[0].Message);

            Assert.Equal(false, canExecuteStates[canExecuteStates.Count - 2]);
            Assert.Equal(true, canExecuteStates[canExecuteStates.Count - 1]);
        }

        [Fact]
        public void NoSubscriberOfThrownExceptionsEqualsDeath()
        {
            (new TestScheduler()).With(sched => {
                var canExecute = new Subject<bool>();
                var fixture = createCommand(canExecute);

                canExecute.OnNext(true);
                canExecute.OnError(new Exception("Aieeeee!"));

                bool failed = true;
                try {
                    sched.Start();
                    Assert.True(fixture.CanExecute(null));
                } catch (Exception ex) {
                    failed = (ex.InnerException.Message != "Aieeeee!");
                }

                Assert.False(failed);
            });
        }
    }

    public class ReactiveCommandTest : ReactiveCommandInterfaceTest
    {
        protected override IReactiveCommand createCommand(IObservable<bool> canExecute, IScheduler scheduler = null)
        {
            return new ReactiveCommand(canExecute, scheduler);
        }

        protected override IReactiveCommand createRelayCommand(Func<object, bool> canExecute,
            IScheduler scheduler = null)
        {
            return ReactiveCommand.Create(canExecute, null, scheduler);
        }

        public class TheCreateCommandMethod
        {
            [Fact]
            public void CreatesCommandThatHandlesThrownExceptions()
            {
                var command = ReactiveCommand.Create(_ => true, _ => { throw new Exception(); });
                Assert.NotNull(command.ThrownExceptions);
                bool handled = false;
                command.ThrownExceptions.Subscribe(e => handled = true);
                command.Execute(null);
                Assert.True(handled);
            }
        }
    }

    public class ReactiveAsyncCommandBaseTest : ReactiveCommandInterfaceTest
    {
        protected override IReactiveCommand createCommand(IObservable<bool> canExecute, IScheduler scheduler = null)
        {
            return new ReactiveAsyncCommand(canExecute, 1, scheduler);
        }

        protected override IReactiveCommand createRelayCommand(Func<object, bool> canExecute,
            IScheduler scheduler = null)
        {
            return ReactiveAsyncCommand.Create(x => 1, x => { }, canExecute, 1, scheduler);
        }
    }

    public class ReactiveAsyncCommandTest
    {
        [Fact]
        public void RegisterAsyncFunctionSmokeTest()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new ReactiveAsyncCommand(null, 1);
                ReactiveList<int> results;

                results = fixture.RegisterAsyncObservable(_ =>
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(5), sched)).CreateCollection();

                var inflightResults = fixture.ItemsInflight.CreateCollection();
                sched.AdvanceToMs(10);
                Assert.True(fixture.CanExecute(null));

                fixture.Execute(null);
                sched.AdvanceToMs(1005);
                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(5100);
                Assert.True(fixture.CanExecute(null));

                new[] {0, 1, 0}.AssertAreEqual(inflightResults);
                new[] {5}.AssertAreEqual(results);
            });
        }

        [Fact]
        public void RegisterMemoizedFunctionSmokeTest()
        {
            var input = new[] {1, 1, 1, 1, 1, 2, 2, 2, 2, 2};
            var output = new[] {5, 5, 5, 5, 5, 10, 10, 10, 10, 10};
            var sched = new EventLoopScheduler();
            var results = new List<Timestamped<int>>();

            var start = sched.Now;
            sched.With(_ => {
                var fixture = new ReactiveAsyncCommand(null, 5, sched);

                fixture.RegisterMemoizedFunction(x => {
                    Thread.Sleep(1000);
                    return ((int)x)*5;
                }, 50, null, sched)
                    .Timestamp()
                    .Subscribe(x => results.Add(x));

                Assert.True(fixture.CanExecute(1));

                foreach (var i in input) {
                    Assert.True(fixture.CanExecute(i));
                    fixture.Execute(i);
                }

                Thread.Sleep(2500);
            });

            Assert.Equal(10, results.Count);

            results.Select(x => x.Timestamp - start)
                .Run(x => { });

            output.AssertAreEqual(results.Select(x => x.Value));

            Assert.False(results.Any(x => x.Timestamp - start > new TimeSpan(0, 0, 3)));
        }

        [Fact]
        public void MultipleSubscribersShouldntDecrementRefcountBelowZero()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new ReactiveAsyncCommand();
                var results = new List<int>();
                bool[] subscribers = new[] {false, false, false, false, false};

                var output = fixture.RegisterAsyncObservable(_ =>
                    Observable.Return(5).Delay(TimeSpan.FromMilliseconds(5000), sched));
                output.Subscribe(x => results.Add(x));

                Enumerable.Range(0, 5).Run(x => output.Subscribe(_ => subscribers[x] = true));

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
                int latestInFlight = 0;
                var fixture = new ReactiveAsyncCommand(null, 1, sched);

                var results = fixture
                    .RegisterAsyncObservable(_ => new[] {1, 2, 3}.ToObservable())
                    .CreateCollection();
                fixture.ItemsInflight.Subscribe(x => latestInFlight = x);


                fixture.Execute(1);
                sched.Start();

                Assert.Equal(3, results.Count);
                Assert.Equal(0, latestInFlight);
            });
        }

        [Fact]
        public void RAFShouldActuallyRunOnTheTaskpool()
        {
            var deferred = RxApp.MainThreadScheduler;
            var taskpool = RxApp.TaskpoolScheduler;

            try {
                var testDeferred = new CountingTestScheduler(Scheduler.Immediate);
                var testTaskpool = new CountingTestScheduler(Scheduler.NewThread);
                RxApp.MainThreadScheduler = testDeferred;
                RxApp.TaskpoolScheduler = testTaskpool;

                var fixture = new ReactiveAsyncCommand();
                var result = fixture.RegisterAsyncFunction(x => {
                    Thread.Sleep(1000);
                    return (int)x*5;
                });

                fixture.Execute(1);
                Assert.Equal(5, result.First());

                Assert.True(testDeferred.ScheduledItems.Count >= 1);
                Assert.True(testTaskpool.ScheduledItems.Count >= 1);
            } finally {
                RxApp.MainThreadScheduler = deferred;
                RxApp.TaskpoolScheduler = taskpool;
            }
        }

        [Fact]
        public void RAOShouldActuallyRunOnTheTaskpool()
        {
            var deferred = RxApp.MainThreadScheduler;
            var taskpool = RxApp.TaskpoolScheduler;

            try {
                var testDeferred = new CountingTestScheduler(Scheduler.Immediate);
                var testTaskpool = new CountingTestScheduler(Scheduler.NewThread);
                RxApp.MainThreadScheduler = testDeferred;
                RxApp.TaskpoolScheduler = testTaskpool;

                var fixture = new ReactiveAsyncCommand();
                var result = fixture.RegisterAsyncObservable(x =>
                    Observable.Return((int)x*5).Delay(TimeSpan.FromSeconds(1), RxApp.TaskpoolScheduler));

                fixture.Execute(1);
                Assert.Equal(5, result.First());

                Assert.True(testDeferred.ScheduledItems.Count >= 1);
                Assert.True(testTaskpool.ScheduledItems.Count >= 1);
            } finally {
                RxApp.MainThreadScheduler = deferred;
                RxApp.TaskpoolScheduler = taskpool;
            }
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

                var fixture = new ReactiveAsyncCommand(canExecute);
                int calculatedResult = -1;
                bool latestCanExecute = false;

                fixture.RegisterAsyncObservable(x =>
                    Observable.Return((int)x*5).Delay(TimeSpan.FromMilliseconds(900), RxApp.MainThreadScheduler))
                    .Subscribe(x => calculatedResult = x);

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



        private Task<bool> DummyTestFunction()
        {
            return Task.Factory.StartNew(() => true);
        }

        [Fact]
        public void ReactiveAsyncCommandInitialConditionDefaultBehavior()
        {
            (new TestScheduler()).With(sched => {
                var canExecute = sched.CreateHotObservable(
                    sched.OnNextAt(0, false),
                    sched.OnNextAt(250, true)
                    );

                var fixture = new ReactiveAsyncCommand(canExecute);

                fixture.RegisterAsyncTask(_ => DummyTestFunction());

                Assert.True(fixture.CanExecute(null));

                sched.AdvanceToMs(10);
                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(255);
                Assert.True(fixture.CanExecute(null));
            });
        }


        [Fact]
        public void ReactiveAsyncCommandInitialConditionNewBehavior()
        {
            (new TestScheduler()).With(sched =>
            {
                var canExecute = sched.CreateHotObservable(
                    sched.OnNextAt(0, false),
                    sched.OnNextAt(250, true)
                   );

                var fixture = new ReactiveAsyncCommand(canExecute, initialCondition:false);
                
                fixture.RegisterAsyncTask(_ => DummyTestFunction());

                Assert.False(fixture.CanExecute(null));
                
                sched.AdvanceToMs(10);
                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(255);
                Assert.True(fixture.CanExecute(null));              
            });
        }

        [Fact]
        public void ReactiveCommandInitialConditionDefaultBehavior()
        {
            (new TestScheduler()).With(sched =>
            {
                var canExecute = sched.CreateHotObservable(
                    sched.OnNextAt(0, false),
                    sched.OnNextAt(250, true)
                    );

                var fixture = new ReactiveCommand(canExecute);

                Assert.True(fixture.CanExecute(null));

                sched.AdvanceToMs(10);
                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(255);
                Assert.True(fixture.CanExecute(null));
            });
        }


        [Fact]
        public void ReactiveCommandInitialConditionNewBehavior()
        {
            (new TestScheduler()).With(sched =>
            {
                var canExecute = sched.CreateHotObservable(
                    sched.OnNextAt(0, false),
                    sched.OnNextAt(250, true)
                   );

                var fixture = new ReactiveCommand(canExecute, initialCondition:false);

                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(10);
                Assert.False(fixture.CanExecute(null));

                sched.AdvanceToMs(255);
                Assert.True(fixture.CanExecute(null));
            });
        }
    }
}