using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Xunit;

namespace ReactiveUI.Tests
{
    [DataContract]
    public class TestFixture : ReactiveObject
    {
        [IgnoreDataMember]
        string _IsNotNullString;
        [DataMember]
        public string IsNotNullString {
            get { return _IsNotNullString; }
            set { this.RaiseAndSetIfChanged(ref _IsNotNullString, value); }
        }

        [IgnoreDataMember]
        string _IsOnlyOneWord;
        [DataMember]
        public string IsOnlyOneWord {
            get { return _IsOnlyOneWord; }
            set { this.RaiseAndSetIfChanged(ref _IsOnlyOneWord, value); }
        }

        [IgnoreDataMember]
        List<string> _StackOverflowTrigger;
        [DataMember]
        public List<string> StackOverflowTrigger {
            get { return _StackOverflowTrigger; }
            set { this.RaiseAndSetIfChanged(ref _StackOverflowTrigger, value.ToList()); }
        }

        [IgnoreDataMember]
        string _UsesExprRaiseSet;
        [DataMember]
        public string UsesExprRaiseSet {
            get { return _UsesExprRaiseSet; }
            set { this.RaiseAndSetIfChanged(ref _UsesExprRaiseSet, value); }
        }

        [IgnoreDataMember]
        string _PocoProperty;
        [DataMember]
        public string PocoProperty {
            get { return _PocoProperty; }
            set { _PocoProperty = value; }
        }

        [DataMember]
        public ReactiveList<int> TestCollection { get; protected set; }

        string _NotSerialized;
        public string NotSerialized {
            get { return _NotSerialized; }
            set { this.RaiseAndSetIfChanged(ref _NotSerialized, value); }
        }

        [IgnoreDataMember]
        int? _nullableInt;

        [DataMember]
        public int? NullableInt
        {
            get { return _nullableInt; }
            set { this.RaiseAndSetIfChanged(ref _nullableInt, value); }
        }

        public TestFixture()
        {
            TestCollection = new ReactiveList<int>() {ChangeTrackingEnabled = true};
        }
    }

    public class OaphTestFixture : TestFixture
    {
        [IgnoreDataMember]
        ObservableAsPropertyHelper<string> _FirstThreeLettersOfOneWord;
        [IgnoreDataMember]
        public string FirstThreeLettersOfOneWord {
            get { return _FirstThreeLettersOfOneWord.Value; }
        }

        public OaphTestFixture()
        {
            this.WhenAnyValue(x => x.IsOnlyOneWord)
                .Select(x => (x ?? "")).Select(x => x.Length >= 3 ? x.Substring(0, 3) : x)
                .ToProperty(this, x => x.FirstThreeLettersOfOneWord, out _FirstThreeLettersOfOneWord);
        }
    }

    public class ReactiveObjectTest
    {
        [Fact]        
        public void ReactiveObjectSmokeTest()
        {
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
            // {"IsNotNullString":"Foo","IsOnlyOneWord":"Baz","NullableInt":null,"PocoProperty":null,"StackOverflowTrigger":null,"TestCollection":[],"UsesExprRaiseSet":null}
            Assert.True(json.Count(x => x == ',') == 6);
            Assert.True(json.Count(x => x == ':') == 7);
            Assert.True(json.Count(x => x == '"') == 18);
        }

        [Fact]
        public void RaiseAndSetUsingExpression()
        {
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
            Assert.Equal("IsNotNullString", output[0].GetPropertyName());
            Assert.Equal("Bar", output[0].Value);

            Assert.Equal(fixture, output[1].Sender);
            Assert.Equal("IsNotNullString", output[1].GetPropertyName());
            Assert.Equal("Baz", output[1].Value);
        }

        [Fact]
        public void ChangingShouldAlwaysArriveBeforeChanged()
        {
            string before_set = "Foo";
            string after_set = "Bar"; 
            
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

        [Fact]
        public void ExceptionsThrownInSubscribersShouldMarshalToThrownExceptions()
        {
            var fixture = new TestFixture() { IsOnlyOneWord = "Foo" };

            fixture.Changed.Subscribe(x => { throw new Exception("Die!"); });
            var exceptionList = fixture.ThrownExceptions.CreateCollection();

            fixture.IsOnlyOneWord = "Bar";
            Assert.Equal(1, exceptionList.Count);
        }

        [Fact]
        public void DeferringNotificationsDontShowUpUntilUndeferred()
        {
            var fixture = new TestFixture();
            var output = fixture.Changed.CreateCollection();

            Assert.Equal(0, output.Count);
            fixture.NullableInt = 4;
            Assert.Equal(1, output.Count);

            var stopDelaying = fixture.DelayChangeNotifications();

            fixture.NullableInt = 5;
            Assert.Equal(1, output.Count);

            fixture.IsNotNullString = "Bar";
            Assert.Equal(1, output.Count);

            fixture.NullableInt = 6;
            Assert.Equal(1, output.Count);

            fixture.IsNotNullString = "Baz";
            Assert.Equal(1, output.Count);

            var stopDelayingMore = fixture.DelayChangeNotifications();

            fixture.IsNotNullString = "Bamf";
            Assert.Equal(1, output.Count);

            stopDelaying.Dispose();

            fixture.IsNotNullString = "Blargh";
            Assert.Equal(1, output.Count);

            // NB: Because we debounce queued up notifications, we should only
            // see a notification from the latest NullableInt and the latest
            // IsNotNullableString
            stopDelayingMore.Dispose();
            Assert.Equal(3, output.Count);
        }
    }
}
