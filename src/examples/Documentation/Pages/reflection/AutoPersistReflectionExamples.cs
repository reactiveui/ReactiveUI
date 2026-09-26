// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace ReactiveUI.Documentation.Reflection;

/// <summary>
/// Shows the <c>AutoPersist</c> and <c>AutoPersistCollection</c> overloads that reflect over an object's runtime
/// type to find its <c>[DataContract]</c>/<c>[DataMember]</c> attributes. The overloads that take an explicit
/// <see cref="AutoPersistHelperMixins.AutoPersistMetadata"/> — built once with
/// <see cref="AutoPersistHelperMixins.CreateMetadata{T}"/> — do the same job without reflecting at run time.
/// </summary>
public static class AutoPersistReflectionExamples
{
    /// <summary>Without an explicit interval, a single object waits out the default three-second quiet period.</summary>
    public static void PersistsASingleTrackAfterTheDefaultQuietPeriod()
    {
        PlaylistTrack track = new("Africa");

        int saveCount = 0;
        IDisposable subscription = track.AutoPersist(_ =>
        {
            saveCount++;
            return Signal.Emit(RxVoid.Default);
        });

        track.IsFavorite = true;

        // Disposed straight away: the default three-second quiet period never elapses, so no save runs.
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }

    /// <summary>An explicit interval lets the object save once it has been quiet for that long.</summary>
    /// <returns>A task that completes once the track has saved.</returns>
    public static async Task PersistsASingleTrackAfterAnExplicitQuietPeriod()
    {
        PlaylistTrack track = new("Bohemian Rhapsody");

        int saveCount = 0;
        using IDisposable subscription = track.AutoPersist(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            TimeSpan.FromMilliseconds(30));

        track.IsFavorite = true;
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Console.WriteLine(saveCount);

        // Output:
        // 1
    }

    /// <summary>A manual save signal forces a save even without a change, but still waits out the quiet period.</summary>
    /// <returns>A task that completes once the forced save has run.</returns>
    public static async Task ManualSaveSignalForcesASingleTrackToSave()
    {
        PlaylistTrack track = new("Clocks");
        Signal<RxVoid> manualSave = new();

        int saveCount = 0;
        using IDisposable subscription = track.AutoPersist(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
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
        PlaylistTrack track = new("Yellow");
        Signal<RxVoid> manualSave = new();

        int saveCount = 0;
        IDisposable subscription = track.AutoPersist(
            _ =>
            {
                saveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            manualSave);

        manualSave.OnNext(RxVoid.Default);

        // Disposed straight away: the manual signal only starts the same three-second timer, which never elapses.
        subscription.Dispose();

        Console.WriteLine(saveCount);

        // Output:
        // 0
    }

    /// <summary>
    /// <c>AutoPersistCollection</c> applies <c>AutoPersist</c> to every track already in the playlist, and to every
    /// track added later. The two overloads shown here differ only by the explicit interval.
    /// </summary>
    /// <returns>A task that completes once the timed collection has saved.</returns>
    public static async Task CollectionOverloadsDifferOnlyByInterval()
    {
        ObservableCollection<PlaylistTrack> undated = [new PlaylistTrack("Africa")];
        int undatedSaveCount = 0;
        IDisposable undatedSubscription = undated.AutoPersistCollection(_ =>
        {
            undatedSaveCount++;
            return Signal.Emit(RxVoid.Default);
        });
        undated[0].IsFavorite = true;

        // Disposed straight away: the default three-second quiet period never elapses, so no save runs.
        undatedSubscription.Dispose();

        ObservableCollection<PlaylistTrack> timed = [];
        List<string> saved = [];
        using IDisposable timedSubscription = timed.AutoPersistCollection(
            track =>
            {
                saved.Add(track.Title);
                return Signal.Emit(RxVoid.Default);
            },
            TimeSpan.FromMilliseconds(30));

        PlaylistTrack clocks = new("Clocks");
        timed.Add(clocks);

        // Only a change after the track joins the collection requests a save.
        clocks.IsFavorite = true;
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Console.WriteLine(undatedSaveCount);
        Console.WriteLine(string.Join(", ", saved));

        // Output:
        // 0
        // Clocks
    }

    /// <summary>
    /// The manual-save overloads on a playlist behave the same as on a single track: a signal forces every track to
    /// save, but the interval overload still waits out the quiet period while the no-interval overload defaults to it.
    /// </summary>
    /// <returns>A task that completes once the forced save has run.</returns>
    public static async Task CollectionManualSaveOverloads()
    {
        ObservableCollection<PlaylistTrack> playlist = [new PlaylistTrack("Yellow")];
        Signal<RxVoid> timedManualSave = new();

        int timedSaveCount = 0;
        using IDisposable timedSubscription = playlist.AutoPersistCollection(
            _ =>
            {
                timedSaveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            timedManualSave,
            TimeSpan.FromMilliseconds(30));

        timedManualSave.OnNext(RxVoid.Default);
        await Task.Delay(TimeSpan.FromMilliseconds(60));

        ObservableCollection<PlaylistTrack> otherPlaylist = [new PlaylistTrack("Fix You")];
        Signal<RxVoid> defaultManualSave = new();

        int defaultSaveCount = 0;
        IDisposable defaultSubscription = otherPlaylist.AutoPersistCollection(
            _ =>
            {
                defaultSaveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            defaultManualSave);

        defaultManualSave.OnNext(RxVoid.Default);

        // Disposed straight away: the manual signal only starts the same three-second timer, which never elapses.
        defaultSubscription.Dispose();

        Console.WriteLine(timedSaveCount);
        Console.WriteLine(defaultSaveCount);

        // Output:
        // 1
        // 0
    }

    /// <summary>
    /// <c>AutoPersistCollection</c> works the same way on a <see cref="ReadOnlyObservableCollection{T}"/>: it watches
    /// the read-only view, but new tracks still have to arrive through the writable source collection behind it.
    /// </summary>
    /// <returns>A task that completes once the new track has saved.</returns>
    public static async Task ReadOnlyCollectionManualSaveOverloads()
    {
        ObservableCollection<PlaylistTrack> source = [];
        ReadOnlyObservableCollection<PlaylistTrack> playlist = new(source);
        Signal<RxVoid> manualSave = new();

        List<string> saved = [];
        using IDisposable subscription = playlist.AutoPersistCollection(
            track =>
            {
                saved.Add(track.Title);
                return Signal.Emit(RxVoid.Default);
            },
            manualSave,
            TimeSpan.FromMilliseconds(30));

        PlaylistTrack chores = new("Adventure of a Lifetime");
        source.Add(chores);

        // Only a change after the track joins the collection requests a save.
        chores.IsFavorite = true;
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        ReadOnlyObservableCollection<PlaylistTrack> otherPlaylist = new([new PlaylistTrack("Africa")]);
        Signal<RxVoid> otherManualSave = new();

        int otherSaveCount = 0;
        IDisposable otherSubscription = otherPlaylist.AutoPersistCollection(
            _ =>
            {
                otherSaveCount++;
                return Signal.Emit(RxVoid.Default);
            },
            otherManualSave);

        otherManualSave.OnNext(RxVoid.Default);

        // Disposed straight away: the manual signal only starts the same three-second timer, which never elapses.
        otherSubscription.Dispose();

        Console.WriteLine(string.Join(", ", saved));
        Console.WriteLine(otherSaveCount);

        // Output:
        // Adventure of a Lifetime
        // 0
    }
}
