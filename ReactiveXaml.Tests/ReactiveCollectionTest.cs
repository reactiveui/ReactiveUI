using Antireptilia;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using ReactiveXaml;
using System.IO;
using System.Text;
using ReactiveXaml.Tests;

namespace Antireptilia.Tests
{
    [TestClass()]
    public class ReactiveCollectionTest : IEnableLogger
    {
        [TestMethod()]
        [DeploymentItem("Antireptilia.exe")]
        public void CollectionCountChangedTest()
        {
            var fixture = new ReactiveCollection<int>();
            var output = new List<int>();
            fixture.CollectionCountChanged.Subscribe(output.Add);

            fixture.Add(10);
            fixture.Add(20);
            fixture.Add(30);
            fixture.RemoveAt(1);
            fixture.Clear();

            var results = new[]{1,2,3,2,0};
            Assert.AreEqual(results.Length, output.Count);
            results.Zip(output, (expected, actual) => new {expected,actual})
                   .Run(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod()]
        [DeploymentItem("Antireptilia.exe")]
        public void ItemsAddedAndRemovedTest()
        {
            var fixture = new ReactiveCollection<int>();
            var added = new List<int>();
            var removed = new List<int>();
            fixture.ItemsAdded.Subscribe(added.Add);
            fixture.ItemsRemoved.Subscribe(removed.Add);

            fixture.Add(10);
            fixture.Add(20);
            fixture.Add(30);
            fixture.RemoveAt(1);
            fixture.Clear();

            var added_results = new[]{10,20,30};
            Assert.AreEqual(added_results.Length, added.Count);
            added_results.Zip(added, (expected, actual) => new {expected,actual})
                         .Run(x => Assert.AreEqual(x.expected, x.actual));

            var removed_results = new[]{20};
            Assert.AreEqual(removed_results.Length, removed.Count);
            removed_results.Zip(removed, (expected, actual) => new {expected,actual})
                           .Run(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod()]
        [DeploymentItem("Antireptilia.exe")]
        public void ReactiveCollectionIsRoundTrippable()
        {
            var output = new[] {"Foo", "Bar", "Baz", "Bamf"};
            var fixture = new ReactiveCollection<string>(output);

            string json = JSONHelper.Serialize(fixture);
            var results = JSONHelper.Deserialize<ReactiveCollection<string>>(json);
            this.Log().Debug(json);

            output.Zip(results, (expected, actual) => new { expected, actual })
                  .Run(x => Assert.AreEqual(x.expected, x.actual));

            bool should_die = true;
            results.ItemsAdded.Subscribe(_ => should_die = false);
            results.Add("Foobar");
            Assert.IsFalse(should_die);
        }

        [TestMethod()]
        [DeploymentItem("Antireptilia.exe")]
        public void ChangeTrackingShouldFireNotifications()
        {
            var fixture = new ReactiveCollection<TestFixture>() { ChangeTrackingEnabled = true };
            var output = new List<Tuple<TestFixture, string>>();
            var item1 = new TestFixture() { IsOnlyOneWord = "Foo" };
            var item2 = new TestFixture() { IsOnlyOneWord = "Bar" };

            fixture.ItemPropertyChanged.Subscribe(x => {
                output.Add(new Tuple<TestFixture,string>((TestFixture)x.Sender, x.PropertyName));
            });

            fixture.Add(item1);
            fixture.Add(item2);

            item1.IsOnlyOneWord = "Baz";
            Assert.AreEqual(1, output.Count);
            item2.IsNotNullString = "FooBar";
            Assert.AreEqual(2, output.Count);

            fixture.Remove(item2);
            item2.IsNotNullString = "FooBarBaz";
            Assert.AreEqual(2, output.Count);

            fixture.ChangeTrackingEnabled = false;
            item1.IsNotNullString = "Bamf";
            Assert.AreEqual(2, output.Count);

            new[]{item1, item2}.Zip(output.Select(x => x.Item1), (expected, actual) => new { expected, actual })
                .Run(x => Assert.AreEqual(x.expected, x.actual));
            new[]{"IsOnlyOneWord", "IsNotNullString"}.Zip(output.Select(x => x.Item2), (expected, actual) => new { expected, actual })
                .Run(x => Assert.AreEqual(x.expected, x.actual));
        }

        [TestMethod()]
        public void DerivedCollectionsShouldFollowBaseCollection()
        {
            var input = new[] {"Foo", "Bar", "Baz", "Bamf"};
            var fixture = new ReactiveCollection<TestFixture>(
                input.Select(x => new TestFixture() { IsOnlyOneWord = x }));

            var output = fixture.CreateDerivedCollection(new Func<TestFixture, string>(x => x.IsOnlyOneWord));

            Assert.AreEqual(4, output.Count);
            input.Zip(output, (expected, actual) => new { expected, actual })
                 .Run(x => Assert.AreEqual(x.expected, x.actual));

            fixture.Add(new TestFixture() { IsOnlyOneWord = "Hello" });
            Assert.AreEqual(5, output.Count);
            Assert.AreEqual(output[4], "Hello");

            fixture.RemoveAt(4);
            Assert.AreEqual(4, output.Count);

            fixture[1] = new TestFixture() { IsOnlyOneWord = "Goodbye" };
            Assert.AreEqual(4, output.Count);
            Assert.AreEqual(output[1], "Goodbye");

            fixture.Clear();
            Assert.AreEqual(0, output.Count);
        }
    }

    public class JSONHelper
    {
        public static string Serialize<T>(T obj)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            return retVal;
        }

        public static T Deserialize<T>(string json)
        {
            var obj = Activator.CreateInstance<T>();
            var ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }
}