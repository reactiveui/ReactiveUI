using System;
using System.Concurrency;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveXaml.Tests;

namespace ReactiveXaml.Serialization.Tests
{
    [TestClass]
    public abstract class StorageEngineInterfaceTest : IEnableLogger
    {
        protected abstract IStorageEngine createFixture(int key = -1);

        protected IExtendedStorageEngine createExtendedFixture(int key = -1)
        {
            var ret = createFixture(key);
            if (ret is IExtendedStorageEngine) {
                return (IExtendedStorageEngine)ret;
            }

            return new NaiveExtendedEngine(ret);
        }

        [TestMethod]
        public void StorageEngineLoadSaveSmokeTest() {
            var input = new SubobjectTestObject() { SomeProperty = "Foo" };
            var fixture = createFixture();
            SubobjectTestObject result;

            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.FlushChanges();

                Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.ContentHash));

                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SomeProperty, result.SomeProperty);
            }
        }

        [TestMethod]
        public void StorageEngineShouldActuallySerializeStuff() {
            var input = new SubobjectTestObject() { SomeProperty = "Foo" };
            var fixture = createFixture(0xbeef);
            SubobjectTestObject result;

            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.FlushChanges();

                Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.ContentHash));

                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SomeProperty, result.SomeProperty);
            }

            fixture = createFixture(0xbeef);
            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SomeProperty, result.SomeProperty);
            }
        }

        [TestMethod]
        public void StorageEngineShouldReuseObjectsWithTheSameHash() {
            var input = new RootSerializationTestObject() { SubObject = new SubobjectTestObject() { SomeProperty = "Foo" } };
            var input2 = new SubobjectTestObject() { SomeProperty = "Foo" };
            var fixture = createFixture();

            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.Save(input2);

                var result = fixture.Load<RootSerializationTestObject>(input.ContentHash);
                Assert.AreEqual(input.ContentHash, result.ContentHash);
                Assert.AreEqual(input.SubObject.ContentHash, result.SubObject.ContentHash);
                Assert.AreEqual(2, fixture.GetObjectCount());
            }
        }

        [TestMethod]
        public void EmptyStorageEngineShouldReturnZeroResults() {
            using (var fixture = createFixture()) {
                Assert.AreEqual(0, fixture.GetAllObjectHashes().Length);
                Assert.AreEqual(0, fixture.GetObjectCount());
                Assert.AreEqual(0, fixture.GetOrderedRevisionList(typeof(object)).Length);
            }
        }

        [TestMethod]
        public void CreateSyncPointSmokeTest() 
        {
            var scheduler = new TestScheduler();
            var fixture = createFixture();

            scheduler.With(sched => {
                using (fixture)
                using (fixture.AsPrimaryEngine()) {
                    var input = new RootSerializationTestObject() { SubObject = new SubobjectTestObject() { SomeProperty = "Foo" } };

                    var syncPoint = fixture.CreateSyncPoint(input);

                    sched.RunTo(sched.FromTimeSpan(TimeSpan.FromDays(1.0)));

                    Assert.AreEqual(3, fixture.GetObjectCount());
                    Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.ContentHash));
                    Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.SubObject.ContentHash));
                    Assert.IsTrue(fixture.GetAllObjectHashes().Contains(syncPoint.ContentHash));

                    Assert.AreEqual(1, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject)).Length);
                    Assert.AreEqual(0, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject), "WeirdQualifier").Length);

                    input.SomeInteger = 10;

                    syncPoint = fixture.CreateSyncPoint(input);

                    Assert.AreEqual(5, fixture.GetObjectCount());
                    Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.ContentHash));
                    Assert.IsTrue(fixture.GetAllObjectHashes().Contains(input.SubObject.ContentHash));
                    Assert.IsTrue(fixture.GetAllObjectHashes().Contains(syncPoint.ContentHash));

                    Assert.AreEqual(2, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject)).Length);
                    Assert.AreEqual(0, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject), "WeirdQualifier").Length);
                }

                return 0;
            });
        }

        [TestMethod]
        public void ExtendedStorageEngineGetRootObject()
        {
            var scheduler = new TestScheduler();
            var fixture = createExtendedFixture();

            scheduler.With(sched => {
                using (fixture)
                using (fixture.AsPrimaryEngine()) {
                    var input = new RootSerializationTestObject() { SubObject = new SubobjectTestObject() { SomeProperty = "Foo" } };
                    var origHash = input.ContentHash;
                    var syncPoint = fixture.CreateSyncPoint(input, null, DateTimeOffset.Now);

                    // N.B. This doesn't appear to actually affect IScheduler.Now :-/
                    sched.RunTo(sched.FromTimeSpan(TimeSpan.FromDays(1.0)));

                    Assert.AreEqual(input.ContentHash, fixture.GetLatestRootObject<RootSerializationTestObject>().ContentHash);
                    Assert.AreEqual(null, fixture.GetLatestRootObject<RootSerializationTestObject>("SomeWeirdQualifier"));
                    Assert.AreEqual(null, fixture.GetLatestRootObject<RootSerializationTestObject>(null, DateTimeOffset.Now - TimeSpan.FromHours(1.0)));

                    input.SomeInteger = 10;
                    syncPoint = fixture.CreateSyncPoint(input, null, DateTimeOffset.Now + TimeSpan.FromDays(1.0));
                    Assert.AreEqual(origHash, fixture.GetLatestRootObject<RootSerializationTestObject>(null, DateTimeOffset.Now + TimeSpan.FromSeconds(1.0)).ContentHash);
                }

                return 0;
            });
        }

        [TestMethod]
        public void LoadMethodWithInvalidKeyShouldReturnNoResults()
        {
            var fixture = createFixture();
            var output = fixture.Load(Guid.NewGuid());
            Assert.AreEqual(null, output);
        }

        [TestMethod]
        public void GetLatestRootObjectWithEmptyEngineShouldReturnNoResults()
        {
            var fixture = createExtendedFixture();
            var output = fixture.GetLatestRootObject<RootSerializationTestObject>();
            Assert.AreEqual(null, output);
        }

        [TestMethod]
        public void GetOrderedRevisionRangeWithEmptyEngineShouldReturnNoResults()
        {
            var fixture = createExtendedFixture();
            var output = fixture.GetOrderedRevisionList(typeof(Type));
            Assert.IsNotNull(output);
            Assert.AreEqual(0, output.Length);
        }

        [TestMethod]
        public void TryingToSaveNullGuidShouldBePunished()
        {
            var input = new ZeroContentHashModel();
            var fixture = createExtendedFixture();

            bool shouldFail = true;
            try {
                fixture.Save(input);
            } catch(Exception ex) {
                this.Log().Debug(ex);
                shouldFail = false;
            }
            Assert.IsFalse(shouldFail);

            shouldFail = true;
            try {
                fixture.CreateSyncPoint(input);
            } catch(Exception ex) {
                this.Log().Debug(ex);
                shouldFail = false;
            }
            Assert.IsFalse(shouldFail);
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

#if !SILVERLIGHT
    [TestClass]
    public class EsentStorageEngineTest : StorageEngineInterfaceTest
    {
        static int _nextFreeSubdirectory = 1;
        protected override IStorageEngine createFixture(int key = -1)
        {
            var di = new DirectoryInfo(".");
            int subdir = (key == -1 ? Interlocked.Increment(ref _nextFreeSubdirectory) : key);
            this.Log().InfoFormat("Opening db with name '{0}'", subdir.ToString());
            var path = di.CreateSubdirectory(subdir.ToString());
            return new Esent.EsentStorageEngine(path.FullName);
        }
    }
#endif

    public class ZeroContentHashModel : ModelBase
    {
        public override Guid CalculateHash()
        {
            return Guid.Empty;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
