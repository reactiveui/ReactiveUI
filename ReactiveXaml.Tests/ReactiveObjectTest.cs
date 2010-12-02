using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ReactiveXaml.Tests
{
    public class TestFixture : ReactiveObject
    {
#if SILVERLIGHT
        [IgnoreDataMember]
        public string _IsNotNullString;
        public string IsNotNullString {
            get { return _IsNotNullString; }
            set { RaiseAndSetIfChanged(_IsNotNullString, value, x => _IsNotNullString = x, "IsNotNullString"); }
        }

        [IgnoreDataMember]
        public string _IsOnlyOneWord;
        public string IsOnlyOneWord {
            get { return _IsOnlyOneWord; }
            set { RaiseAndSetIfChanged(_IsOnlyOneWord, value, x => _IsOnlyOneWord = x, "IsOnlyOneWord"); }
        }

        [IgnoreDataMember]
        public string _UsesExprRaiseSet;
        public string UsesExprRaiseSet {
            get { return _UsesExprRaiseSet; }
            set { this.RaiseAndSetIfChanged(x => x.UsesExprRaiseSet, value); }
        }
#else
        string _IsNotNullString;
        public string IsNotNullString {
            get { return _IsNotNullString; }
            set { RaiseAndSetIfChanged(_IsNotNullString, value, x => _IsNotNullString = x, "IsNotNullString"); }
        }

        string _IsOnlyOneWord;
        public string IsOnlyOneWord {
            get { return _IsOnlyOneWord; }
            set { RaiseAndSetIfChanged(_IsOnlyOneWord, value, x => _IsOnlyOneWord = x, "IsOnlyOneWord"); }
        }

        string _UsesExprRaiseSet;
        public string UsesExprRaiseSet {
            get { return _UsesExprRaiseSet; }
            set { this.RaiseAndSetIfChanged(x => x.UsesExprRaiseSet, value); }
        }
#endif

        public ReactiveCollection<int> TestCollection { get; protected set; }

        public TestFixture()
        {
            TestCollection = new ReactiveCollection<int>() { ChangeTrackingEnabled = true };
            WatchCollection(TestCollection, "TestCollection");
        }
    }

    [TestClass()]
    public class ReactiveObjectTest : IEnableLogger
    {
        [TestMethod()]        
        public void ReactiveObjectSmokeTest()
        {
#if IOS
			Assert.Fail("This crashes Mono in a quite spectacular way");
#endif
            var output_changing = new List<string>();
            var output = new List<string>();
            var fixture = new TestFixture();

            fixture.BeforeChange.Subscribe(x => output_changing.Add(x.PropertyName));
            fixture.Subscribe(x => output.Add(x.PropertyName));

            fixture.IsNotNullString = "Foo Bar Baz";
            fixture.IsOnlyOneWord = "Foo";
            fixture.IsOnlyOneWord = "Bar";
            fixture.IsNotNullString = null;     // Sorry.

            var results = new[] { "IsNotNullString", "IsOnlyOneWord", "IsOnlyOneWord", "IsNotNullString" };
            results.AssertAreEqual(output);

            output.AssertAreEqual(output_changing);
        }

        [TestMethod()]
        public void SubscriptionExceptionsShouldntPermakillReactiveObject()
        {
            return;
            Assert.Inconclusive("This test doesn't work yet");

            var fixture = new TestFixture();
            int i = 0;
            fixture.Subscribe(x => {
                if (++i == 2)
                    throw new Exception("Deaded!");
            });

            fixture.IsNotNullString = "Foo";
            fixture.IsNotNullString = "Bar";
            fixture.IsNotNullString = "Baz";
            fixture.IsNotNullString = "Bamf";

            var output = new List<string>();
            fixture.Subscribe(x => output.Add(x.PropertyName));
            fixture.IsOnlyOneWord = "Bar";

            Assert.AreEqual("IsOnlyOneWord", output[0]);
            Assert.AreEqual(1, output.Count);
        }

        [TestMethod()]
        public void ReactiveObjectShouldWatchCollections()
        {
#if IOS
			Assert.Fail("This crashes Mono in a quite spectacular way");
#endif			
            var output = new List<string>();
            var fixture = new TestFixture();

            fixture.Subscribe(x => output.Add(x.PropertyName));

            fixture.TestCollection.Add(5);
            fixture.TestCollection.Add(10);
            fixture.TestCollection.RemoveAt(1);

            Assert.AreEqual(3, output.Count);
        }

        [TestMethod()]
        public void ReactiveObjectShouldntSerializeAnythingExtra()
        {
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
            string json = JSONHelper.Serialize(fixture);
            this.Log().Debug(json);

            // Should look something like:
            // {"IsNotNullString":"Foo","IsOnlyOneWord":"Baz", "UserExprRaiseSet":null}
            Assert.IsTrue(json.Count(x => x == ',') == 2);
            Assert.IsTrue(json.Count(x => x == ':') == 3);
            Assert.IsTrue(json.Count(x => x == '"') == 10);
        }

        [TestMethod()]
        public void RaiseAndSetUsingExpression()
        {
#if IOS
			Assert.Fail("This crashes Mono in a quite spectacular way");
#endif
			
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
            var output = new List<string>();
            fixture.Subscribe(x => output.Add(x.PropertyName));

            fixture.UsesExprRaiseSet = "Foo";
            fixture.UsesExprRaiseSet = "Foo";   // This one shouldn't raise a change notification

            Assert.AreEqual("Foo", fixture.UsesExprRaiseSet);
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("UsesExprRaiseSet", output[0]);
        }


        [TestMethod()]
        public void ObservableForPropertyUsingExpression()
        {
            var fixture = new TestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
            var output = new List<ObservedChange<TestFixture, string>>();
            fixture.ObservableForProperty(x => x.IsNotNullString).Subscribe(output.Add);

            fixture.IsNotNullString = "Bar";
            fixture.IsNotNullString = "Baz";
            fixture.IsNotNullString = "Baz";

            fixture.IsOnlyOneWord = "Bamf";

            Assert.AreEqual(2, output.Count);

            Assert.AreEqual(fixture, output[0].Sender);
            Assert.AreEqual("IsNotNullString", output[0].PropertyName);
            Assert.AreEqual("Bar", output[0].Value);

            Assert.AreEqual(fixture, output[1].Sender);
            Assert.AreEqual("IsNotNullString", output[1].PropertyName);
            Assert.AreEqual("Baz", output[1].Value);
        }

        [TestMethod()]
        public void ChangingShouldAlwaysArriveBeforeChanged()
        {
            string before_set = "Foo";
            string after_set = "Bar"; 
			
#if IOS
			Assert.Fail("This crashes Mono in a quite spectacular way");
#endif
			
            var fixture = new TestFixture() { IsOnlyOneWord = before_set };

            bool before_fired = false;
            fixture.BeforeChange.Subscribe(x => {
                // XXX: The content of these asserts don't actually get 
                // propagated back, it only prevents before_fired from
                // being set - we have to enable 1st-chance exceptions
                // to see the real error
                Assert.AreEqual("IsOnlyOneWord", x.PropertyName);
                Assert.AreEqual(fixture.IsOnlyOneWord, before_set);
                before_fired = true;
            });

            bool after_fired = false;
            fixture.Subscribe(x => {
                Assert.AreEqual("IsOnlyOneWord", x.PropertyName);
                Assert.AreEqual(fixture.IsOnlyOneWord, after_set);
                after_fired = true;
            });

            fixture.IsOnlyOneWord = after_set;

            Assert.IsTrue(before_fired);
            Assert.IsTrue(after_fired);
        }
    }
}