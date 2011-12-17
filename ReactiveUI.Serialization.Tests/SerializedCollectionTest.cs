using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.Reactive.Testing;
using Xunit;
using ReactiveUI.Testing;
using ReactiveUI.Tests;

namespace ReactiveUI.Serialization.Tests
{
    [PexClass(MaxBranches=40000)]
    public partial class SerializedCollectionTest
    {
        [PexMethod]
        public void AddingItemsShouldChangeTheContentHash(string[] toAdd) 
        {
            PexAssume.IsNotNull(toAdd);
            PexAssume.AreElementsNotNull(toAdd);
            PexAssume.IsTrue(toAdd.Length > 0);

            (new TestScheduler()).With(sched => {
                var fixture = new SerializedCollection<ModelTestFixture>();
                var hashes = new List<Guid>();
                int changeCount = 0;

                fixture.Changed.Subscribe(_ => {
                    hashes.Add(fixture.ContentHash);
                    changeCount++;
                });

                foreach (var v in toAdd) {
                    fixture.Add(new ModelTestFixture() {TestString = v});
                    sched.Start();
                }

                PexAssert.AreDistinctValues(hashes.ToArray());
                PexAssert.AreEqual(toAdd.Length, fixture.Count);
                PexAssert.AreEqual(toAdd.Uniq().Count(), changeCount);
            });

            
        }

        [PexMethod]
        public void RemovingItemsShouldChangeTheContentHash(string[] initialContents, int[] itemsToRemove) 
        {
            PexAssume.IsNotNullOrEmpty(initialContents);
            PexAssume.IsNotNullOrEmpty(itemsToRemove);
            PexAssume.AreDistinctValues(initialContents);
            PexAssume.AreDistinctValues(itemsToRemove);
            PexAssume.TrueForAll(itemsToRemove, x => x < initialContents.Length && x > 0);

            (new TestScheduler()).With(sched => {
                var fixture = new SerializedCollection<ModelTestFixture>(initialContents.Select(x => new ModelTestFixture() { TestString = x }));
                var hashes = new List<Guid>();
                int changeCount = 0;

                fixture.Changed.Subscribe(_ => {
                    changeCount++;
                    hashes.Add(fixture.ContentHash);
                });

                var toRemove = itemsToRemove.Select(x => fixture[x]);
                foreach(var v in toRemove) {
                    fixture.Remove(v);
                }

                sched.Start();

                PexAssert.AreDistinctValues(hashes.ToArray());
                PexAssert.AreEqual(itemsToRemove.Length, changeCount);
            });

        }

        [PexMethod(MaxConditions = 1000)]
        public void ChangingASerializableItemShouldChangeTheContentHash(string[] items, int toChange, string newValue)
        {
            PexAssume.IsNotNullOrEmpty(items);
            PexAssume.TrueForAll(items, x => x != null);
            PexAssume.AreDistinctReferences(items);
            PexAssume.IsTrue(toChange >= 0 && toChange < items.Length);

            (new TestScheduler()).With(sched => {

                var fixture = new SerializedCollection<ModelTestFixture>(
                    items.Select(x => new ModelTestFixture() {TestString = x}));
                bool shouldDie = true;
                var hashBefore = fixture.ContentHash;
                PexAssume.AreNotEqual(newValue, fixture[toChange].TestString);

                fixture.Changed.Subscribe(_ => shouldDie = false);
                Observable.Return(newValue, sched).Subscribe(x => fixture[toChange].TestString = x);

                sched.Start();

                PexAssert.AreNotEqual(hashBefore, fixture.ContentHash);
                PexAssert.IsFalse(shouldDie);
            });

        }

        [Fact]
        public void ChangingASerializableItemShouldChangeTheContentHashSmokeTest()
        {
            var items = new[] {"foo"};
            ChangingASerializableItemShouldChangeTheContentHash(items, 0, "bar");
        }

        [Fact]
        public void ChangesShouldPropagateThroughMultilevelCollections()
        {
            (new TestScheduler()).With(sched => {
                var input = new ModelTestFixture() {TestString = "Foo"};
                var coll = new SerializedCollection<ISerializableItem>(new[] {input});
                var fixture =  new SerializedCollection<ISerializableList<ISerializableItem>>(new[] {(ISerializableList<ISerializableItem>)coll});

                bool inputChanging = false; bool inputChanged = false;
                bool collChanging = false; bool collChanged = false;
                bool fixtureChanging = false; bool fixtureChanged = false;
                input.Changing.Subscribe(_ => inputChanging = true);
                input.Changed.Subscribe(_ => inputChanged = true);
                coll.ItemChanging.Subscribe(_ => collChanging = true);
                coll.ItemChanging.Subscribe(_ => collChanged = true);
                fixture.ItemChanging.Subscribe(_ => fixtureChanging = true);
                fixture.ItemChanged.Subscribe(_ => fixtureChanged = true);

                input.TestString = "Bar";
                sched.RunToMilliseconds(1000);

                Assert.True(inputChanging);
                Assert.True(inputChanged);
                Assert.True(collChanging);
                Assert.True(collChanged);
                Assert.True(fixtureChanging);
                Assert.True(fixtureChanged);
            });
        }
    }
}
