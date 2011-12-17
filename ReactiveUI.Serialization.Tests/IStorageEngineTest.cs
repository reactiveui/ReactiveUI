using System;
using System.Reactive.Concurrency;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Reactive.Testing;
using Xunit;
using ReactiveUI.Testing;
using ReactiveUI.Tests;

namespace ReactiveUI.Serialization.Tests
{
    public abstract class StorageEngineInterfaceTest
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

        [Fact]
        public void StorageEngineLoadSaveSmokeTest() {
            var input = new SubobjectTestObject() { SomeProperty = "Foo" };
            var fixture = createFixture();
            SubobjectTestObject result;

            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.FlushChanges();

                Assert.True(fixture.GetAllObjectHashes().Contains(input.ContentHash));

                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.Equal(input.ContentHash, result.ContentHash);
                Assert.Equal(input.SomeProperty, result.SomeProperty);
            }
        }

        [Fact]
        public void StorageEngineShouldActuallySerializeStuff() {
            var input = new SubobjectTestObject() { SomeProperty = "Foo" };
            var fixture = createFixture(0xbeef);
            SubobjectTestObject result;

            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.FlushChanges();

                Assert.True(fixture.GetAllObjectHashes().Contains(input.ContentHash));

                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.Equal(input.ContentHash, result.ContentHash);
                Assert.Equal(input.SomeProperty, result.SomeProperty);
            }

            fixture = createFixture(0xbeef);
            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                result = fixture.Load<SubobjectTestObject>(input.ContentHash);
                Assert.Equal(input.ContentHash, result.ContentHash);
                Assert.Equal(input.SomeProperty, result.SomeProperty);
            }
        }

        [Fact]
        public void StorageEngineShouldReuseObjectsWithTheSameHash() {
            var input = new RootSerializationTestObject() { SubObject = new SubobjectTestObject() { SomeProperty = "Foo" } };
            var input2 = new SubobjectTestObject() { SomeProperty = "Foo" };
            var fixture = createFixture();

            using (fixture)
            using (fixture.AsPrimaryEngine()) {
                fixture.Save(input);
                fixture.Save(input2);

                var result = fixture.Load<RootSerializationTestObject>(input.ContentHash);
                Assert.Equal(input.ContentHash, result.ContentHash);
                Assert.Equal(input.SubObject.ContentHash, result.SubObject.ContentHash);
                Assert.Equal(2, fixture.GetObjectCount());
            }
        }

        [Fact]
        public void EmptyStorageEngineShouldReturnZeroResults() {
            using (var fixture = createFixture()) {
                Assert.Equal(0, fixture.GetAllObjectHashes().Length);
                Assert.Equal(0, fixture.GetObjectCount());
                Assert.Equal(0, fixture.GetOrderedRevisionList(typeof(object)).Length);
            }
        }

        [Fact]
        public void CreateSyncPointSmokeTest() 
        {
            var scheduler = new TestScheduler();
            var fixture = createFixture();

            scheduler.With(sched => {
                using (fixture)
                using (fixture.AsPrimaryEngine()) {
                    var input = new RootSerializationTestObject() { SubObject = new SubobjectTestObject() { SomeProperty = "Foo" } };

                    var syncPoint = fixture.CreateSyncPoint(input);

                    sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromDays(1.0)));

                    Assert.Equal(3, fixture.GetObjectCount());
                    Assert.True(fixture.GetAllObjectHashes().Contains(input.ContentHash));
                    Assert.True(fixture.GetAllObjectHashes().Contains(input.SubObject.ContentHash));
                    Assert.True(fixture.GetAllObjectHashes().Contains(syncPoint.ContentHash));

                    Assert.Equal(1, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject)).Length);
                    Assert.Equal(0, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject), "WeirdQualifier").Length);

                    input.SomeInteger = 10;

                    syncPoint = fixture.CreateSyncPoint(input);

                    Assert.Equal(5, fixture.GetObjectCount());
                    Assert.True(fixture.GetAllObjectHashes().Contains(input.ContentHash));
                    Assert.True(fixture.GetAllObjectHashes().Contains(input.SubObject.ContentHash));
                    Assert.True(fixture.GetAllObjectHashes().Contains(syncPoint.ContentHash));

                    Assert.Equal(2, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject)).Length);
                    Assert.Equal(0, fixture.GetOrderedRevisionList(typeof (RootSerializationTestObject), "WeirdQualifier").Length);
                }

                return 0;
            });
        }

        [Fact]
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
                    sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromDays(1.0)));

                    Assert.Equal(input.ContentHash, fixture.GetLatestRootObject<RootSerializationTestObject>().ContentHash);
                    Assert.Equal(null, fixture.GetLatestRootObject<RootSerializationTestObject>("SomeWeirdQualifier"));
                    Assert.Equal(null, fixture.GetLatestRootObject<RootSerializationTestObject>(null, DateTimeOffset.Now - TimeSpan.FromHours(1.0)));

                    input.SomeInteger = 10;
                    syncPoint = fixture.CreateSyncPoint(input, null, DateTimeOffset.Now + TimeSpan.FromDays(1.0));
                    Assert.Equal(origHash, fixture.GetLatestRootObject<RootSerializationTestObject>(null, DateTimeOffset.Now + TimeSpan.FromSeconds(1.0)).ContentHash);
                }

                return 0;
            });
        }

        [Fact]
        public void LoadMethodWithInvalidKeyShouldReturnNoResults()
        {
            var fixture = createFixture();
            var output = fixture.Load(Guid.NewGuid());
            Assert.Equal(null, output);
        }

        [Fact]
        public void GetLatestRootObjectWithEmptyEngineShouldReturnNoResults()
        {
            var fixture = createExtendedFixture();
            var output = fixture.GetLatestRootObject<RootSerializationTestObject>();
            Assert.Equal(null, output);
        }

        [Fact]
        public void GetOrderedRevisionRangeWithEmptyEngineShouldReturnNoResults()
        {
            var fixture = createExtendedFixture();
            var output = fixture.GetOrderedRevisionList(typeof(Type));
            Assert.NotNull(output);
            Assert.Equal(0, output.Length);
        }

        [Fact]
        public void TryingToSaveNullGuidShouldBePunished()
        {
            var input = new ZeroContentHashModel();
            var fixture = createExtendedFixture();

            bool shouldFail = true;
            try {
                fixture.Save(input);
            } catch(Exception ex) {
                shouldFail = false;
            }
            Assert.False(shouldFail);

            shouldFail = true;
            try {
                fixture.CreateSyncPoint(input);
            } catch(Exception ex) {
                shouldFail = false;
            }
            Assert.False(shouldFail);
        }
    }

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
    public class EsentStorageEngineTest : StorageEngineInterfaceTest
    {
        static int _nextFreeSubdirectory = 1;
        protected override IStorageEngine createFixture(int key = -1)
        {
            var di = new DirectoryInfo(".");
            int subdir = (key == -1 ? Interlocked.Increment(ref _nextFreeSubdirectory) : key);

            di = new DirectoryInfo(Path.Combine(di.FullName, subdir.ToString()));
            if (key == -1 && di.Exists) {
                di.Delete(true);
            }

            di.Create();
            return new Esent.EsentStorageEngine(di.FullName);
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
