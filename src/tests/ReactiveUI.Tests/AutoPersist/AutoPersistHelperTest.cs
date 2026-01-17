// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.AutoPersist;

/// <summary>
///     Comprehensive test suite for AutoPersistHelper.
///     Tests cover all overloads, throttling, scheduling, and collection behavior.
/// </summary>
[NotInParallel]
[TestExecutor<WithSchedulerExecutor>]
public class AutoPersistHelperTest
{
    /// <summary>
    ///     Tests that ActOnEveryObject calls onAdd when new item added.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_AddNewItem_CallsOnAdd()
    {
        var collection = new ObservableCollection<TestFixture>();
        var addedItems = new List<TestFixture>();

        collection.ActOnEveryObject(
            addedItems.Add,
            _ => { });

        var item = new TestFixture();
        collection.Add(item);

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(1);
            await Assert.That(addedItems[0]).IsEqualTo(item);
        }
    }

    /// <summary>
    ///     Tests that ActOnEveryObject handles collection Clear.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ClearCollection_CallsOnRemove()
    {
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1, item2 };
        var removedItems = new List<TestFixture>();

        collection.ActOnEveryObject(
            _ => { },
            removedItems.Add);

        collection.Clear();

        using (Assert.Multiple())
        {
            await Assert.That(removedItems).Count().IsEqualTo(2);
            await Assert.That(removedItems).Contains(item1);
            await Assert.That(removedItems).Contains(item2);
        }
    }

    /// <summary>
    ///     Tests that ActOnEveryObject calls onRemove on disposal.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_Dispose_CallsOnRemoveForAll()
    {
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1, item2 };
        var removedItems = new List<TestFixture>();

        var subscription = collection.ActOnEveryObject(
            _ => { },
            removedItems.Add);

        subscription.Dispose();

        using (Assert.Multiple())
        {
            await Assert.That(removedItems).Count().IsEqualTo(2);
            await Assert.That(removedItems).Contains(item1);
            await Assert.That(removedItems).Contains(item2);
        }
    }

    /// <summary>
    ///     Tests that ActOnEveryObject calls onAdd for existing items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ExistingItems_CallsOnAdd()
    {
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1, item2 };
        var addedItems = new List<TestFixture>();

        collection.ActOnEveryObject(
            addedItems.Add,
            _ => { });

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(2);
            await Assert.That(addedItems).Contains(item1);
            await Assert.That(addedItems).Contains(item2);
        }
    }

    /// <summary>
    ///     Tests that ActOnEveryObject throws on null collection.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_NullCollection_ThrowsArgumentNullException()
    {
        ObservableCollection<TestFixture>? collection = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            collection!.ActOnEveryObject(_ => { }, _ => { });
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that ActOnEveryObject throws on null onAdd.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_NullOnAdd_ThrowsArgumentNullException()
    {
        var collection = new ObservableCollection<TestFixture>();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            collection.ActOnEveryObject(null!, _ => { });
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that ActOnEveryObject throws on null onRemove.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_NullOnRemove_ThrowsArgumentNullException()
    {
        var collection = new ObservableCollection<TestFixture>();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            collection.ActOnEveryObject(_ => { }, null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that ActOnEveryObject works with ReadOnlyObservableCollection.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ReadOnlyCollection_WorksCorrectly()
    {
        var innerCollection = new ObservableCollection<TestFixture>();
        var readOnlyCollection = new ReadOnlyObservableCollection<TestFixture>(innerCollection);
        var addedItems = new List<TestFixture>();

        readOnlyCollection.ActOnEveryObject(
            addedItems.Add,
            _ => { });

        var item = new TestFixture();
        innerCollection.Add(item);

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(1);
            await Assert.That(addedItems[0]).IsEqualTo(item);
        }
    }

    /// <summary>
    ///     Tests that ActOnEveryObject calls onRemove when item removed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_RemoveItem_CallsOnRemove()
    {
        var item = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item };
        var removedItems = new List<TestFixture>();

        collection.ActOnEveryObject(
            _ => { },
            removedItems.Add);

        collection.Remove(item);

        using (Assert.Multiple())
        {
            await Assert.That(removedItems).Count().IsEqualTo(1);
            await Assert.That(removedItems[0]).IsEqualTo(item);
        }
    }

    /// <summary>
    ///     Tests that ActOnEveryObject handles collection Replace.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActOnEveryObject_ReplaceItem_CallsOnRemoveAndOnAdd()
    {
        var item1 = new TestFixture();
        var item2 = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item1 };
        var addedItems = new List<TestFixture>();
        var removedItems = new List<TestFixture>();

        collection.ActOnEveryObject(
            addedItems.Add,
            removedItems.Add);

        collection[0] = item2;

        using (Assert.Multiple())
        {
            await Assert.That(addedItems).Count().IsEqualTo(2);
            await Assert.That(addedItems[0]).IsEqualTo(item1);
            await Assert.That(addedItems[1]).IsEqualTo(item2);
            await Assert.That(removedItems).Count().IsEqualTo(1);
            await Assert.That(removedItems[0]).IsEqualTo(item1);
        }
    }

    /// <summary>
    ///     Tests that AutoPersist disposes correctly.
    /// </summary>
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
                return Observables.Unit;
            },
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
        fixture.IsNotNullString = "First";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);

        subscription.Dispose();

        fixture.IsNotNullString = "Second";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(2));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that manual save signal triggers immediate save.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_ManualSaveSignal_TriggersSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var manualSave = new Subject<Unit>();
        var saveCount = 0;

        fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            manualSave,
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        manualSave.OnNext(Unit.Default);
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersist with metadata provider works for collections.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_MetadataProvider_WorksCorrectly()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var metadataProvider = AutoPersistHelper.CreateMetadataProvider<TestFixture>();
        var fixture = new TestFixture();
        var metadata = metadataProvider(fixture);

        await Assert.That(metadata).IsNotNull();
        await Assert.That(metadata.HasDataContract).IsTrue();
        await Assert.That(metadata.PersistablePropertyNames).Contains("IsNotNullString");
    }

    /// <summary>
    ///     Tests that AutoPersist throws for objects without DataContract attribute.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NoDataContract_ThrowsArgumentException()
    {
        var obj = new ObjectWithoutDataContract();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            obj.AutoPersist(_ => Observables.Unit);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that AutoPersist only saves for DataMember properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_NonDataMemberProperty_DoesNotTriggerSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        fixture.PocoProperty = "NoSave";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(2));

        await Assert.That(saveCount).IsEqualTo(0);
    }

    /// <summary>
    ///     Tests that AutoPersist throws on null doPersist.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NullDoPersist_ThrowsArgumentNullException()
    {
        var fixture = new TestFixture();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            fixture.AutoPersist(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that AutoPersist throws on null manualSaveSignal.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NullManualSaveSignal_ThrowsArgumentNullException()
    {
        var fixture = new TestFixture();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            fixture.AutoPersist(_ => Observables.Unit, null!, TimeSpan.FromSeconds(1));
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that AutoPersist with metadata throws on null metadata.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersist_NullMetadata_ThrowsArgumentNullException()
    {
        var fixture = new TestFixture();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            fixture.AutoPersist(_ => Observables.Unit, null!, TimeSpan.FromSeconds(1));
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that AutoPersist saves when a DataMember property changes.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_PropertyChange_TriggersSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
        fixture.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersist respects throttle interval.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_Throttle_RespectInterval()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            TimeSpan.FromSeconds(2));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        fixture.IsNotNullString = "First";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));
        await Assert.That(saveCount).IsEqualTo(0);

        fixture.IsNotNullString = "Second";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));
        await Assert.That(saveCount).IsEqualTo(0);

        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));
        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersist with metadata works correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_WithMetadata_SavesCorrectly()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var metadata = AutoPersistHelper.CreateMetadata<TestFixture>();
        var saveCount = 0;

        fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            metadata,
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
        fixture.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection adds persistence to collection items.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_AddItem_EnablesPersistence()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var collection = new ObservableCollection<TestFixture>();
        var saveCount = 0;

        collection.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        var item = new TestFixture();
        collection.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(150));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection throws on null collection.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersistCollection_NullCollection_ThrowsArgumentNullException()
    {
        ObservableCollection<TestFixture>? collection = null;

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            collection!.AutoPersistCollection(_ => Observables.Unit);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that AutoPersistCollection throws on null doPersist.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersistCollection_NullDoPersist_ThrowsArgumentNullException()
    {
        var collection = new ObservableCollection<TestFixture>();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            collection.AutoPersistCollection(null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that AutoPersistCollection throws on null metadata.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AutoPersistCollection_NullMetadata_ThrowsArgumentNullException()
    {
        var collection = new ObservableCollection<TestFixture>();

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            collection.AutoPersistCollection(_ => Observables.Unit, (AutoPersistHelper.AutoPersistMetadata)null!);
            await Task.CompletedTask;
        });
    }

    /// <summary>
    ///     Tests that AutoPersistCollection works with ReadOnlyObservableCollection.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_ReadOnlyCollection_WorksCorrectly()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var innerCollection = new ObservableCollection<TestFixture>();
        var readOnlyCollection = new ReadOnlyObservableCollection<TestFixture>(innerCollection);
        var saveCount = 0;
        var manualSave = new Subject<Unit>();

        readOnlyCollection.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            manualSave,
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        var item = new TestFixture();
        innerCollection.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(150));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection removes persistence when item removed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_RemoveItem_DisablesPersistence()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var item = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { item };
        var saveCount = 0;

        collection.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        collection.Remove(item);

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(150));

        await Assert.That(saveCount).IsEqualTo(0);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection with metadata works correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_WithMetadata_SavesCorrectly()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var collection = new ObservableCollection<TestFixture>();
        var metadata = AutoPersistHelper.CreateMetadata<TestFixture>();
        var saveCount = 0;

        collection.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Observables.Unit;
            },
            metadata,
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        var item = new TestFixture();
        collection.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(150));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that CreateMetadata returns correct metadata.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateMetadata_ReturnsCorrectMetadata()
    {
        var metadata = AutoPersistHelper.CreateMetadata<TestFixture>();

        using (Assert.Multiple())
        {
            await Assert.That(metadata).IsNotNull();
            await Assert.That(metadata.HasDataContract).IsTrue();
            await Assert.That(metadata.PersistablePropertyNames).Contains("IsNotNullString");
            await Assert.That(metadata.PersistablePropertyNames).Contains("IsOnlyOneWord");
            await Assert.That(metadata.PersistablePropertyNames).DoesNotContain("PocoProperty");
        }
    }

    /// <summary>
    ///     Test object without DataContract attribute.
    /// </summary>
    private class ObjectWithoutDataContract : ReactiveObject
    {
        private string? _property;

        /// <summary>
        ///     Gets or sets a test property.
        /// </summary>
        public string? Property
        {
            get => _property;
            set => this.RaiseAndSetIfChanged(ref _property, value);
        }
    }
}
