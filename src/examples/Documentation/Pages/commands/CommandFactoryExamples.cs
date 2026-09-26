// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>Shows every synchronous <c>Create</c> overload, the observable-based factories, and <c>CreateCombined</c>.</summary>
public static class CommandFactoryExamples
{
    /// <summary><c>Create(Action, ...)</c> has four overloads: the action alone, a <c>canExecute</c> observable, an output <see cref="ISequencer"/>, or both.</summary>
    /// <returns>A task that completes once every command has logged its event.</returns>
    public static async Task LogDeskEvents()
    {
        LibraryDesk desk = new();
        List<string> log = [];

        using ReactiveCommand<RxVoid, RxVoid> open = ReactiveCommand.Create(() => log.Add("Desk opened"));
        using ReactiveCommand<RxVoid, RxVoid> lendBook = ReactiveCommand.Create(() => log.Add("Book lent"), desk.WhenAnyValue(d => d.IsOpen));

        // The last argument is the output ISequencer: it picks where the command delivers its result, IsExecuting
        // and ThrownExceptions. Left out, a command uses RxSchedulers.MainThreadScheduler, the UI thread in an app.
        using ReactiveCommand<RxVoid, RxVoid> tidyShelf = ReactiveCommand.Create(() => log.Add("Shelf tidied"), Sequencer.Immediate);
        using ReactiveCommand<RxVoid, RxVoid> waiveFee = ReactiveCommand.Create(() => log.Add("Late fee waived"), desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        _ = await open.Execute();
        _ = await lendBook.Execute();
        _ = await tidyShelf.Execute();
        _ = await waiveFee.Execute();

        desk.IsOpen = false;

        foreach (string entry in log)
        {
            Console.WriteLine(entry);
        }

        Console.WriteLine(await lendBook.CanExecute.FirstAsync());

        // Output:
        // Desk opened
        // Book lent
        // Shelf tidied
        // Late fee waived
        // False
    }

    /// <summary><c>Create&lt;TResult&gt;(Func&lt;TResult&gt;, ...)</c> has the same four overloads, but returns a value.</summary>
    /// <returns>A task that completes once every count has been read.</returns>
    public static async Task CountBooksOnShelf()
    {
        LibraryDesk desk = new();
        _ = desk.Borrow(1);

        using ReactiveCommand<RxVoid, int> countAll = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using ReactiveCommand<RxVoid, int> countLoans = ReactiveCommand.Create(() => desk.LoanCount, desk.WhenAnyValue(d => d.IsOpen));

        // The ISequencer argument picks where the result is delivered; omitted, it defaults to RxSchedulers.MainThreadScheduler.
        using ReactiveCommand<RxVoid, int> countByTitle = ReactiveCommand.Create(() => desk.Search("Code").Count, Sequencer.Immediate);
        using ReactiveCommand<RxVoid, int> countLoansImmediate = ReactiveCommand.Create(() => desk.LoanCount, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        Console.WriteLine(await countAll.Execute());
        Console.WriteLine(await countLoans.Execute());
        Console.WriteLine(await countByTitle.Execute());
        Console.WriteLine(await countLoansImmediate.Execute());

        // Output:
        // 4
        // 1
        // 1
        // 1
    }

    /// <summary><c>Create&lt;TParam&gt;(Action&lt;TParam&gt;, ...)</c> passes a book id through to the action.</summary>
    /// <returns>A task that completes once every book has been flagged.</returns>
    public static async Task FlagOverdueBooks()
    {
        LibraryDesk desk = new();
        List<int> overdueNotices = [];

        using ReactiveCommand<int, RxVoid> flag = ReactiveCommand.Create<int>(overdueNotices.Add);
        using ReactiveCommand<int, RxVoid> flagWhileOpen = ReactiveCommand.Create<int>(overdueNotices.Add, desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<int, RxVoid> flagImmediate = ReactiveCommand.Create<int>(overdueNotices.Add, Sequencer.Immediate);
        using ReactiveCommand<int, RxVoid> flagGuarded = ReactiveCommand.Create<int>(overdueNotices.Add, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        _ = await flag.Execute(1);
        _ = await flagWhileOpen.Execute(2);
        _ = await flagImmediate.Execute(3);
        _ = await flagGuarded.Execute(4);

        Console.WriteLine(string.Join(", ", overdueNotices));

        // Output:
        // 1, 2, 3, 4
    }

    /// <summary><c>Create&lt;TParam, TResult&gt;(Func&lt;TParam, TResult&gt;, ...)</c> takes a search term and returns matches.</summary>
    /// <returns>A task that completes once every search has run.</returns>
    public static async Task SearchCatalogue()
    {
        LibraryDesk desk = new();

        using ReactiveCommand<string, IReadOnlyList<Book>> search = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search);
        using ReactiveCommand<string, IReadOnlyList<Book>> searchWhileOpen = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, desk.WhenAnyValue(d => d.IsOpen));

        // The ISequencer argument picks where the search result is delivered; omitted, it defaults to RxSchedulers.MainThreadScheduler.
        using ReactiveCommand<string, IReadOnlyList<Book>> searchImmediate = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, Sequencer.Immediate);
        using ReactiveCommand<string, IReadOnlyList<Book>> searchGuarded = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        IReadOnlyList<Book> byTitle = await search.Execute("Refactoring");
        IReadOnlyList<Book> byAuthor = await searchWhileOpen.Execute("Fowler");
        IReadOnlyList<Book> immediate = await searchImmediate.Execute("Pragmatic");
        IReadOnlyList<Book> guarded = await searchGuarded.Execute("Gamma");

        Console.WriteLine(byTitle[0].Title);
        Console.WriteLine(byAuthor[0].Title);
        Console.WriteLine(immediate[0].Title);
        Console.WriteLine(guarded[0].Title);

        // Output:
        // Refactoring
        // Refactoring
        // The Pragmatic Programmer
        // Design Patterns
    }

    /// <summary>
    /// <c>CreateFromObservable&lt;TResult&gt;(Func&lt;IObservable&lt;TResult&gt;&gt;, ...)</c> wraps an asynchronous
    /// download of the central catalogue as an observable, using <c>Signal.FromAsync</c>.
    /// </summary>
    /// <returns>A task that completes once every sync has finished.</returns>
    public static async Task SyncCatalogueAsObservable()
    {
        LibraryDesk desk = new();

        using ReactiveCommand<RxVoid, int> sync = ReactiveCommand.CreateFromObservable(() => Signal.FromAsync(desk.SyncWithCentralCatalogueAsync));
        using ReactiveCommand<RxVoid, int> syncWhileOpen = ReactiveCommand.CreateFromObservable(() => Signal.FromAsync(desk.SyncWithCentralCatalogueAsync), desk.WhenAnyValue(d => d.IsOpen));

        // The ISequencer argument picks where the synced count is delivered; omitted, it defaults to RxSchedulers.MainThreadScheduler.
        using ReactiveCommand<RxVoid, int> syncImmediate = ReactiveCommand.CreateFromObservable(() => Signal.FromAsync(desk.SyncWithCentralCatalogueAsync), Sequencer.Immediate);
        using ReactiveCommand<RxVoid, int> syncGuarded = ReactiveCommand.CreateFromObservable(
            () => Signal.FromAsync(desk.SyncWithCentralCatalogueAsync),
            desk.WhenAnyValue(d => d.IsOpen),
            Sequencer.Immediate);

        Console.WriteLine(await sync.Execute());
        Console.WriteLine(await syncWhileOpen.Execute());
        Console.WriteLine(await syncImmediate.Execute());
        Console.WriteLine(await syncGuarded.Execute());

        // Output:
        // 4
        // 4
        // 4
        // 4
    }

    /// <summary>
    /// <c>CreateFromObservable&lt;TParam, TResult&gt;(Func&lt;TParam, IObservable&lt;TResult&gt;&gt;, ...)</c> borrows a book
    /// by id, wrapping the synchronous lookup as a single-value observable with <c>Signal.Emit</c>.
    /// </summary>
    /// <returns>A task that completes once every book has been lent.</returns>
    public static async Task BorrowBookAsObservable()
    {
        LibraryDesk desk = new();

        using ReactiveCommand<int, Book> borrow = ReactiveCommand.CreateFromObservable<int, Book>(bookId => Signal.Emit(desk.Borrow(bookId)));
        using ReactiveCommand<int, Book> borrowWhileOpen = ReactiveCommand.CreateFromObservable<int, Book>(bookId => Signal.Emit(desk.Borrow(bookId)), desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<int, Book> borrowImmediate = ReactiveCommand.CreateFromObservable<int, Book>(bookId => Signal.Emit(desk.Borrow(bookId)), Sequencer.Immediate);
        using ReactiveCommand<int, Book> borrowGuarded = ReactiveCommand.CreateFromObservable<int, Book>(
            bookId => Signal.Emit(desk.Borrow(bookId)),
            desk.WhenAnyValue(d => d.IsOpen),
            Sequencer.Immediate);

        Book first = await borrow.Execute(1);
        Book second = await borrowWhileOpen.Execute(2);
        Book third = await borrowImmediate.Execute(3);
        Book fourth = await borrowGuarded.Execute(4);

        Console.WriteLine(first.Title);
        Console.WriteLine(second.Title);
        Console.WriteLine(third.Title);
        Console.WriteLine(fourth.Title);
        Console.WriteLine(desk.LoanCount);

        // Output:
        // Clean Code
        // The Pragmatic Programmer
        // Design Patterns
        // Refactoring
        // 4
    }

    /// <summary><c>CreateCombined</c> runs every child command together and collects their results into a list.</summary>
    /// <returns>A task that completes once every combined command has run.</returns>
    public static async Task CombineEndOfDayTasks()
    {
        LibraryDesk desk = new();
        _ = desk.Borrow(1);
        _ = desk.Borrow(2);

        using ReactiveCommand<RxVoid, int> takeBackLoans = ReactiveCommand.Create(desk.ReturnAll);
        using ReactiveCommand<RxVoid, int> countShelf = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using CombinedReactiveCommand<RxVoid, int> endOfDay = ReactiveCommand.CreateCombined([takeBackLoans, countShelf]);
        IList<int> firstRun = await endOfDay.Execute();

        using ReactiveCommand<RxVoid, int> countShelfWhileOpen = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using CombinedReactiveCommand<RxVoid, int> endOfDayWhileOpen = ReactiveCommand.CreateCombined([countShelfWhileOpen], desk.WhenAnyValue(d => d.IsOpen));
        IList<int> secondRun = await endOfDayWhileOpen.Execute();

        // The ISequencer argument picks where the combined list is delivered; omitted, it defaults to RxSchedulers.MainThreadScheduler.
        using ReactiveCommand<RxVoid, int> countShelfImmediate = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using CombinedReactiveCommand<RxVoid, int> endOfDayImmediate = ReactiveCommand.CreateCombined([countShelfImmediate], Sequencer.Immediate);
        IList<int> thirdRun = await endOfDayImmediate.Execute();

        using ReactiveCommand<RxVoid, int> countShelfGuarded = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using CombinedReactiveCommand<RxVoid, int> endOfDayGuarded = ReactiveCommand.CreateCombined([countShelfGuarded], desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);
        IList<int> fourthRun = await endOfDayGuarded.Execute();

        Console.WriteLine(firstRun[0]);
        Console.WriteLine(firstRun[1]);
        Console.WriteLine(secondRun[0]);
        Console.WriteLine(thirdRun[0]);
        Console.WriteLine(fourthRun[0]);

        // Output:
        // 2
        // 4
        // 4
        // 4
        // 4
    }
}
