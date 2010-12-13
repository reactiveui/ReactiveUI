using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveXaml.Tests;

namespace ReactiveXaml.Serialization.Tests
{
    [PexClass, TestClass]
    public partial class SerializedCollectionTest
    {
        [PexMethod]
        public void AddingItemsShouldChangeTheContentHash(string[] toAdd) {
            PexAssume.IsNotNull(toAdd);
            PexAssume.AreElementsNotNull(toAdd);

            var sched = new TestScheduler();
            var fixture = sched.With(_ => new SerializedCollection<ModelTestFixture>());
            var hashes = new List<Guid>();
            int changeCount = 0;

            toAdd.ToObservable(sched).Subscribe(x => fixture.Add(new ModelTestFixture() {TestString = x}));
            fixture.ItemChanged.Subscribe(_ => {
                hashes.Add(fixture.ContentHash);
                changeCount++;
            });

            sched.Run();

            PexAssert.AreDistinctValues(hashes.ToArray());
            PexAssert.AreEqual(toAdd.Length, changeCount);
        }

        [PexMethod]
        public void RemovingItemsShouldChangeTheContentHash(string[] initialContents, int[] itemsToRemove) 
        {
            PexAssume.IsNotNullOrEmpty(initialContents);
            PexAssume.IsNotNullOrEmpty(itemsToRemove);
            PexAssume.AreDistinctValues(initialContents);
            PexAssume.AreDistinctValues(itemsToRemove);
            PexAssume.TrueForAll(itemsToRemove, x => x < initialContents.Length && x > 0);

            var sched = new TestScheduler();
            var fixture = sched.With(_ =>
                new SerializedCollection<ModelTestFixture>(initialContents.Select(x => new ModelTestFixture() { TestString = x })));
            var hashes = new List<Guid>();
            int changeCount = 0;

            fixture.ItemChanged.Subscribe(_ => {
                changeCount++;
                hashes.Add(fixture.ContentHash);
            });

            var toRemove = itemsToRemove.Select(x => fixture[x]);
            toRemove.ToObservable(sched).Subscribe(x => fixture.Remove(x));

            sched.Run();

            PexAssert.AreDistinctValues(hashes.ToArray());
            PexAssert.AreEqual(itemsToRemove.Length, changeCount);
        }

        [PexMethod(MaxBranches = 40000, MaxConstraintSolverTime = 5)]
        public void ChangingASerializableItemShouldChangeTheContentHash(string[] items, int toChange, string newValue)
        {
            PexAssume.IsNotNullOrEmpty(items);
            PexAssume.TrueForAll(items, x => x != null);
            PexAssume.AreDistinctReferences(items);
            PexAssume.IsTrue(toChange >= 0 && toChange < items.Length);

            var sched = new TestScheduler();
            var fixture = sched.With(_ => new SerializedCollection<ModelTestFixture>(
                items.Select(x => new ModelTestFixture() {TestString = x})));
            bool shouldDie = true;
            var hashBefore = fixture.ContentHash;
            PexAssume.AreNotEqual(newValue, fixture[toChange].TestString);

            fixture.ItemChanged.Subscribe(_ => shouldDie = false);
            Observable.Return(newValue, sched).Subscribe(x => fixture[toChange].TestString = x);

            sched.Run();

            Assert.AreNotEqual(hashBefore, fixture.ContentHash);
            Assert.IsFalse(shouldDie);
        }

        [TestMethod]
        public void ChangingASerializableItemShouldChangeTheContentHashSmokeTest()
        {
            var items = new[] {"foo"};
            ChangingASerializableItemShouldChangeTheContentHash(items, 0, "bar");
        }

        [TestMethod]
        public void ChangesShouldPropagateThroughMultilevelCollections()
        {
            var sched = new TestScheduler();
            var input = sched.With(_ => new ModelTestFixture() {TestString = "Foo"});
            var coll = sched.With(_ => new SerializedCollection<ISerializableItemBase>(new[] {input}));
            var fixture = sched.With(_ => 
                new SerializedCollection<ISerializableList<ISerializableItemBase>>(new[] {(ISerializableList<ISerializableItemBase>)coll}));

            bool inputChanging = false; bool inputChanged = false;
            bool collChanging = false; bool collChanged = false;
            bool fixtureChanging = false; bool fixtureChanged = false;
            input.ItemChanging.Subscribe(_ => inputChanging = true);
            input.ItemChanged.Subscribe(_ => inputChanged = true);
            coll.ItemChanging.Subscribe(_ => collChanging = true);
            coll.ItemChanging.Subscribe(_ => collChanged = true);
            fixture.ItemChanging.Subscribe(_ => fixtureChanging = true);
            fixture.ItemChanged.Subscribe(_ => fixtureChanged = true);

            input.TestString = "Bar";
            sched.RunToMilliseconds(1000);

            Assert.IsTrue(inputChanging);
            Assert.IsTrue(inputChanged);
            Assert.IsTrue(collChanging);
            Assert.IsTrue(collChanged);
            Assert.IsTrue(fixtureChanging);
            Assert.IsTrue(fixtureChanged);
        }
    }
}