using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Tests
{
    public class OAPHViewModel : ReactiveObject
    {
        public OAPHViewModel(IObservable<Tuple<int, string>> obs1,
            IObservable<int> obs2)
        {
            obs2.ToProperty(this, x => x.Property2, out _Property2);

            obs1
                .Where(x => x.Item1 == Property2)
                .Select(x => x.Item2)
                .Do(x => Console.WriteLine("Im gonna hit it with {0}!", x))
                .ToProperty(this, x => x.Property1, out _Property1);

            this.WhenAnyValue(x => x.Property2)
                .Select(x =>
                {
                    return "SomeString";
                })
                .ToProperty(this, x => x.Property1, out _Property1);
        }

        private ObservableAsPropertyHelper<string> _Property1;
        public string Property1
        {
            get { return _Property1.Value; }
        }

        private ObservableAsPropertyHelper<int> _Property2;
        public int Property2
        {
            get { return _Property2.Value; }
        }
    }

    public class ObservableAsPropertyHelperTest
    {
        [Fact]
        public void OAPHShouldFireChangeNotifications()
        {
            var input = new[] {1, 2, 3, 3, 4}.ToObservable();
            var output = new List<int>();

            (new TestScheduler()).With(sched => {
                var fixture = new ObservableAsPropertyHelper<int>(input,
                    x => output.Add(x), -5);

                sched.Start();

                Assert.Equal(input.Last(), fixture.Value);

                // Note: Why doesn't the list match the above one? We're supposed
                // to suppress duplicate notifications, of course :)
                (new[] { -5, 1, 2, 3, 4 }).AssertAreEqual(output);
            });
        }

        [Fact]
        public void OAPHShouldProvideLatestValue()
        {
            var sched = new TestScheduler();
            var input = new Subject<int>();

            var fixture = new ObservableAsPropertyHelper<int>(input,
                _ => { }, -5, sched);

            Assert.Equal(-5, fixture.Value);
            (new[] { 1, 2, 3, 4 }).Run(x => input.OnNext(x));

            sched.Start();
            Assert.Equal(4, fixture.Value);

            input.OnCompleted();
            sched.Start();
            Assert.Equal(4, fixture.Value);
        }

        [Fact]
        public void OAPHShouldRethrowErrors()
        {
            var input = new Subject<int>();
            var sched = new TestScheduler();

            var fixture = new ObservableAsPropertyHelper<int>(input, _ => { }, -5, sched);
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
        public void ToPropertyShouldFireInThePresenceOfWhenAny()
        {
            var sub1 = new Subject<Tuple<int, string>>();
            var sub2 = new Subject<int>();
            var sut = new OAPHViewModel(sub1, sub2);

            sub2.OnNext(1);
            sub1.OnNext(new Tuple<int, string>(1, "ExpectedString"));

            Assert.Equal(1, sut.Property2);
            Assert.Equal("ExpectedString", sut.Property1);
        }
    }
}