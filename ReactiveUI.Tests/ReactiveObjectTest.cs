using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Xunit;

namespace ReactiveUI.Tests
{
    [DataContract]
    public class TestFixture : ReactiveObject
    {
        [DataMember]
        public string _IsNotNullString;
        [IgnoreDataMember]
        public string IsNotNullString {
            get { return _IsNotNullString; }
            set { this.RaiseAndSetIfChanged(ref _IsNotNullString, value); }
        }

        [DataMember]
        public string _IsOnlyOneWord;
        [IgnoreDataMember]
        public string IsOnlyOneWord {
            get { return _IsOnlyOneWord; }
            set { this.RaiseAndSetIfChanged(ref _IsOnlyOneWord, value); }
        }

        [DataMember]
        public List<string> _StackOverflowTrigger;
        [IgnoreDataMember]
        public List<string> StackOverflowTrigger {
            get { return _StackOverflowTrigger; }
            set { this.RaiseAndSetIfChanged(ref _StackOverflowTrigger, value.ToList()); }
        }

        [DataMember]
        public string _UsesExprRaiseSet;
        [IgnoreDataMember]
        public string UsesExprRaiseSet {
            get { return _UsesExprRaiseSet; }
            set { this.RaiseAndSetIfChanged(ref _UsesExprRaiseSet, value); }
        }

        [DataMember]
        public string _PocoProperty;
        [IgnoreDataMember]
        public string PocoProperty {
            get { return _PocoProperty; }
            set { _PocoProperty = value; }
        }

        [DataMember]
        public ReactiveCollection<int> TestCollection { get; protected set; }

        public TestFixture()
        {
            TestCollection = new ReactiveCollection<int>() {ChangeTrackingEnabled = true};
        }
    }

    public class ReactiveObjectTest
    {
        [Fact]        
        public void ReactiveObjectSmokeTest()
        {
#if IOS
            Assert.Fail("This crashes Mono in a quite spectacular way");
#endif
            var output_changing = new List<string>();
            var output = new List<string>();
            var fixture = new TestFixture();

            fixture.Changing.Subscribe(x => output_changing.Add(x.PropertyName));
            fixture.Changed.Subscribe(x => output.Add(x.PropertyName));

            fixture.IsNotNullString = "Foo Bar Baz";
            fixture.IsOnlyOneWord = "Foo";
            fixture.IsOnlyOneWord = "Bar";
            fixture.IsNotNullString = null;     // Sorry.
            fixture.IsNotNullString = null;

            var results = new[] { "IsNotNullString", "IsOnlyOneWord", "IsOnlyOneWord", "IsNotNullString" };

            Assert.Equal(results.Length, output.Count);

            output.AssertAreEqual(output_changing);
            results.AssertAreEqual(output);
        }

        [Fact]
        public void ReactiveObjectShouldntSerializeAnythingExtra()
        {
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
            string json = JSONHelper.Serialize(fixture);

            // Should look something like:
            // "{"TestCollection":[],"_IsNotNullString":"Foo","_IsOnlyOneWord":"Baz","_PocoProperty":null,"_StackOverflowTrigger":null,"_UsesExprRaiseSet":null}"
            Assert.True(json.Count(x => x == ',') == 5);
            Assert.True(json.Count(x => x == ':') == 6);
            Assert.True(json.Count(x => x == '"') == 16);
        }

        [Fact]
        public void RaiseAndSetUsingExpression()
        {
#if IOS
            Assert.Fail("This crashes Mono in a quite spectacular way");
#endif
            
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
            var output = new List<string>();
            fixture.Changed.Subscribe(x => output.Add(x.PropertyName));

            fixture.UsesExprRaiseSet = "Foo";
            fixture.UsesExprRaiseSet = "Foo";   // This one shouldn't raise a change notification

            Assert.Equal("Foo", fixture.UsesExprRaiseSet);
            Assert.Equal(1, output.Count);
            Assert.Equal("UsesExprRaiseSet", output[0]);
        }


        [Fact]
        public void ObservableForPropertyUsingExpression()
        {
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
            var output = new List<IObservedChange<TestFixture, string>>();
            fixture.ObservableForProperty(x => x.IsNotNullString).Subscribe(x => {
                output.Add(x);
            });

            fixture.IsNotNullString = "Bar";
            fixture.IsNotNullString = "Baz";
            fixture.IsNotNullString = "Baz";

            fixture.IsOnlyOneWord = "Bamf";

            Assert.Equal(2, output.Count);

            Assert.Equal(fixture, output[0].Sender);
            Assert.Equal("IsNotNullString", output[0].PropertyName);
            Assert.Equal("Bar", output[0].Value);

            Assert.Equal(fixture, output[1].Sender);
            Assert.Equal("IsNotNullString", output[1].PropertyName);
            Assert.Equal("Baz", output[1].Value);
        }

        [Fact]
        public void ChangingShouldAlwaysArriveBeforeChanged()
        {
            string before_set = "Foo";
            string after_set = "Bar"; 
            
#if IOS
        Assert.Fail("This crashes Mono in a quite spectacular way");
#endif
            
            var fixture = new TestFixture() { IsOnlyOneWord = before_set };

            bool before_fired = false;
            fixture.Changing.Subscribe(x => {
                // XXX: The content of these asserts don't actually get 
                // propagated back, it only prevents before_fired from
                // being set - we have to enable 1st-chance exceptions
                // to see the real error
                Assert.Equal("IsOnlyOneWord", x.PropertyName);
                Assert.Equal(fixture.IsOnlyOneWord, before_set);
                before_fired = true;
            });

            bool after_fired = false;
            fixture.Changed.Subscribe(x => {
                Assert.Equal("IsOnlyOneWord", x.PropertyName);
                Assert.Equal(fixture.IsOnlyOneWord, after_set);
                after_fired = true;
            });

            fixture.IsOnlyOneWord = after_set;

            Assert.True(before_fired);
            Assert.True(after_fired);
        }
    }
}
