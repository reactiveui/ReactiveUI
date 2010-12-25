using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Concurrency;
using System.Collections.Generic;
using System.Threading;

namespace ReactiveXaml.Tests
{
    [TestClass()]
    public class ReactiveCommandTest : IEnableLogger
    {
        [TestMethod()]
        public void CompletelyDefaultReactiveCommandShouldFire()
        {
            var sched = new TestScheduler();
            var fixture = new ReactiveCommand(null, sched);
            Assert.IsTrue(fixture.CanExecute(null));

            string result = null;
            fixture.Subscribe(x => result = x as string);

            fixture.Execute("Test");
            sched.Run();
            Assert.AreEqual("Test", result);
            fixture.Execute("Test2");
            sched.Run();
            Assert.AreEqual("Test2", result);
        }

        [TestMethod()]
        public void ObservableCanExecuteShouldShowUpInCommand()
        {
            var input = new[] {true, false, false, true, false, true};
            var result = (new TestScheduler()).With(sched => {
                var can_execute = new Subject<bool>();
                var fixture = new ReactiveCommand(can_execute, null);
                var changes_as_observable = new ListObservable<bool>(fixture.CanExecuteObservable);

                int change_event_count = 0;
                fixture.CanExecuteChanged += (o, e) => { change_event_count++; };
                input.Run(x => {
                    can_execute.OnNext(x);
                    sched.Run();
                    Assert.AreEqual(x, fixture.CanExecute(null));
                });

                // N.B. We check against '5' instead of 6 because we're supposed to 
                // suppress changes that aren't actually changes i.e. false => false
                can_execute.OnCompleted();
                sched.Run();
                Assert.AreEqual(5, change_event_count);

                return changes_as_observable;
            });

            input.AssertAreEqual(result.ToList());
        }

        [TestMethod()]
        public void ObservableCanExecuteFuncShouldShowUpInCommand()
        {
            int counter = 1;
            var sched = new TestScheduler();
            var fixture = ReactiveCommand.Create(_ => (++counter % 2 == 0), null, sched);
            var changes_as_observable = new ListObservable<bool>(fixture.CanExecuteObservable);

            int change_event_count = 0;
            fixture.CanExecuteChanged += (o, e) => { change_event_count++; };
            Enumerable.Range(0, 6).Run(x => {
                sched.Run();
                this.Log().InfoFormat("Counter = {0}, x = {1}", counter, x);
                Assert.AreEqual(x % 2 == 0, fixture.CanExecute(null));
            });

            sched.Run();
            Assert.AreEqual(6, change_event_count);
        }


