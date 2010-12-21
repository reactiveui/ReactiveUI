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
        protected abstract IStorageEngine createFixture();

        [TestMethod]
        public void StorageEngineLoadSaveSmokeTest()
        {
            var input = new SubobjectTestObject() {SomeProperty = "Foo"};
            var fixture = createFixture();
            SubobjectTestObject result;

            using(fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.FlushChanges();

                Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.ContentHash));

                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SomeProperty, result.SomeProperty);
            }
        }
    }

    [TestClass]
    public class DictionaryStorageEngineTest : StorageEngineInterfaceTest
    {
        protected override IStorageEngine createFixture()
        {
            return new DictionaryStorageEngine();
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
