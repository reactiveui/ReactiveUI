using ReactiveXaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Antireptilia.Tests;

namespace ReactiveXaml.Tests
{
    public class TestFixture : ReactiveValidatedObject
    {
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
            var output = new List<string>();
            var fixture = new TestFixture();

            fixture.Subscribe(x => output.Add(x.PropertyName));

            fixture.IsNotNullString = "Foo Bar Baz";
            fixture.IsOnlyOneWord = "Foo";
            fixture.IsOnlyOneWord = "Bar";
            fixture.IsNotNullString = null;     // Sorry.

            var results = new[] { "IsNotNullString", "IsOnlyOneWord", "IsOnlyOneWord", "IsNotNullString" };
            results.Zip(output, (expected, actual) => new { expected, actual })
                   .Run(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod()]
        public void SubscriptionExceptionsShouldntPermakillReactiveObject()
        {
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
            var fixture = new ValidatedTestFixture() { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
            string json = JSONHelper.Serialize(fixture);
            this.Log().Debug(json);

            // Should look something like:
            // {"IsNotNullString":"Foo","IsOnlyOneWord":"Baz"}
            Assert.IsTrue(json.Count(x => x == ',') == 1);
            Assert.IsTrue(json.Count(x => x == ':') == 2);
            Assert.IsTrue(json.Count(x => x == '"') == 8);
        }
    }
}