        [TestMethod()]
        public void ObservableExecuteFuncShouldBeObservableAndAct()
        {
            var executed_params = new List<object>();
            var fixture = ReactiveCommand.Create(null, x => executed_params.Add(x));

            var observed_params = new ReplaySubject<object>();
            fixture.Subscribe(observed_params.OnNext, observed_params.OnError, observed_params.OnCompleted);

            var range = Enumerable.Range(0, 5);
            range.Run(x => fixture.Execute(x));

            range.AssertAreEqual(executed_params.OfType<int>());

            range.ToObservable()
                .Zip(observed_params, (expected, actual) => new { expected, actual })
                .Do(Console.WriteLine)
                .Subscribe(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod]
        public void MultipleSubscribesShouldntResultInMultipleNotifications()
        {
            var input = new[] { 1, 2, 1, 2 };
            var sched = new TestScheduler();
            var fixture = new ReactiveCommand(null, sched);

            var odd_list = new List<int>();
            var even_list = new List<int>();
            fixture.Where(x => ((int)x) % 2 != 0).Subscribe(x => odd_list.Add((int)x));
            fixture.Where(x => ((int)x) % 2 == 0).Subscribe(x => even_list.Add((int)x));

            input.Run(x => fixture.Execute(x));
            sched.RunToMilliseconds(1000);

            new[]{1,1}.AssertAreEqual(odd_list);
            new[]{2,2}.AssertAreEqual(even_list);
        }

        [TestMethod]
        public void ActionExceptionShouldntPermabreakCommands()
        {
            var input = new[] {1,2,3,4};
            var fixture = ReactiveCommand.Create(null, x => {
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

            Assert.IsTrue(we_threw);
            input.AssertAreEqual(out_list);

            // Now, make sure that the command isn't broken
            fixture.Execute(5);
            Console.WriteLine(String.Join(",", out_list.Select(x => x.ToString()).ToArray()));
            Assert.AreEqual(5, out_list.Count);
        }

        [TestMethod]
        public void CanExecuteExceptionShouldntPermabreakCommands()
        {
        }
    }

    [TestClass()]
    public class ReactiveAsyncCommandTest : IEnableLogger
    {
        [TestMethod()]
        public void AsyncCommandSmokeTest()
        {
            var sched = new TestScheduler();
            IObservable<int> async_data;
            ReactiveAsyncCommand fixture;

            using (TestUtils.WithTestScheduler(sched)) {
                fixture = new ReactiveAsyncCommand(null, 1);
                async_data = fixture
                    .Delay(TimeSpan.FromSeconds(5), RxApp.TaskpoolScheduler)
                    .Select(_ => 5)
                    .Do(_ => fixture.AsyncCompletedNotification.OnNext(new Unit()));
            }

            var inflight_results = new List<int>();
            fixture.ItemsInflight.Subscribe(inflight_results.Add);

            var output = new List<int>();
            async_data.Subscribe(output.Add);

            Assert.IsTrue(fixture.CanExecute(null));

            fixture.Execute(null);

            sched.RunToMilliseconds(1005);
            Assert.IsFalse(fixture.CanExecute(null));

            sched.RunToMilliseconds(5005);
            Assert.IsTrue(fixture.CanExecute(null));

            new[] {0,1,0}.AssertAreEqual(inflight_results);
            new[] {5}.AssertAreEqual(output);
        }


        [TestMethod]
        public void RegisterAsyncFunctionSmokeTest()
        {
            var sched = new TestScheduler();
            var fixture = sched.With(_ => new ReactiveAsyncCommand(null, 1));
            ReactiveCollection<int> results;

            using (TestUtils.WithTestScheduler(sched)) {
                results = fixture.RegisterAsyncObservable(_ => 
                    Observable.Return(5).Delay(TimeSpan.FromSeconds(5), sched)
                ).CreateCollection();
            }

            var inflight_results = sched.With(_ => fixture.ItemsInflight.CreateCollection());
            sched.RunToMilliseconds(10);
            Assert.IsTrue(fixture.CanExecute(null));

            fixture.Execute(null);
            sched.RunToMilliseconds(1005);
            Assert.IsFalse(fixture.CanExecute(null));

            sched.RunToMilliseconds(5005);
            Assert.IsTrue(fixture.CanExecute(null));

            new[] {0,1,0}.AssertAreEqual(inflight_results);
            new[] {5}.AssertAreEqual(results);
        }

        [TestMethod]
        public void RegisterMemoizedFunctionSmokeTest()
        {
            var input = new[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
            var output = new[] { 5, 5, 5, 5, 5, 10, 10, 10, 10, 10 };
            var fixture = new ReactiveAsyncCommand(null, 0);
            var results = new List<Timestamped<int>>();

            fixture.RegisterMemoizedFunction<int>(x => { Thread.Sleep(1000); return ((int)x) * 5; })
                   .Timestamp()
                   .DebugObservable()
                   .Subscribe(x => results.Add(x));

            Assert.IsTrue(fixture.CanExecute(1));

            var start = DateTimeOffset.Now;
            foreach(var i in input) {
                Assert.IsTrue(fixture.CanExecute(i));
                fixture.Execute(i);
            }

            Thread.Sleep(2500);

            Assert.IsTrue(results.Count == 10);

            this.Log().Info("Timestamp Deltas");
            results.Select(x => x.Timestamp - start)
                   .Run(x => this.Log().Info(x));

            output.AssertAreEqual(results.Select(x => x.Value));

            Assert.IsFalse(results.Any(x => x.Timestamp - start > new TimeSpan(0, 0, 3)));
        }

        [TestMethod]
        public void MakeSureMemoizedReleaseFuncGetsCalled()
        {
            var input = new[] { 1, 1, 2, 2, 1, 1, 3, 3 };
            var output = new[] { 5, 5, 10, 10, 5, 5, 15, 15 };

            var fixture = new ReactiveAsyncCommand(null, 0);
            var results = new List<Timestamped<int>>();
            var released = new List<int>();

            fixture.RegisterMemoizedFunction<int>(x => { Thread.Sleep(250); return ((int)x) * 5; }, 2, x => released.Add(x))
                   .Timestamp()
                   .DebugObservable()
                   .Subscribe(x => results.Add(x));

            Assert.IsTrue(fixture.CanExecute(1));

            var start = DateTimeOffset.Now;
            foreach(var i in input) {
                Assert.IsTrue(fixture.CanExecute(i));
                fixture.Execute(i);
            }

            Thread.Sleep(1000);

            this.Log().Info("Timestamp Deltas");
            results.Select(x => x.Timestamp - start)
                   .Run(x => this.Log().Info(x));

            this.Log().Info("Release list");
            released.Run(x => this.Log().Info(x));

            output.AssertAreEqual(results.Select(x => x.Value));

            Assert.IsTrue(results.Count == 8);

            Assert.IsTrue(released.Count == 1);
            Assert.IsTrue(released[0] == 2*5);
        }

        [TestMethod]
        public void MultipleSubscribersShouldntDecrementRefcountBelowZero()
        {
			var sched = new TestScheduler();
            var fixture = new ReactiveAsyncCommand(null, 1, sched);
            var results = new List<int>();
            bool[] subscribers = new[] { false, false, false, false, false };
			
			var output = fixture.RegisterAsyncObservable(_ => 
				Observable.Return(5).Delay(TimeSpan.FromMilliseconds(5000), sched));
            output.Subscribe(x => results.Add(x));

            Enumerable.Range(0, 5).Run(x => output.Subscribe(_ => subscribers[x] = true));
            
            Assert.IsTrue(fixture.CanExecute(null));

            fixture.Execute(null);
            sched.RunToMilliseconds(2000);
            Assert.IsFalse(fixture.CanExecute(null));

            sched.RunToMilliseconds(6000);
            Assert.IsTrue(fixture.CanExecute(null));

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0] == 5);
            Assert.IsTrue(subscribers.All(x => x == true));
        }
    }
}
