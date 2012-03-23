using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Reactive.Testing;
using ReactiveUI.Xaml;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests
{
    public abstract class ReactiveCommandInterfaceTest
    {
        protected abstract IReactiveCommand createCommand(IObservable<bool> canExecute, IScheduler scheduler = null);
        protected abstract IReactiveCommand createRelayCommand(Func<object, bool> canExecute, IScheduler scheduler = null);

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
                sched.RunToMilliseconds(10 * 1000);
                return changes_as_observable;
            });

            input.DistinctUntilChanged().AssertAreEqual(result.ToList());
        }

        [Fact]
        public void ObservableCanExecuteFuncShouldShowUpInCommand()
        {
            int counter = 0;
            var fixture = createRelayCommand(_ => (counter % 2 == 0));
            var changes_as_observable = fixture.CanExecuteObservable.CreateCollection();

            int change_event_count = 0;
            fixture.CanExecuteChanged += (o, e) => { change_event_count++; };
            Enumerable.Range(0, 6).Run(x => {
                Assert.Equal(x % 2 == 0, fixture.CanExecute(null));
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
                .Zip(observed_params, (expected, actual) => new { expected, actual })
                .Do(Console.WriteLine)
                .Subscribe(x => Assert.Equal(x.expected, x.actual));
        }

        [Fact]
        public void MultipleSubscribesShouldntResultInMultipleNotifications()
        {
            var input = new[] { 1, 2, 1, 2 };
            var sched = new TestScheduler();
            var fixture = createCommand(null, sched);

            var odd_list = new List<int>();
            var even_list = new List<int>();
            fixture.Where(x => ((int)x) % 2 != 0).Subscribe(x => odd_list.Add((int)x));
            fixture.Where(x => ((int)x) % 2 == 0).Subscribe(x => even_list.Add((int)x));

            input.Run(x => fixture.Execute(x));
            sched.RunToMilliseconds(1000);

            new[]{1,1}.AssertAreEqual(odd_list);
            new[]{2,2}.AssertAreEqual(even_list);
        }

        [Fact(Skip="I'm not convinced this is actually true, if you throw in a Subscribe it *should* permabreak")]
        public void ActionExceptionShouldntPermabreakCommands()
        {
            var input = new[] {1,2,3,4};
            var fixture = createCommand(null);
            fixture.Subscribe(x => {
                if (((int)x) == 2)
                    throw new Exception("Die!");
            });

            var exception_list = new List<Exception>();
            var out_list = new List<int>();

            fixture.Subscribe(x => out_list.Add((int)x), ex => exception_list.Add(ex));
            bool we_threw = false;
            foreach (int i in input) {
                try {
                    fixture.Execute(i);
                } catch {
                    we_threw = true;
                    if (i != 2)
                        throw;
                }
            }

            Assert.True(we_threw);
            input.AssertAreEqual(out_list);

            // Now, make sure that the command isn't broken
            fixture.Execute(5);
            Console.WriteLine(String.Join(",", out_list.Select(x => x.ToString()).ToArray()));
            Assert.Equal(5, out_list.Count);
        }

        [Fact]
        public void CanExecuteExceptionShouldntPermabreakCommands()
        {
        }
    }

    public class ReactiveCommandTest : ReactiveCommandInterfaceTest
    {
        protected override IReactiveCommand createCommand(IObservable<bool> canExecute, IScheduler scheduler = null) {
            return new ReactiveCommand(canExecute, scheduler);
        }

        protected override IReactiveCommand createRelayCommand(Func<object, bool> canExecute, IScheduler scheduler = null) {
            return ReactiveCommand.Create(canExecute, null, scheduler);
        }
    }

    public class ReactiveAsyncCommandBaseTest : ReactiveCommandInterfaceTest
    {
        protected override IReactiveCommand createCommand(IObservable<bool> canExecute, IScheduler scheduler = null) {
            return new ReactiveAsyncCommand(canExecute, 1, scheduler);
        }

        protected override IReactiveCommand createRelayCommand(Func<object, bool> canExecute, IScheduler scheduler = null) {
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
                ReactiveCollection<int> results;

                results = fixture.RegisterAsyncObservable(_ => 
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(5), sched)).CreateCollection();

                var inflightResults = fixture.ItemsInflight.CreateCollection();
                sched.RunToMilliseconds(10);
                Assert.True(fixture.CanExecute(null));

                fixture.Execute(null);
                sched.RunToMilliseconds(1005);
                Assert.False(fixture.CanExecute(null));

                sched.RunToMilliseconds(5100);
                Assert.True(fixture.CanExecute(null));

                new[] {0,1,0}.AssertAreEqual(inflightResults);
                new[] {5}.AssertAreEqual(results);
            });
        }

        [Fact]
        public void RegisterMemoizedFunctionSmokeTest()
        {
            var input = new[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
            var output = new[] { 5, 5, 5, 5, 5, 10, 10, 10, 10, 10 };
            var sched = new EventLoopScheduler();
            var results = new List<Timestamped<int>>();

            var start = sched.Now;
            sched.With(_ => {
                var fixture = new ReactiveAsyncCommand(null, 5, sched);

                fixture.RegisterMemoizedFunction(x => { Thread.Sleep(1000); return ((int) x) * 5; }, 50, null, sched)
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
                bool[] subscribers = new[] { false, false, false, false, false };
    			
    			var output = fixture.RegisterAsyncObservable(_ => 
    				Observable.Return(5).Delay(TimeSpan.FromMilliseconds(5000), sched));
                output.Subscribe(x => results.Add(x));

                Enumerable.Range(0, 5).Run(x => output.Subscribe(_ => subscribers[x] = true));
                
                Assert.True(fixture.CanExecute(null));

                fixture.Execute(null);
                sched.RunToMilliseconds(2000);
                Assert.False(fixture.CanExecute(null));

                sched.RunToMilliseconds(6000);
                Assert.True(fixture.CanExecute(null));

                Assert.True(results.Count == 1);
                Assert.True(results[0] == 5);
                Assert.True(subscribers.All(x => x == true));
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
            var deferred = RxApp.DeferredScheduler;
            var taskpool = RxApp.TaskpoolScheduler;

            try {
                var testDeferred = new CountingTestScheduler(Scheduler.Immediate);
                var testTaskpool = new CountingTestScheduler(Scheduler.NewThread);
                RxApp.DeferredScheduler = testDeferred; RxApp.TaskpoolScheduler = testTaskpool;

                var fixture = new ReactiveAsyncCommand();
                var result = fixture.RegisterAsyncFunction(x => { Thread.Sleep(1000); return (int)x * 5; });

                fixture.Execute(1);
                Assert.Equal(5, result.First());

                Assert.True(testDeferred.ScheduledItems.Count >= 1);
                Assert.True(testTaskpool.ScheduledItems.Count >= 1);
            } finally {
                RxApp.DeferredScheduler = deferred;
                RxApp.TaskpoolScheduler = taskpool;
            }
        }

        [Fact]
        public void RAOShouldActuallyRunOnTheTaskpool()
        {
            var deferred = RxApp.DeferredScheduler;
            var taskpool = RxApp.TaskpoolScheduler;

            try {
                var testDeferred = new CountingTestScheduler(Scheduler.Immediate);
                var testTaskpool = new CountingTestScheduler(Scheduler.NewThread);
                RxApp.DeferredScheduler = testDeferred; RxApp.TaskpoolScheduler = testTaskpool;

                var fixture = new ReactiveAsyncCommand();
                var result = fixture.RegisterAsyncObservable(x => 
                    Observable.Return((int)x * 5).Delay(TimeSpan.FromSeconds(1), RxApp.TaskpoolScheduler));

                fixture.Execute(1);
                Assert.Equal(5, result.First());

                Assert.True(testDeferred.ScheduledItems.Count >= 1);
                Assert.True(testTaskpool.ScheduledItems.Count >= 1);
            } finally {
                RxApp.DeferredScheduler = deferred;
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
                    Observable.Return((int)x * 5).Delay(TimeSpan.FromMilliseconds(900), RxApp.DeferredScheduler))
                    .Subscribe(x => calculatedResult = x);

                fixture.CanExecuteObservable.Subscribe(x => latestCanExecute = x);

                // CanExecute should be true, both input observable is true
                // and we don't have anything inflight
                sched.RunToMilliseconds(10);
                Assert.True(fixture.CanExecute(1));
                Assert.True(latestCanExecute);

                // Invoke a command 10ms in
                fixture.Execute(1);

                // At 300ms, input is false
                sched.RunToMilliseconds(300);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);

                // At 600ms, input is true, but the command is still running
                sched.RunToMilliseconds(600);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);

                // After we've completed, we should still be false, since from
                // 750ms-1000ms the input observable is false
                sched.RunToMilliseconds(900);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);
                Assert.Equal(-1, calculatedResult);

                sched.RunToMilliseconds(1010);
                Assert.True(fixture.CanExecute(1));
                Assert.True(latestCanExecute);
                Assert.Equal(calculatedResult, 5);

                sched.RunToMilliseconds(1200);
                Assert.False(fixture.CanExecute(1));
                Assert.False(latestCanExecute);
            });
        }
    }
}