// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>
/// Shows <c>AutoPersist</c> on a notes app: a <see cref="Note"/> saves itself a few moments after its last change,
/// using explicit <see cref="AutoPersistHelperMixins.AutoPersistMetadata"/> instead of reflecting over the type.
/// </summary>
public static class NoteExamples
{
    /// <summary>
    /// <c>AutoPersistMetadata</c> names the <c>[DataMember]</c> properties to watch, so <c>AutoPersist</c> never
    /// reflects over the note's type. A save runs once the note has been quiet for the given interval.
    /// </summary>
    /// <returns>A task that completes once the debounced save has run.</returns>
    public static async Task SavesAfterAQuietPeriod()
    {
        Note note = new() { Title = "Groceries" };
        AutoPersistHelperMixins.AutoPersistMetadata metadata = new(
            hasDataContract: true,
            persistablePropertyNames: new HashSet<string> { "Title", "Body" });

        Console.WriteLine(metadata.HasDataContract);
        Console.WriteLine(metadata.PersistablePropertyNames.Count);

        int saveCount = 0;
        using IDisposable subscription = note.AutoPersist(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            metadata,
            TimeSpan.FromMilliseconds(30));

        note.Body = "Milk, eggs, bread";
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Console.WriteLine(saveCount);

        // Output:
        // True
        // 2
        // 1
    }

    /// <summary>
    /// A manual save signal forces a save even when nothing changed, but it still waits out the quiet period; only a
    /// change to a <c>[DataMember]</c> property (never <see cref="Note.LastOpened"/>) requests a save on its own.
    /// </summary>
    /// <returns>A task that completes once both waits have elapsed.</returns>
    public static async Task SavesOnManualSignal()
    {
        Note note = new();
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = new(
            hasDataContract: true,
            persistablePropertyNames: new HashSet<string> { "Title", "Body" });

        int saveCount = 0;
        using IDisposable subscription = note.AutoPersist(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata,
            TimeSpan.FromMilliseconds(30));

        note.LastOpened = new DateTimeOffset(2026, 1, 1, 9, 0, 0, TimeSpan.Zero);
        await Task.Delay(TimeSpan.FromMilliseconds(60));
        Console.WriteLine(saveCount);

        manualSave.OnNext(RxVoid.Default);
        await Task.Delay(TimeSpan.FromMilliseconds(60));
        Console.WriteLine(saveCount);

        // Output:
        // 0
        // 1
    }

    /// <summary>Without an explicit interval, <c>AutoPersist</c> waits out a three-second quiet period before saving.</summary>
    public static void DefaultIntervalIsThreeSeconds()
    {
        Note note = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = new(
            hasDataContract: true,
            persistablePropertyNames: new HashSet<string> { "Title", "Body" });

        int saveCount = 0;
        IDisposable subscription = note.AutoPersist(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            metadata);

        note.Title = "Shopping list";

        // Disposed straight away: the default three-second quiet period never elapses, so no save runs.
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }

    /// <summary>The manual-signal overload without an interval also defaults to a three-second quiet period.</summary>
    public static void ManualSaveSignalWithDefaultInterval()
    {
        Note note = new();
        Signal<RxVoid> manualSave = new();
        AutoPersistHelperMixins.AutoPersistMetadata metadata = new(
            hasDataContract: true,
            persistablePropertyNames: new HashSet<string> { "Title", "Body" });

        int saveCount = 0;
        IDisposable subscription = note.AutoPersist(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            metadata);

        note.Title = "Groceries";
        manualSave.OnNext(RxVoid.Default);

        // Disposed straight away: the manual signal only starts the same three-second timer, which never elapses.
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }
}
