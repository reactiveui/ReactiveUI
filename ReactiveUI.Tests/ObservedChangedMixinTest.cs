using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Linq;
using System.Reactive.Subjects;
using Xunit;
using ReactiveUI.Testing;

#if MONO
using Mono.Reactive.Testing;
#else
using Microsoft.Reactive.Testing;
#endif

namespace ReactiveUI.Tests
{
    public class ObservedChangedMixinTest
    {
        [Fact]
        public void GetValueShouldActuallyReturnTheValue()
        {
            var input = new[] {"Foo", "Bar", "Baz"};
            var output = new List<string>();
            var output2 = new List<string>();

            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();

                // Two cases: Changed is guaranteed to *not* set ObservedChange.Value
                fixture.Changed.Subscribe(x => {
                    output.Add((string) x.GetValue());
                });

                // ...whereas ObservableForProperty *is* guaranteed to.
                fixture.ObservableForProperty(x => x.IsOnlyOneWord).Subscribe(x => {
                    output2.Add(x.GetValue());
                });

                foreach (var v in input) { fixture.IsOnlyOneWord = v; }

                sched.AdvanceToMs(1000);

                input.AssertAreEqual(output);
                input.AssertAreEqual(output2);
            });
        }

        [Fact]
        public void GetValueShouldReturnTheValueFromAPath()
        {
            var input = new HostTestFixture() {
                Child = new TestFixture() {IsNotNullString = "Foo"},
            };

            var fixture = new ObservedChange<HostTestFixture, string>() {
                Sender = input,
                PropertyName = "Child.IsNotNullString",
                Value = null,
            };

            Assert.Equal("Foo", fixture.GetValue());
        }

        [Fact]
        public void SetValuePathSmokeTest()
        {
            var output = new HostTestFixture() {
                Child = new TestFixture() {IsNotNullString = "Foo"},
            };

            var fixture = new ObservedChange<TestFixture, string>() {
                Sender = new TestFixture() { IsOnlyOneWord = "Bar" },
                PropertyName = "IsOnlyOneWord",
                Value = null,
            };

            fixture.SetValueToProperty(output, x => x.Child.IsNotNullString);
            Assert.Equal("Bar", output.Child.IsNotNullString);
        }

        [Fact]
        public void ValueTest() 
        {
            var input = new[] {"Foo", "Bar", "Baz"};
            IEnumerable<string> output = null;
            IEnumerable<string> output2 = null;

            (new TestScheduler()).With(sched => {
                var fixture = new TestFixture();

                // Same deal as above
                output = fixture.Changed.Value<object, object, string>().CreateCollection();
                output2 = fixture.ObservableForProperty(x => x.IsOnlyOneWord).Value().CreateCollection();

                foreach (var v in input) { fixture.IsOnlyOneWord = v; }

                sched.AdvanceToMs(1000);

                input.AssertAreEqual(output);
                input.AssertAreEqual(output2);
            });
        }

        [Fact]
        public void BindToSmokeTest()
        {
            (new TestScheduler()).With(sched => {
                var input = new ScheduledSubject<string>(sched);
                var fixture = new HostTestFixture() {Child = new TestFixture()};

                input.BindTo(fixture, x => x.Child.IsNotNullString);

                Assert.Null(fixture.Child.IsNotNullString);

                input.OnNext("Foo");
                sched.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);

                input.OnNext("Bar");
                sched.Start();
                Assert.Equal("Bar", fixture.Child.IsNotNullString);
            });
        }

        [Fact]
        public void DisposingDisconnectsTheBindTo()
        {
            (new TestScheduler()).With(sched => {
                var input = new ScheduledSubject<string>(sched);
                var fixture = new HostTestFixture() {Child = new TestFixture()};

                var subscription = input.BindTo(fixture, x => x.Child.IsNotNullString);

                Assert.Null(fixture.Child.IsNotNullString);

                input.OnNext("Foo");
                sched.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);

                subscription.Dispose();

                input.OnNext("Bar");
                sched.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);
            });
        }

        [Fact]
        public void BindToIsNotFooledByIntermediateObjectSwitching()
        {
            (new TestScheduler()).With(sched => {
                var input = new ScheduledSubject<string>(sched);
                var fixture = new HostTestFixture() {Child = new TestFixture()};

                var subscription = input.BindTo(fixture, x => x.Child.IsNotNullString);

                Assert.Null(fixture.Child.IsNotNullString);

                input.OnNext("Foo");
                sched.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);

                fixture.Child = new TestFixture();
                sched.Start();
                Assert.Null(fixture.Child.IsNotNullString);

                input.OnNext("Bar");
                sched.Start();
                Assert.Equal("Bar", fixture.Child.IsNotNullString);
            });
        }
    }
}
