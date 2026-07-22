// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.AutoPersist;

/// <summary>
///     Comprehensive test suite for AutoPersistHelperMixins.
///     Tests cover all overloads, throttling, scheduling, and collection behavior.
/// </summary>
[NotInParallel]
[TestExecutor<WithSchedulerExecutor>]
public partial class AutoPersistHelperTest
{
    /// <summary>Milliseconds to advance the scheduler past initial subscription setup.</summary>
    private const int InitialAdvanceMilliseconds = 10;

    /// <summary>The throttle interval, in milliseconds, used by the collection persistence tests.</summary>
    private const int ThrottleMilliseconds = 100;

    /// <summary>Milliseconds to advance past the throttle interval to allow a save to fire.</summary>
    private const int PastThrottleMilliseconds = 150;

    /// <summary>The default debounce interval, in seconds, used by the reflection-based overloads.</summary>
    private const int DefaultIntervalSeconds = 3;

    /// <summary>Tests that ActOnEveryObject calls onAdd when new item added.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_AddNewItem_CallsOnAdd()
    {
        var collection = new ObservableCollection<TestFixture>();
        var addedItems = new List<TestFixture>();

        _ = collection.ActOnEveryObject(
            addedItems.Add,
            static _ => { });

        var item = new TestFixture();
        collection.Add(item);

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(1);
            await Assert.That(addedItems[0]).IsEqualTo(item);
        }
    }

    /// <summary>Tests that ActOnEveryObject handles collection Clear.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ClearCollection_CallsOnRemove()
    {
        const int ExpectedRemovedCount = 2;
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1, item2 };
        var removedItems = new List<TestFixture>();

        _ = collection.ActOnEveryObject(
            static _ => { },
            removedItems.Add);

        collection.Clear();

        using (Assert.Multiple())
        {
            await Assert.That(removedItems).Count().IsEqualTo(ExpectedRemovedCount);
            await Assert.That(removedItems).Contains(item1);
            await Assert.That(removedItems).Contains(item2);
        }
    }

    /// <summary>Tests that ActOnEveryObject calls onRemove on disposal.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_Dispose_CallsOnRemoveForAll()
    {
        const int ExpectedRemovedCount = 2;
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1, item2 };
        var removedItems = new List<TestFixture>();

        var subscription = collection.ActOnEveryObject(
            static _ => { },
            removedItems.Add);

        subscription.Dispose();

        using (Assert.Multiple())
        {
            await Assert.That(removedItems).Count().IsEqualTo(ExpectedRemovedCount);
            await Assert.That(removedItems).Contains(item1);
            await Assert.That(removedItems).Contains(item2);
        }
    }

    /// <summary>Tests that ActOnEveryObject calls onAdd for existing items.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ExistingItems_CallsOnAdd()
    {
        const int ExpectedAddedCount = 2;
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1, item2 };
        var addedItems = new List<TestFixture>();

        _ = collection.ActOnEveryObject(
            addedItems.Add,
            static _ => { });

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(ExpectedAddedCount);
            await Assert.That(addedItems).Contains(item1);
            await Assert.That(addedItems).Contains(item2);
        }
    }

    /// <summary>Tests that ActOnEveryObject throws on null collection.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_NullCollection_ThrowsArgumentNullException()
    {
        ObservableCollection<TestFixture>? collection = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = collection!.ActOnEveryObject(static _ => { }, static _ => { });
            await Task.CompletedTask;
        });
    }

    /// <summary>Tests that ActOnEveryObject throws on null onAdd.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_NullOnAdd_ThrowsArgumentNullException()
    {
        var collection = new ObservableCollection<TestFixture>();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = collection.ActOnEveryObject(null!, static _ => { });
            await Task.CompletedTask;
        });
    }

    /// <summary>Tests that ActOnEveryObject throws on null onRemove.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_NullOnRemove_ThrowsArgumentNullException()
    {
        var collection = new ObservableCollection<TestFixture>();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = collection.ActOnEveryObject(static _ => { }, null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>Tests that ActOnEveryObject works with ReadOnlyObservableCollection.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ReadOnlyCollection_WorksCorrectly()
    {
        var innerCollection = new ObservableCollection<TestFixture>();
        var readOnlyCollection = new ReadOnlyObservableCollection<TestFixture>(innerCollection);
        var addedItems = new List<TestFixture>();

        _ = readOnlyCollection.ActOnEveryObject(
            addedItems.Add,
            static _ => { });

        var item = new TestFixture();
        innerCollection.Add(item);

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(1);
            await Assert.That(addedItems[0]).IsEqualTo(item);
        }
    }

    /// <summary>Tests that ActOnEveryObject calls onRemove when item removed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_RemoveItem_CallsOnRemove()
    {
        var item = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item };
        var removedItems = new List<TestFixture>();

        _ = collection.ActOnEveryObject(
            static _ => { },
            removedItems.Add);

        _ = collection.Remove(item);

        using (Assert.Multiple())
        {
            await Assert.That(removedItems).Count().IsEqualTo(1);
            await Assert.That(removedItems[0]).IsEqualTo(item);
        }
    }

    /// <summary>Tests that ActOnEveryObject handles collection Replace.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ReplaceItem_CallsOnRemoveAndOnAdd()
    {
        const int ExpectedAddedCount = 2;
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1 };
        var addedItems = new List<TestFixture>();
        var removedItems = new List<TestFixture>();

        _ = collection.ActOnEveryObject(
            addedItems.Add,
            removedItems.Add);

        collection[0] = item2;

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(ExpectedAddedCount);
            await Assert.That(addedItems[0]).IsEqualTo(item1);
            await Assert.That(addedItems[1]).IsEqualTo(item2);
            await Assert.That(removedItems).Count().IsEqualTo(1);
            await Assert.That(removedItems[0]).IsEqualTo(item1);
        }
    }

    /// <summary>Tests that AutoPersist disposes correctly.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_Dispose_StopsSaving()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        var subscription = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromSeconds(1));

        const int AfterDisposeSeconds = 2;
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        fixture.IsNotNullString = "First";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);

        subscription.Dispose();

        fixture.IsNotNullString = "Second";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(AfterDisposeSeconds));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>Tests that manual save signal triggers immediate save.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_ManualSaveSignal_TriggersSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var manualSave = new Signal<RxVoid>();
        var saveCount = 0;

        _ = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            manualSave,
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        manualSave.OnNext(RxVoid.Default);
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>Tests that AutoPersist with metadata provider works for collections.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_MetadataProvider_WorksCorrectly()
    {
        var metadataProvider = AutoPersistHelperMixins.CreateMetadataProvider<TestFixture>();
        var fixture = new TestFixture();
        var metadata = metadataProvider(fixture);

        await Assert.That(metadata).IsNotNull();
        await Assert.That(metadata.HasDataContract).IsTrue();
        await Assert.That(metadata.PersistablePropertyNames).Contains("IsNotNullString");
    }

    /// <summary>Tests that AutoPersist throws for objects without DataContract attribute.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NoDataContract_ThrowsArgumentException()
    {
        var obj = new ObjectWithoutDataContract();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            _ = obj.AutoPersist(static _ => ImmutableReturnRxVoidSignal.Instance);
            await Task.CompletedTask;
        });
    }

    /// <summary>Tests that AutoPersist only saves for DataMember properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_NonDataMemberProperty_DoesNotTriggerSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        _ = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromSeconds(1));

        const int SmallAdvanceMilliseconds = 2;
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        fixture.PocoProperty = "NoSave";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(SmallAdvanceMilliseconds));

        await Assert.That(saveCount).IsEqualTo(0);
    }

    /// <summary>Tests that AutoPersist throws on null doPersist.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NullDoPersist_ThrowsArgumentNullException()
    {
        var fixture = new TestFixture();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = fixture.AutoPersist(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>Tests that AutoPersist throws on null manualSaveSignal.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NullManualSaveSignal_ThrowsArgumentNullException()
    {
        var fixture = new TestFixture();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = fixture.AutoPersist(static _ => ImmutableReturnRxVoidSignal.Instance, null!, TimeSpan.FromSeconds(1));
            await Task.CompletedTask;
        });
    }

    /// <summary>Tests that AutoPersist with metadata throws on null metadata.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NullMetadata_ThrowsArgumentNullException()
    {
        var fixture = new TestFixture();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            _ = fixture.AutoPersist(static _ => ImmutableReturnRxVoidSignal.Instance, null!, TimeSpan.FromSeconds(1));
            await Task.CompletedTask;
        });
    }

    /// <summary>Tests that AutoPersist saves when a DataMember property changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_PropertyChange_TriggersSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        _ = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        fixture.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>Tests that AutoPersist respects throttle interval.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_Throttle_RespectInterval()
    {
        const int ThrottleSeconds = 2;
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        _ = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromSeconds(ThrottleSeconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        fixture.IsNotNullString = "First";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));
        await Assert.That(saveCount).IsEqualTo(0);

        fixture.IsNotNullString = "Second";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));
        await Assert.That(saveCount).IsEqualTo(0);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));
        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>Tests that AutoPersist with metadata works correctly.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_WithMetadata_SavesCorrectly()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var metadata = AutoPersistHelperMixins.CreateMetadata<TestFixture>();
        var saveCount = 0;

        _ = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            metadata,
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        fixture.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>Tests that AutoPersistCollection adds persistence to collection items.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_AddItem_EnablesPersistence()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var collection = new ObservableCollection<TestFixture>();
        var saveCount = 0;

        _ = collection.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        var item = new TestFixture();
        collection.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>Test object without DataContract attribute.</summary>
    private sealed class ObjectWithoutDataContract : ReactiveObject
    {
        /// <summary>Gets or sets a test property.</summary>
        [SuppressMessage(
            "Design",
            "SST2324:Public member on a non-public type",
            Justification = "the public surface is required for interface/reflection binding; the containing " +
                "test double is an intentionally non-public detail.")]
        public string? Property
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>A <c>[DataContract]</c> base reactive type used to exercise runtime-type metadata resolution.</summary>
    [DataContract]
    private class DataContractBaseFixture : ReactiveObject
    {
        /// <summary>Gets or sets a persistable base value.</summary>
        [DataMember]
        [SuppressMessage(
            "Design",
            "SST2324:Public member on a non-public type",
            Justification = "the public surface is required for interface/reflection binding; the containing " +
                "test double is an intentionally non-public detail.")]
        public string? BaseValue
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>A derived reactive type whose runtime type differs from the statically known base type.</summary>
    [DataContract]
    private sealed class DerivedDataContractFixture : DataContractBaseFixture
    {
        /// <summary>Gets or sets a persistable derived value.</summary>
        [DataMember]
        public string? DerivedValue
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }

    /// <summary>A change-notifying collection that can raise a Replace with an arbitrary previous item.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    private sealed class ReplaceableCollection<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        /// <summary>The backing storage for the collection items.</summary>
        private readonly List<T> _items = [];

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>Raises a Replace notification swapping a previous item for a current item.</summary>
        /// <param name="previous">The previous item.</param>
        /// <param name="current">The current item.</param>
        /// <param name="index">The index at which the replace occurs.</param>
        public void RaiseReplace(T previous, T current, int index)
        {
            if (index < _items.Count)
            {
                _items[index] = current;
            }
            else
            {
                _items.Add(current);
            }

            CollectionChanged?.Invoke(
                this,
                new(
                    NotifyCollectionChangedAction.Replace,
                    current,
                    previous,
                    index));
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>A change-notifying collection that is neither <see cref="ObservableCollection{T}" /> nor read-only, used to bind the generic AutoPersist extension block.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    private sealed class CustomNotifyCollection<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        /// <summary>The backing storage for the collection items.</summary>
        private readonly List<T> _items = [];

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>Adds an item to the collection and raises a collection-changed notification.</summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            _items.Add(item);
            CollectionChanged?.Invoke(
                this,
                new(
                    NotifyCollectionChangedAction.Add,
                    item,
                    _items.Count - 1));
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
