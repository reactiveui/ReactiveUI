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
            var fixture = new ReactiveCommand(null, Scheduler.Immediate);
            Assert.IsTrue(fixture.CanExecute(null));

            string result = null;
            fixture.Subscribe(x => result = x as string);

            fixture.Execute("Test");
            Assert.AreEqual("Test", result);
            fixture.Execute("Test2");
            Assert.AreEqual("Test2", result);
        }

        [TestMethod()]
        public void ObservableCanExecuteShouldShowUpInCommand()
        {
            var can_execute = new Subject<bool>();
            var fixture = new ReactiveCommand(can_execute, null);
            var changes_as_observable = new ListObservable<bool>(fixture.CanExecuteObservable);

            var input = new[] { true, false, false, true, false, true };

            int change_event_count = 0;
            fixture.CanExecuteChanged += (o, e) => { change_event_count++; };
            input.Run(x => {
                can_execute.OnNext(x);
                Assert.AreEqual(x, fixture.CanExecute(null));
            });

            // N.B. We check against '5' instead of 6 because we're supposed to 
            // suppress changes that aren't actually changes i.e. false => false
            can_execute.OnCompleted();
            Assert.AreEqual(5, change_event_count);

            input.Zip(changes_as_observable.ToList(), (expected, actual) => new { expected, actual })
                 .Do(Console.WriteLine)
                 .Run(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod()]
        public void ObservableCanExecuteFuncShouldShowUpInCommand()
        {
            int counter = 1;
            var fixture = new ReactiveCommand(_ => (++counter % 2 == 0), null);
            var changes_as_observable = new ListObservable<bool>(fixture.CanExecuteObservable);

            int change_event_count = 0;
            fixture.CanExecuteChanged += (o, e) => { change_event_count++; };
            Enumerable.Range(0, 6).Run(x => {
                Assert.AreEqual(x % 2 == 0, fixture.CanExecute(null));
            });

            Assert.AreEqual(6, change_event_count);
        }


        [TestMethod()]
        public void ObservableExecuteFuncShouldBeObservableAndAct()
        {
            var executed_params = new List<object>();
            var fixture = new ReactiveCommand(x => executed_params.Add(x));

            var observed_params = new ReplaySubject<object>();
            fixture.Subscribe(observed_params.OnNext, observed_params.OnError, observed_params.OnCompleted);

            var range = Enumerable.Range(0, 5);
            range.Run(x => fixture.Execute(x));

            range.Zip(executed_params, (expected, actual) => new { expected, actual })
                 .Run(x => Assert.AreEqual(x.expected, x.actual));

            range.ToObservable()
                .Zip(observed_params, (expected, actual) => new { expected, actual })
                .Do(Console.WriteLine)
                .Subscribe(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod]
        public void MultipleSubscribesShouldntResultInMultipleNotifications()
        {
            var input = new[] { 1, 2, 1, 2 };
            var fixture = new ReactiveCommand(null, null);

            var odd_list = new List<int>();
            var even_list = new List<int>();
            fixture.Where(x => ((int)x) % 2 != 0).Subscribe(x => odd_list.Add((int)x));
            fixture.Where(x => ((int)x) % 2 == 0).Subscribe(x => even_list.Add((int)x));

            input.Run(x => fixture.Execute(x));

            new[]{1,1}.Zip(odd_list, (expected, actual) => new { expected, actual })
                .Run(x => Assert.AreEqual(x.expected, x.actual));

            new[]{2,2}.Zip(even_list, (expected, actual) => new { expected, actual })
                .Run(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod]
        public void ActionExceptionShouldntPermabreakCommands()
        {
            var input = new[] {1,2,3,4};
            var fixture = new ReactiveCommand(x => {
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
            input.Zip(out_list, (expected, actual) => new { expected, actual })
                 .Run(x => Assert.AreEqual(x.expected, x.actual));

            // Now, make sure that the command isn't broken
            fixture.Execute(5);
            Console.WriteLine(String.Join(",", out_list));
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
            var inflight_results = new List<int>();
            var fixture = new ReactiveAsyncCommand(null, 1, Scheduler.Immediate);

            var async_data = fixture
                .ObserveOn(Scheduler.NewThread)
                .Delay(new TimeSpan(0, 0, 5))
                .Select(_ => 5)
                .Do(_ => fixture.AsyncCompletedNotification.OnNext(new Unit()));

            fixture.ItemsInflight.Subscribe(x => inflight_results.Add(x));

            async_data.Subscribe();

            Assert.IsTrue(fixture.CanExecute(null));

            fixture.Execute(null);
            Assert.IsFalse(fixture.CanExecute(null));

            Thread.Sleep(5500);
            Assert.IsTrue(fixture.CanExecute(null));

            new[] {0,1,0}.Zip(inflight_results, (expected, actual) => new {expected, actual})
                         .Run(x => Assert.AreEqual(x.expected, x.actual));
        }


        [TestMethod]
        public void RegisterAsyncFunctionSmokeTest()
        {
            var inflight_results = new List<int>();
            var fixture = new ReactiveAsyncCommand(null, 1);
            var results = new List<int>();

            fixture.RegisterAsyncFunction(_ => { Thread.Sleep(2000); return 5; })
                   .Subscribe(x => results.Add(x));

            fixture.ItemsInflight.Subscribe(x => inflight_results.Add(x));

            Assert.IsTrue(fixture.CanExecute(null));

            fixture.Execute(null);
            Assert.IsFalse(fixture.CanExecute(null));

            Thread.Sleep(6000);
            Assert.IsTrue(fixture.CanExecute(null));

            new[] {0,1,0}.Zip(inflight_results, (expected, actual) => new {expected, actual})
                         .Run(x => Assert.AreEqual(x.expected, x.actual));

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0] == 5);
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

            output.Zip(results.Select(x => x.Value), (expected, actual) => new { expected, actual })
                  .Run(x => Assert.AreEqual(x.expected, x.actual));

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

            output.Zip(results.Select(x => x.Value), (expected, actual) => new { expected, actual })
                  .Run(x => Assert.AreEqual(x.expected, x.actual));

            Assert.IsTrue(results.Count == 8);

            Assert.IsTrue(released.Count == 1);
            Assert.IsTrue(released[0] == 2*5);
        }

        [TestMethod]
        public void MultipleSubscribersShouldntDecrementRefcountBelowZero()
        {
            var fixture = new ReactiveAsyncCommand(null, 1);
            var results = new List<int>();
            bool[] subscribers = new[] { false, false, false, false, false };

            var output = fixture.RegisterAsyncFunction(_ => { Thread.Sleep(2000); return 5; });
            output.Subscribe(x => results.Add(x));

            Enumerable.Range(0, 5).Run(x => output.Subscribe(_ => subscribers[x] = true));

            Assert.IsTrue(fixture.CanExecute(null));

            fixture.Execute(null);
            Assert.IsFalse(fixture.CanExecute(null));

            Thread.Sleep(6000);
            Assert.IsTrue(fixture.CanExecute(null));

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0] == 5);
            Assert.IsTrue(subscribers.All(x => x == true));
        }
    }
}
