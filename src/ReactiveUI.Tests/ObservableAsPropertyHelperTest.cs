using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;

using System.Reactive.Disposables;
using Microsoft.Reactive.Testing;
using Splat;

namespace ReactiveUI.Tests
{
    public class ObservableAsPropertyHelperTest
    {
        internal class OAPHTestFixture : ReactiveObject
        {
            private string _text;

            public string Text
            {
                get { return _text; }
                set { this.RaiseAndSetIfChanged(ref _text, value); }
            }

            public string this[string propertyName]
            {
                get { return string.Empty; }
            }

            public OAPHTestFixture()
            {
                var temp = this.WhenAnyValue(f => f.Text)
                       .ToProperty(this, f => f["Whatever"])
                       .Value;
            }
        }

        [Fact]
        public void OAPHShouldFireChangeNotifications()
        {
            var input = new[] {1, 2, 3, 3, 4}.ToObservable();
            var output = new List<int>();

            (new TestScheduler()).With(sched => {
                var fixture = new ObservableAsPropertyHelper<int>(input,
                    x => output.Add(x), -5);

                sched.Start();

                Assert.Equal(input.LastAsync().Wait(), fixture.Value);

                // Note: Why doesn't the list match the above one? We're supposed
                // to suppress duplicate notifications, of course :)
                (new[] { -5, 1, 2, 3, 4 }).AssertAreEqual(output);
            });
        }

        [Fact]
        public void OAPHShouldSkipFirstValueIfItMatchesTheInitialValue()
        {
            var input = new[] { 1, 2, 3 }.ToObservable();
            var output = new List<int>();

            (new TestScheduler()).With(sched => {
                var fixture = new ObservableAsPropertyHelper<int>(input,
                    x => output.Add(x), 1);

                sched.Start();

                Assert.Equal(input.LastAsync().Wait(), fixture.Value);

                (new[] { 1, 2, 3 }).AssertAreEqual(output);
            });
        }

        [Fact]
        public void OAPHShouldProvideInitialValueImmediatelyRegardlessOfScheduler()
        {
            var output = new List<int>();

            (new TestScheduler()).With(sched => {
                var fixture = new ObservableAsPropertyHelper<int>(Observable<int>.Never,
                    x => output.Add(x), 32);

                Assert.Equal(32, fixture.Value);
            });
        }

        [Fact]
        public void OAPHShouldProvideLatestValue()
        {
            var sched = new TestScheduler();
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5, scheduler: sched);

            Assert.Equal(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            sched.Start();
            Assert.Equal(4, fixture.Value);

            input.OnCompleted();
            sched.Start();
            Assert.Equal(4, fixture.Value);
        }

        [Fact]
        public void OAPHShouldSubscribeImmediatelyToSource()
        {
            bool isSubscribed = false;

            var observable = Observable.Create<int>(o =>
            {
                isSubscribed = true;
                o.OnNext(42);
                o.OnCompleted();

                return Disposable.Empty;
            });

            var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0);

            Assert.True(isSubscribed);
            Assert.Equal(42, fixture.Value);
        }

        [Fact]
        public void OAPHDeferSubscriptionParameterDefersSubscriptionToSource()
        {
            bool isSubscribed = false;

            var observable = Observable.Create<int>(o =>
            {
                isSubscribed = true;
                o.OnNext(42);
                o.OnCompleted();

                return Disposable.Empty;
            });

            var fixture = new ObservableAsPropertyHelper<int>(observable, _ => { }, 0, true);

            Assert.False(isSubscribed);
            Assert.Equal(42, fixture.Value);
            Assert.True(isSubscribed);
        }

        [Fact]
        public void OAPHShouldRethrowErrors()
        {
            var input = new Subject<int>();
            var sched = new TestScheduler();

            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, scheduler: sched);
            var errors = new List<Exception>();

