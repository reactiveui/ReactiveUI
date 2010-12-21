using System.IO;
using System.Threading;
using Microsoft.Pex.Framework;
using ReactiveXaml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ReactiveXaml.Serialization.Tests
{
    [TestClass]
    public abstract class StorageEngineInterfaceTest
    {
        protected abstract IStorageEngine createFixture(int key = -1);

        [TestMethod]
        public void StorageEngineLoadSaveSmokeTest()
        {
            var input = new SubobjectTestObject() {SomeProperty = "Foo"};
            var fixture = createFixture();
            SubobjectTestObject result;

            using(fixture)
            using(fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.FlushChanges();

                Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.ContentHash));

                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SomeProperty, result.SomeProperty);
            }
        }

        [TestMethod]
        public void StorageEngineShouldActuallySerializeStuff()
        {
            var input = new SubobjectTestObject() {SomeProperty = "Foo"};
            var fixture = createFixture(0xbeef);
            SubobjectTestObject result;

            using(fixture)
            using(fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.FlushChanges();

                Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.ContentHash));

                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SomeProperty, result.SomeProperty);
            }

            fixture = createFixture(0xbeef);
            using(fixture)
            using(fixture.AsPrimaryEngine()) {
                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SomeProperty, result.SomeProperty);
            }
        }

        [TestMethod]
        public void StorageEngineShouldReuseObjectsWithTheSameHash()
        {
            var input = new RootSerializationTestObject() {SubObject = new SubobjectTestObject() {SomeProperty = "Foo"}};
            var input2 = new SubobjectTestObject() {SomeProperty = "Foo"};
            var fixture = createFixture();

            using(fixture)
            using(fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.Save(input2);

                var result = fixture.Load<RootSerializationTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SubObject.ContentHash, result.SubObject.ContentHash);
                Assert.AreEqual(2, fixture.GetObjectCount());
            }
        }
    }

    [TestClass]
    public class DictionaryStorageEngineTest : StorageEngineInterfaceTest
    {
        protected override IStorageEngine createFixture(int key = -1)
        {
            if (key == -1) {
                return new DictionaryStorageEngine();
            }

            var di = new DirectoryInfo(".");
            var path = di.CreateSubdirectory(key.ToString());
            return new DictionaryStorageEngine(Path.Combine(path.FullName, "dict.json"));
        }
    }

    [TestClass]
    public class EsentStorageEngineTest : StorageEngineInterfaceTest
    {
        int _nextFreeSubdirectory = 1;
        protected override IStorageEngine createFixture(int key = -1)
        {
            var di = new DirectoryInfo(".");
            int subdir = (key == -1 ? Interlocked.Increment(ref _nextFreeSubdirectory) : key);
            var path = di.CreateSubdirectory(subdir.ToString());
            return new Esent.EsentStorageEngine(path.FullName);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
