// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// Shows <c>SwitchSubscribe</c> and <c>SwitchSelect</c>: a subscription that follows a view model property as it is
/// replaced, such as a clerk console's search command, active session or progress stream, rather than staying
/// attached to whatever the property held at subscribe time.
/// </summary>
public static class SwitchSubscribeExamples
{
    /// <summary>A property that is itself an observable can be swapped directly; <c>SwitchSubscribe</c> follows it.</summary>
    public static void TrackSwappedProgressStream()
    {
        DeskConsoleViewModel console = new();
        List<int> matchCounts = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.Progress).SwitchSubscribe(matchCounts.Add);

        ScheduledSignal<int> firstSearch = new(Sequencer.Immediate);
        console.Progress = firstSearch;
        firstSearch.OnNext(1);
        firstSearch.OnNext(2);

        ScheduledSignal<int> secondSearch = new(Sequencer.Immediate);
        console.Progress = secondSearch;
        firstSearch.OnNext(3);
        secondSearch.OnNext(5);

        Console.WriteLine(string.Join(", ", matchCounts));

        // Output:
        // 1, 2, 5
    }

    /// <summary>The error and completion handlers overload reports a stream failing, such as a search losing its connection.</summary>
    public static void TrackSwappedProgressStreamWithHandlers()
    {
        DeskConsoleViewModel console = new();
        List<int> matchCounts = [];
        List<string> notes = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.Progress).SwitchSubscribe(
            matchCounts.Add,
            error => notes.Add($"Search failed: {error.Message}"),
            () => notes.Add("Search completed"));

        ScheduledSignal<int> search = new(Sequencer.Immediate);
        console.Progress = search;
        search.OnNext(3);
        search.OnError(new InvalidOperationException("Catalogue offline"));

        Console.WriteLine(string.Join(", ", matchCounts));
        Console.WriteLine(notes[0]);

        // Output:
        // 3
        // Search failed: Catalogue offline
    }

    /// <summary>A selector projects a swapped property to the observable that matters, here a session's match count.</summary>
    public static void TrackSwappedSessionProgress()
    {
        DeskConsoleViewModel console = new();
        List<int> matchCounts = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.ActiveSession)
            .SwitchSubscribe(static session => session.MatchCount, matchCounts.Add);

        ScheduledSignal<int> firstCount = new(Sequencer.Immediate);
        console.ActiveSession = new SearchSession(firstCount);
        firstCount.OnNext(1);

        ScheduledSignal<int> secondCount = new(Sequencer.Immediate);
        console.ActiveSession = new SearchSession(secondCount);
        firstCount.OnNext(9);
        secondCount.OnNext(4);

        Console.WriteLine(string.Join(", ", matchCounts));

        // Output:
        // 1, 4
    }

    /// <summary>The selector overload also takes error and completion handlers, reported once for each session that ends.</summary>
    public static void TrackSwappedSessionProgressWithHandlers()
    {
        DeskConsoleViewModel console = new();
        List<int> matchCounts = [];
        List<string> notes = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.ActiveSession).SwitchSubscribe(
            static session => session.MatchCount,
            matchCounts.Add,
            error => notes.Add($"Search failed: {error.Message}"),
            () => notes.Add("Search completed"));

        ScheduledSignal<int> count = new(Sequencer.Immediate);
        console.ActiveSession = new SearchSession(count);
        count.OnNext(2);
        count.OnError(new InvalidOperationException("Catalogue offline"));

        Console.WriteLine(string.Join(", ", matchCounts));
        Console.WriteLine(notes[0]);

        // Output:
        // 2
        // Search failed: Catalogue offline
    }

    /// <summary><c>SwitchSelect</c> projects a swapped property to a plain observable, for use with <c>Subscribe</c> or <c>ToProperty</c>.</summary>
    public static void SelectSwappedSessionProgress()
    {
        DeskConsoleViewModel console = new();
        List<int> matchCounts = [];

        IObservable<int> matchCount = console.WhenAnyValue(v => v.ActiveSession).SwitchSelect(static session => session.MatchCount);
        using IDisposable subscription = matchCount.Subscribe(matchCounts.Add);

        ScheduledSignal<int> count = new(Sequencer.Immediate);
        console.ActiveSession = new SearchSession(count);
        count.OnNext(7);

        Console.WriteLine(string.Join(", ", matchCounts));

        // Output:
        // 7
    }

    /// <summary>Subscribing to a command property follows results across every command the clerk swaps in, not just the first.</summary>
    /// <returns>A task that completes once both searches have run.</returns>
    public static async Task TrackSwappedCommandResults()
    {
        LibraryDesk desk = new();
        DeskConsoleViewModel console = new();
        List<string> firstTitles = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.SearchCommand)
            .SwitchSubscribe(matches => firstTitles.Add(matches[0].Title));

        using ReactiveCommand<string, IReadOnlyList<Book>> byTitle = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, Sequencer.Immediate);
        console.SearchCommand = byTitle;
        _ = await byTitle.Execute("Fowler");

        using ReactiveCommand<string, IReadOnlyList<Book>> byAuthor = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, Sequencer.Immediate);
        console.SearchCommand = byAuthor;
        _ = await byAuthor.Execute("Gamma");

        Console.WriteLine(string.Join(", ", firstTitles));

        // Output:
        // Refactoring, Design Patterns
    }

    /// <summary>The command-results overload also takes error and completion handlers, for a command type that can raise them.</summary>
    /// <returns>A task that completes once the search has run.</returns>
    public static async Task TrackSwappedCommandResultsWithHandlers()
    {
        LibraryDesk desk = new();
        DeskConsoleViewModel console = new();
        List<string> titles = [];
        List<string> notes = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.SearchCommand).SwitchSubscribe(
            matches => titles.Add(matches[0].Title),
            error => notes.Add($"Search failed: {error.Message}"),
            () => notes.Add("Command replaced"));

        using ReactiveCommand<string, IReadOnlyList<Book>> search = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, Sequencer.Immediate);
        console.SearchCommand = search;
        _ = await search.Execute("Martin");

        Console.WriteLine(string.Join(", ", titles));

        // Output:
        // Clean Code
    }

    /// <summary>A selector picks one of the swapped command's observables, here <c>IsExecuting</c>, to drive a busy indicator.</summary>
    /// <returns>A task that completes once the search has run.</returns>
    public static async Task TrackSwappedCommandIsExecuting()
    {
        LibraryDesk desk = new();
        DeskConsoleViewModel console = new();
        List<bool> busyChanges = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.SearchCommand)
            .SwitchSubscribe(static cmd => cmd.IsExecuting, busyChanges.Add);

        using ReactiveCommand<string, IReadOnlyList<Book>> search = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, Sequencer.Immediate);
        console.SearchCommand = search;
        _ = await search.Execute("Hunt");

        Console.WriteLine(string.Join(", ", busyChanges));

        // Output:
        // False, True, False
    }

    /// <summary>The command-selector overload also takes error and completion handlers.</summary>
    /// <returns>A task that completes once the search has run.</returns>
    public static async Task TrackSwappedCommandIsExecutingWithHandlers()
    {
        LibraryDesk desk = new();
        DeskConsoleViewModel console = new();
        List<bool> busyChanges = [];
        List<string> notes = [];

        using IDisposable subscription = console.WhenAnyValue(v => v.SearchCommand).SwitchSubscribe(
            static cmd => cmd.IsExecuting,
            busyChanges.Add,
            error => notes.Add($"Search failed: {error.Message}"),
            () => notes.Add("Command replaced"));

        using ReactiveCommand<string, IReadOnlyList<Book>> search = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, Sequencer.Immediate);
        console.SearchCommand = search;
        _ = await search.Execute("Gamma");

        Console.WriteLine(string.Join(", ", busyChanges));

        // Output:
        // False, True, False
    }

    /// <summary><c>SwitchSelect</c> on a command property is the building block <c>SwitchSubscribe</c> uses, for use with <c>Subscribe</c> or <c>ToProperty</c>.</summary>
    /// <returns>A task that completes once the search has run.</returns>
    public static async Task SelectSwappedCommandIsExecuting()
    {
        LibraryDesk desk = new();
        DeskConsoleViewModel console = new();
        List<bool> busyChanges = [];

        IObservable<bool> isBusy = console.WhenAnyValue(v => v.SearchCommand).SwitchSelect(static cmd => cmd.IsExecuting);
        using IDisposable subscription = isBusy.Subscribe(busyChanges.Add);

        using ReactiveCommand<string, IReadOnlyList<Book>> search = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, Sequencer.Immediate);
        console.SearchCommand = search;
        _ = await search.Execute("Hunt");

        Console.WriteLine(string.Join(", ", busyChanges));

        // Output:
        // False, True, False
    }
}