            Assert.Equal(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            fixture.ThrownExceptions.Subscribe(errors.Add);

            sched.Start();

            Assert.Equal(4, fixture.Value);

            input.OnError(new Exception("Die!"));

            sched.Start();

            Assert.Equal(4, fixture.Value);
            Assert.Equal(1, errors.Count);
        }

        [Fact]
        public void NoThrownExceptionsSubscriberEqualsOAPHDeath()
        {
            (new TestScheduler()).With(sched => {
                var input = new Subject<int>();
                var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5);
    
                Assert.Equal(-5, fixture.Value);
                (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));
    
                input.OnError(new Exception("Die!"));
    
                bool failed = true;
                try {
                    sched.Start();
                } catch (Exception ex) {
                    failed = ex.InnerException.Message != "Die!";
                }
    
                Assert.False(failed);
                Assert.Equal(4, fixture.Value);
            });
        }

        [Fact]
        public void ToPropertyShouldFireBothChangingAndChanged()
        {
            var fixture = new OaphTestFixture();

            // NB: This is a hack to connect up the OAPH
            var dontcare = (fixture.FirstThreeLettersOfOneWord ?? "").Substring(0,0);

            var resultChanging = fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: true)
                .CreateCollection();
            var resultChanged = fixture.ObservableForProperty(x => x.FirstThreeLettersOfOneWord, beforeChange: false)
                .CreateCollection();

            Assert.Empty(resultChanging);
            Assert.Empty(resultChanged);

            fixture.IsOnlyOneWord = "FooBar";
            Assert.Equal(1, resultChanging.Count);
            Assert.Equal(1, resultChanged.Count);
            Assert.Equal("", resultChanging[0].Value);
            Assert.Equal("Foo", resultChanged[0].Value);

            fixture.IsOnlyOneWord = "Bazz";
            Assert.Equal(2, resultChanging.Count);
            Assert.Equal(2, resultChanged.Count);
            Assert.Equal("Foo", resultChanging[1].Value);
            Assert.Equal("Baz", resultChanged[1].Value);
        }

        [Fact]
        public void ToPropertyShouldSubscribeOnlyOnce()
        {
            using (ProductionMode.Set()) {
                var f = new RaceConditionFixture();
                // This line is important because it triggers connect to
                // be called recursively thus cause the subscription
                // to be called twice. Not sure if this is a reactive UI
                // or RX bug.
                f.PropertyChanged += (e, s) => Console.WriteLine(f.A);

                // Trigger subscription to the underlying observable.
                Assert.Equal(true, f.A);

                Assert.Equal(1, f.Count);
            }
        }

        [Fact]
        public void ToProperty_GivenIndexer_NotifiesOnExpectedPropertyName()
        {
            (new TestScheduler()).With(sched => {
                var fixture = new OAPHTestFixture();
                var propertiesChanged = new List<string>();

                fixture.PropertyChanged += (sender, args) => {
                    propertiesChanged.Add(args.PropertyName);
                };

                fixture.Text = "awesome";

                Assert.Equal(new[] { "Text", "Item[]" }, propertiesChanged);
            });
        }
    }

    class ProductionMode : IModeDetector
    {
        public bool? InUnitTestRunner()
        {
            return false;
        }

        public bool? InDesignMode()
        {
            return false;
        }

        public static IDisposable Set()
        {
            ModeDetector.OverrideModeDetector(new ProductionMode());
            return Disposable.Create(() => ModeDetector.OverrideModeDetector(new PlatformModeDetector()));
        }
    }

    public class RaceConditionFixture : ReactiveObject
    {
        public ObservableAsPropertyHelper<bool> _A;
        public int Count;

        public bool A
        {
            get { return _A.Value; }
        }

        public RaceConditionFixture()
        {
            // We need to generate a value on subscription
            // which is different than the default value.
            // This triggers the property change firing
            // upon subscription in the ObservableAsPropertyHelper
            // constructor.
            Observables
                .True
                .Do(_ => Count++)
                .ToProperty(this, x => x.A, out _A);
        }
    }
}