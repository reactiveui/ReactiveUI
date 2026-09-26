// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>
/// Shows <c>AutoPersistCollection</c>: it applies <c>AutoPersist</c> to every item already in a collection, and to
/// every item added later, using explicit <see cref="AutoPersistHelperMixins.AutoPersistMetadata"/> so it never
/// reflects over the item's type. The examples cover an <see cref="ObservableCollection{T}"/>, a
/// <see cref="ReadOnlyObservableCollection{T}"/> and a custom collection such as <see cref="NoteFeed"/>.
/// </summary>
public static class CollectionPersistExamples
{
    /// <summary>Without an explicit interval, each item in the collection waits out the default three-second quiet period.</summary>
    public static void DefaultIntervalAppliesToEveryItem()
    {
        ObservableCollection<Note> notes = new() { new Note { Title = "Groceries" } };
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        int saveCount = 0;
        IDisposable subscription = notes.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            metadata);

        notes[0].Title = "Shopping list";

        // Disposed straight away: the default three-second quiet period never elapses, so no save runs.
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }

    /// <summary>An explicit interval lets every note in the collection save once it has been quiet for that long.</summary>
    /// <returns>A task that completes once both notes have saved.</returns>
    public static async Task PersistsEachNoteAfterAQuietPeriod()
    {
        ObservableCollection<Note> notes = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        // Each note saves on a background thread, so two notes can save at the same moment.
        // A ConcurrentQueue keeps both titles where a List<T> could lose one.
        ConcurrentQueue<string> saved = new();
        using IDisposable subscription = notes.AutoPersistCollection(
            note =>
            {
                saved.Enqueue(note.Title);
                return Signal.Emit(RxVoid.Default);
            },
            metadata,
            TimeSpan.FromMilliseconds(30));

        Note groceries = new();
        Note chores = new();
        notes.Add(groceries);
        notes.Add(chores);

        // Only a change after the item joins the collection requests a save.
        groceries.Title = "Groceries";
        chores.Title = "Chores";
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Console.WriteLine(saved.Count);
        Console.WriteLine(string.Join(", ", saved.Order(StringComparer.Ordinal)));

        // Output:
        // 2
        // Chores, Groceries
    }

    /// <summary>A manual save signal forces every note in the collection to save, but still waits out the quiet period.</summary>
    /// <returns>A task that completes once the forced save has run.</returns>
    public static async Task ManualSaveSignalForcesEveryItemToSave()
    {
        ObservableCollection<Note> notes = new() { new Note { Title = "Groceries" } };
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        int saveCount = 0;
        using IDisposable subscription = notes.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata,
            TimeSpan.FromMilliseconds(30));

        manualSave.OnNext(RxVoid.Default);
        await Task.Delay(TimeSpan.FromMilliseconds(60));

        Console.WriteLine(saveCount);

        // Output:
        // 1
    }

    /// <summary>The manual-signal overload without an interval also defaults to a three-second quiet period.</summary>
    public static void ManualSaveSignalWithoutIntervalUsesTheDefault()
    {
        ObservableCollection<Note> notes = new() { new Note { Title = "Groceries" } };
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        int saveCount = 0;
        IDisposable subscription = notes.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata);

        manualSave.OnNext(RxVoid.Default);

        // Disposed straight away: the manual signal only starts the same three-second timer, which never elapses.
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }

    /// <summary>
    /// <c>AutoPersistCollection</c> works the same way on a <see cref="ReadOnlyObservableCollection{T}"/>: it watches the
    /// read-only view, but new items still have to arrive through the writable source collection behind it.
    /// </summary>
    /// <returns>A task that completes once the new note has saved.</returns>
    public static async Task PersistsThroughAReadOnlyView()
    {
        ObservableCollection<Note> source = new();
        ReadOnlyObservableCollection<Note> notes = new(source);
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        List<string> saved = [];
        using IDisposable subscription = notes.AutoPersistCollection(
            note =>
            {
                saved.Add(note.Title);
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata,
            TimeSpan.FromMilliseconds(30));

        Note chores = new();
        source.Add(chores);

        // Only a change after the item joins the collection requests a save.
        chores.Title = "Chores";
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Console.WriteLine(saved.Count);
        Console.WriteLine(saved[0]);

        // Output:
        // 1
        // Chores
    }

    /// <summary>Without an interval, a manual save signal on a read-only view still waits out the default three seconds.</summary>
    public static void ManualSaveSignalOnAReadOnlyViewUsesTheDefaultInterval()
    {
        ObservableCollection<Note> source = new() { new Note { Title = "Groceries" } };
        ReadOnlyObservableCollection<Note> notes = new(source);
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        int saveCount = 0;
        IDisposable subscription = notes.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata);

        manualSave.OnNext(RxVoid.Default);
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }

    /// <summary>
    /// <c>AutoPersistCollection</c> also works on any collection that raises <c>CollectionChanged</c>, such as a
    /// <see cref="NoteFeed"/> backed by a sync client instead of an <see cref="ObservableCollection{T}"/>.
    /// </summary>
    /// <returns>A task that completes once the published note has saved.</returns>
    public static async Task PersistsEveryNoteInACustomFeed()
    {
        NoteFeed feed = new();
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        List<string> saved = [];
        using IDisposable subscription = feed.AutoPersistCollection(
            (Note note) =>
            {
                saved.Add(note.Title);
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata,
            TimeSpan.FromMilliseconds(30));

        Note chores = new();
        feed.Publish(chores);

        // Only a change after the item joins the feed requests a save.
        chores.Title = "Chores";
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Console.WriteLine(saved.Count);
        Console.WriteLine(saved[0]);

        // Output:
        // 1
        // Chores
    }

    /// <summary>Without an interval, a manual save signal on a custom feed still waits out the default three seconds.</summary>
    public static void ManualSaveSignalOnACustomFeedUsesTheDefaultInterval()
    {
        NoteFeed feed = new();
        feed.Publish(new Note { Title = "Groceries" });
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = AutoPersistHelperMixins.CreateMetadata<Note>();

        int saveCount = 0;
        IDisposable subscription = feed.AutoPersistCollection(
            (Note _) =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata);

        manualSave.OnNext(RxVoid.Default);
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }

    /// <summary>
    /// A metadata provider computes <see cref="AutoPersistHelperMixins.AutoPersistMetadata"/> per item instead of once for
    /// the whole collection. <see cref="AutoPersistHelperMixins.CreateMetadataProvider{TItem}"/> builds one that always
    /// returns the same metadata for a homogeneous collection.
    /// </summary>
    /// <returns>A task that completes once the published note has saved.</returns>
    public static async Task PersistsUsingAMetadataProviderPerItem()
    {
        NoteFeed feed = new();
        Signal<RxVoid> manualSave = new();
        Func<Note, AutoPersistHelperMixins.AutoPersistMetadata> metadataProvider = AutoPersistHelperMixins.CreateMetadataProvider<Note>();

        List<string> saved = [];
        using IDisposable subscription = feed.AutoPersistCollection(
            note =>
            {
                saved.Add(note.Title);
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadataProvider,
            TimeSpan.FromMilliseconds(30));

        Note chores = new();
        feed.Publish(chores);

        // Only a change after the item joins the feed requests a save.
        chores.Title = "Chores";
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Console.WriteLine(saved.Count);
        Console.WriteLine(saved[0]);

        // Output:
        // 1
        // Chores
    }

    /// <summary>Without an interval, the metadata-provider overload still waits out the default three-second quiet period.</summary>
    public static void MetadataProviderWithoutIntervalUsesTheDefault()
    {
        NoteFeed feed = new();
        feed.Publish(new Note { Title = "Groceries" });
        Signal<RxVoid> manualSave = new();
        Func<Note, AutoPersistHelperMixins.AutoPersistMetadata> metadataProvider = AutoPersistHelperMixins.CreateMetadataProvider<Note>();

        int saveCount = 0;
        IDisposable subscription = feed.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadataProvider);

        manualSave.OnNext(RxVoid.Default);
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }
}
