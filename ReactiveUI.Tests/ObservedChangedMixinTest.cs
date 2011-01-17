using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using Xunit;
using ReactiveUI.Testing;

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
            var sched = new TestScheduler();
            sched.With(_ => {
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
            });

            sched.RunToMilliseconds(1000);

            input.AssertAreEqual(output);
            input.AssertAreEqual(output2);
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
        public void ValueTest() 
        {
            var input = new[] {"Foo", "Bar", "Baz"};
            var sched = new TestScheduler();
            IEnumerable<string> output = null;
            IEnumerable<string> output2 = null;

            sched.With(_ => {
                var fixture = new TestFixture();

                // Same deal as above
                output = fixture.Changed.Value<object, object, string>().CreateCollection();
                output2 = fixture.ObservableForProperty(x => x.IsOnlyOneWord).Value().CreateCollection();

                foreach (var v in input) { fixture.IsOnlyOneWord = v; }
            });

            sched.RunToMilliseconds(1000);

            input.AssertAreEqual(output);
            input.AssertAreEqual(output2);
        }
    }
}
