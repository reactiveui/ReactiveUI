// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Database;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using ReactiveUI.AndroidX;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests the AndroidX RecyclerView and pager adapters against real widgets.</summary>
public class AdapterTests
{
    /// <summary>The number of items each test collection starts with.</summary>
    private const int InitialCount = 2;

    /// <summary>The adapter starts with the collection's items and raises one notification per collection change.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task RecyclerAdapter_MirrorsCollectionChangesAsItemNotifications()
    {
        var (count, events, finalCount) = await MainThread.RunAsync(static () =>
        {
            ObservableCollection<TestViewModel> items = [new("a"), new("b")];
            using var adapter = new TestRecyclerAdapter(items.ToReactiveChangeSet());
            var initialCount = adapter.ItemCount;
            var observer = new RecordingDataObserver();
            adapter.RegisterAdapterDataObserver(observer);

            items.Add(new("c"));
            items.RemoveAt(0);
            items.Move(0, 1);
            items[0] = new("replaced");

            adapter.UnregisterAdapterDataObserver(observer);
            return (initialCount, observer.Events, adapter.ItemCount);
        });

        await Assert.That(count).IsEqualTo(InitialCount);
        await Assert.That(events).IsEquivalentTo(["insert 2 1", "remove 0 1", "move 0 1 1", "change 0 1"]);
        await Assert.That(finalCount).IsEqualTo(InitialCount);
    }

    /// <summary>In a real RecyclerView, binding a row sets its view model, clicks report the row, and rows activate while attached.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task RecyclerAdapter_InARecyclerView_BindsSelectsAndActivatesRows()
    {
        var activity = await ActivityLauncher.StartAsync<HostActivity>();
        try
        {
            ObservableCollection<TestViewModel> items = [new("zero"), new("one"), new("two")];
            var recycler = await MainThread.RunAsync(() =>
            {
                var view = new RecyclerView(activity);
                view.SetLayoutManager(new LinearLayoutManager(activity));
                view.SetAdapter(new TestRecyclerAdapter(items.ToReactiveChangeSet()));
                activity.Container.AddView(view);
                return view;
            });

            await Eventually.TrueAsync(() => recycler.FindViewHolderForAdapterPosition(1) is not null, "row 1 to be laid out");
            var holder = (TestViewHolder)recycler.FindViewHolderForAdapterPosition(1)!;

            await Assert.That(holder.ViewModel).IsSameReferenceAs(items[1]);
            await Eventually.TrueAsync(() => holder.Activations == 1, "row 1 to activate");

            var selected = holder.Selected.FirstValueAsync(out var selectedSubscription);
            var selectedViewModel = holder.SelectedWithViewModel.FirstValueAsync(out var viewModelSubscription);
            using (selectedSubscription)
            using (viewModelSubscription)
            {
                _ = await MainThread.RunAsync(holder.ItemView.PerformClick);

                await Assert.That(await selected.WithTimeout("Selected")).IsEqualTo(1);
                await Assert.That(await selectedViewModel.WithTimeout("SelectedWithViewModel")).IsSameReferenceAs(items[1]);
            }

            var longClicked = holder.LongClicked.FirstValueAsync(out var longClickSubscription);
            using (longClickSubscription)
            {
                _ = await MainThread.RunAsync(holder.ItemView.PerformLongClick);

                await Assert.That(await longClicked.WithTimeout("LongClicked")).IsEqualTo(1);
            }

            await MainThread.RunAsync(() => activity.Container.RemoveAllViews());
            await Eventually.TrueAsync(() => holder.Deactivations == 1, "row 1 to deactivate once detached");
        }
        finally
        {
            await ActivityLauncher.FinishAsync(activity);
        }
    }

    /// <summary>The pager adapter counts the collection, sets each page's view model through its view host, and reports each batch.</summary>
    /// <returns>A task representing the test.</returns>
    [Test]
    public async Task PagerAdapter_CreatesPagesThroughViewHosts()
    {
        var (count, pageViewModel, childCount, changes, finalCount, first) = await MainThread.RunAsync(static () =>
        {
            ObservableCollection<TestViewModel> items = [new("page 0"), new("page 1")];
            var context = ActivityLauncher.TargetContext;
            using var adapter = new ReactivePagerAdapter<TestViewModel>(
                items.ToReactiveChangeSet(),
                (_, parent) => new TestViewHost(context, parent).View!);
            var initialCount = adapter.Count;

            var container = new FrameLayout(context);
            var page = (Android.Views.View)adapter.InstantiateItem(container, 1);
            var host = (TestViewHost)page.GetViewHost()!;

            var observer = new CountingDataSetObserver();
            adapter.RegisterDataSetObserver(observer);
            items.Add(new("page 2"));
            adapter.UnregisterDataSetObserver(observer);

            return (initialCount, host.ViewModel, container.ChildCount, observer.Changes, adapter.Count, items[1]);
        });

        await Assert.That(count).IsEqualTo(InitialCount);
        await Assert.That(pageViewModel).IsSameReferenceAs(first);
        await Assert.That(childCount).IsEqualTo(1);
        await Assert.That(changes).IsEqualTo(1);
        await Assert.That(finalCount).IsEqualTo(InitialCount + 1);
    }

    /// <summary>Counts <see cref="DataSetObserver.OnChanged"/> calls.</summary>
    private sealed class CountingDataSetObserver : DataSetObserver
    {
        /// <summary>Gets the number of change notifications.</summary>
        public int Changes { get; private set; }

        /// <inheritdoc/>
        public override void OnChanged() => Changes++;
    }
}
