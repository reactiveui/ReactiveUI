// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Advanced;
using ReactiveUI.Primitives.Concurrency;

namespace ReactiveUI.Documentation.Scheduling;

/// <summary>Shows <see cref="ScheduledSubject{T}"/>, a subject that delivers every notification through a chosen
/// sequencer; a music player's now-playing subject uses one so every listener hears updates the same way.</summary>
public static class ScheduledSubjectExamples
{
    /// <summary>A subject delivers on the sequencer it was built with; <see cref="Sequencer.Immediate"/> delivers in place, which is what a console host has instead of a UI thread.</summary>
    public static void DeliverOnAChosenSequencer()
    {
        using ScheduledSubject<NowPlayingTrack> nowPlaying = new(Sequencer.Immediate);
        List<NowPlayingTrack> played = [];

        Console.WriteLine(nowPlaying.HasObservers);

        IObserver<NowPlayingTrack> witness = Witness.Create<NowPlayingTrack>(played.Add);
        using IDisposable subscription = nowPlaying.Subscribe(witness);
        Console.WriteLine(nowPlaying.HasObservers);

        nowPlaying.OnNext(new NowPlayingTrack("Clocks", "Coldplay"));

        Console.WriteLine(played[0]);
        Console.WriteLine(nowPlaying.IsDisposed);

        // Output:
        // False
        // True
        // NowPlayingTrack { Title = Clocks, Artist = Coldplay }
        // False
    }

    /// <summary>A default observer keeps receiving values while no one else is listening, and takes over again the
    /// moment the last real listener leaves; a music player uses it to log tracks while no speaker is connected.</summary>
    public static void FallBackToADefaultObserverWhenNoOneIsListening()
    {
        List<NowPlayingTrack> offlineLog = [];
        IObserver<NowPlayingTrack> offlineWitness = Witness.Create<NowPlayingTrack>(offlineLog.Add);
        using ScheduledSubject<NowPlayingTrack> nowPlaying = new(Sequencer.Immediate, offlineWitness);

        nowPlaying.OnNext(new NowPlayingTrack("Paradise", "Coldplay"));
        Console.WriteLine(offlineLog.Count);

        List<NowPlayingTrack> speakerLog = [];
        using (IDisposable subscription = nowPlaying.Subscribe(speakerLog.Add))
        {
            nowPlaying.OnNext(new NowPlayingTrack("Adventure of a Lifetime", "Coldplay"));
        }

        nowPlaying.OnNext(new NowPlayingTrack("Hymn for the Weekend", "Coldplay"));

        Console.WriteLine(speakerLog.Count);
        Console.WriteLine(offlineLog.Count);

        // Output:
        // 1
        // 1
        // 2
    }

    /// <summary>Passing a signal of its own lets the subject replay history the way that signal does; a
    /// <c>BehaviorSignal</c> hands a joining listener the most recent track straight away.</summary>
    public static void ReplayTheLatestValueWithACustomSignal()
    {
        BehaviorSignal<NowPlayingTrack?> latest = new(null);
        using ScheduledSubject<NowPlayingTrack?> nowPlaying = new(Sequencer.Immediate, null, latest);

        nowPlaying.OnNext(new NowPlayingTrack("Sky Full of Stars", "Coldplay"));

        List<NowPlayingTrack?> log = [];
        using IDisposable subscription = nowPlaying.Subscribe(log.Add);

        Console.WriteLine(log[0]);

        // Output:
        // NowPlayingTrack { Title = Sky Full of Stars, Artist = Coldplay }
    }

    /// <summary>An error or a completion reaches every subscriber, the same way either would on a plain subject.</summary>
    public static void CompleteOrFailTheSubject()
    {
        List<string> log = [];

        using ScheduledSubject<NowPlayingTrack> nowPlaying = new(Sequencer.Immediate);
        using IDisposable errorSubscription = nowPlaying.Subscribe(
            static _ => { },
            error => log.Add($"Error: {error.Message}"),
            () => log.Add("Completed"));
        nowPlaying.OnError(new InvalidOperationException("Track unavailable"));

        using ScheduledSubject<NowPlayingTrack> playlist = new(Sequencer.Immediate);
        using IDisposable completedSubscription = playlist.Subscribe(
            static _ => { },
            static _ => { },
            () => log.Add("Playlist finished"));
        playlist.OnCompleted();

        Console.WriteLine(log[0]);
        Console.WriteLine(log[1]);

        // Output:
        // Error: Track unavailable
        // Playlist finished
    }

    /// <summary>A subclass overrides the protected <c>Dispose(bool)</c> to run its own cleanup alongside the base subject's.</summary>
    public static void LogWhenTheSubjectIsDisposed()
    {
        List<string> disposalLog = [];

        using (LoggingScheduledSubject<NowPlayingTrack> nowPlaying = new(Sequencer.Immediate, disposalLog))
        {
            nowPlaying.OnNext(new NowPlayingTrack("Speed of Sound", "Coldplay"));
        }

        Console.WriteLine(disposalLog.Count);

        // Output:
        // 1
    }
}
