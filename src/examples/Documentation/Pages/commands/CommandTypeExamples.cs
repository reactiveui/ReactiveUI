// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// Shows the command class hierarchy itself: the constructors <see cref="ReactiveCommand{TParam, TResult}"/> and
/// <see cref="CombinedReactiveCommand{TParam, TResult}"/> expose to a subclass, the members
/// <see cref="ReactiveCommandBase{TParam, TResult}"/> asks a subclass to implement, and the
/// <see cref="IReactiveCommand"/> interfaces a view model or method can depend on instead of a concrete command type.
/// </summary>
public static class CommandTypeExamples
{
    /// <summary>
    /// Write your own command type from <c>ReactiveCommand&lt;TParam, TResult&gt;</c>'s first constructor when every
    /// command of a kind needs the same behaviour around each run; <see cref="RecordingCommand{TParam, TResult}"/>
    /// keeps every borrowed book id for an audit trail, which no <c>ReactiveCommand.Create</c> factory offers.
    /// </summary>
    /// <returns>A task that completes once every book has been borrowed.</returns>
    public static async Task DeriveWithResultObservable()
    {
        LibraryDesk desk = new();

        using RecordingCommand<int, Book> borrow = new(
            bookId => Signal.Emit(desk.Borrow(bookId)),
            canExecute: null,
            outputScheduler: Sequencer.Immediate);

        Book first = await borrow.Execute(1);
        Book second = await borrow.Execute(2);

        Console.WriteLine(first.Title);
        Console.WriteLine(second.Title);
        Console.WriteLine(string.Join(", ", borrow.History));

        // Output:
        // Clean Code
        // The Pragmatic Programmer
        // 1, 2
    }

    /// <summary>
    /// Write your own command type from the cancel-callback constructor when every command of a kind must cancel
    /// through a callback instead of by unsubscribing; here a print job must tell the printer to stop, and
    /// <see cref="RecordingCommand{TParam, TResult}"/> also keeps every book id it printed a receipt for.
    /// </summary>
    /// <returns>A task that completes once the cancelled print job has told the printer to stop.</returns>
    public static async Task DeriveWithCancelCallback()
    {
        bool printerToldToStop = false;
        ScheduledSignal<string> printerOutput = new(Sequencer.Immediate);

        using RecordingCommand<int, string> printReceipt = new(
            bookId => Signal.Emit<(IObservable<string> Result, Action Cancel)>((printerOutput, () => printerToldToStop = true)),
            canExecute: null,
            outputScheduler: Sequencer.Immediate);

        IDisposable execution = printReceipt.Execute(1).Subscribe(static _ => { });
        execution.Dispose();

        Console.WriteLine(printerToldToStop);
        Console.WriteLine(string.Join(", ", printReceipt.History));

        // Output:
        // True
        // 1
    }

    /// <summary>A failed execution's exception reaches <c>ThrownExceptions</c> even when nothing awaits <c>Execute</c>.</summary>
    /// <returns>A task that completes once the failed borrow has been reported.</returns>
    public static async Task ObserveThrownExceptions()
    {
        LibraryDesk desk = new();
        List<string> errors = [];

        using ReactiveCommand<int, Book> borrow = ReactiveCommand.CreateFromObservable<int, Book>(
            bookId => bookId == 99 ? Signal.Fail<Book>(new InvalidOperationException("No such book.")) : Signal.Emit(desk.Borrow(bookId)),
            outputScheduler: Sequencer.Immediate);
        using IDisposable subscription = borrow.ThrownExceptions.Subscribe(error => errors.Add(error.Message));

        try
        {
            _ = await borrow.Execute(99);
        }
        catch (InvalidOperationException)
        {
            // The awaiting caller sees the error too; ThrownExceptions lets other parts of the view model react as well.
        }

        Console.WriteLine(errors[0]);

        // Output:
        // No such book.
    }

    /// <summary>
    /// A command built directly on <see cref="ReactiveCommandBase{TParam, TResult}"/> still works as an
    /// <see cref="System.Windows.Input.ICommand"/>: binding code calls <c>CanExecute</c> and <c>Execute</c> through
    /// that interface, which route to the protected hooks the base class defines for exactly this purpose.
    /// </summary>
    /// <returns>A task that completes once the announcement has been made.</returns>
    public static async Task DeriveFromCommandBase()
    {
        LibraryDesk desk = new();
        List<string> canExecuteChanges = [];
        using DeskAnnouncementCommand closingSoon = new(desk, static () => "The desk closes in ten minutes.");
        using IDisposable subscription = closingSoon.Subscribe(Console.WriteLine);

        System.Windows.Input.ICommand asCommand = closingSoon;
        asCommand.CanExecuteChanged += (_, _) => canExecuteChanges.Add(asCommand.CanExecute(null) ? "can announce" : "cannot announce");

        Console.WriteLine(asCommand.CanExecute(null));
        asCommand.Execute(null);
        await Task.Yield();

        desk.IsOpen = false;

        Console.WriteLine(string.Join(", ", canExecuteChanges));

        // Output:
        // True
        // The desk closes in ten minutes.
        // cannot announce
    }

    /// <summary>
    /// Write your own command type from <c>CombinedReactiveCommand&lt;TParam, TResult&gt;</c>'s three constructors
    /// when every combined command of a kind needs the same behaviour on each run;
    /// <see cref="LoggingCombinedCommand{TParam, TResult}"/> writes a line each time it runs, which
    /// <c>ReactiveCommand.CreateCombined</c> has no hook for.
    /// </summary>
    /// <returns>A task that completes once every combined command has run.</returns>
    public static async Task DeriveCombinedCommand()
    {
        LibraryDesk desk = new();
        _ = desk.Borrow(1);

        using ReactiveCommand<RxVoid, int> takeBackLoans = ReactiveCommand.Create(desk.ReturnAll);
        using ReactiveCommand<RxVoid, int> countShelf = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using LoggingCombinedCommand<RxVoid, int> withCanExecuteAndScheduler = new([takeBackLoans, countShelf], desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);
        IList<int> firstRun = await withCanExecuteAndScheduler.Execute();

        using ReactiveCommand<RxVoid, int> countShelfAgain = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using LoggingCombinedCommand<RxVoid, int> withCanExecute = new([countShelfAgain], desk.WhenAnyValue(d => d.IsOpen));
        IList<int> secondRun = await withCanExecute.Execute();

        using ReactiveCommand<RxVoid, int> countShelfOnceMore = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using LoggingCombinedCommand<RxVoid, int> withScheduler = new([countShelfOnceMore], Sequencer.Immediate);
        IList<int> thirdRun = await withScheduler.Execute();

        Console.WriteLine(firstRun[1]);
        Console.WriteLine(secondRun[0]);
        Console.WriteLine(thirdRun[0]);

        // Output:
        // Combined command running
        // Combined command running
        // Combined command running
        // 4
        // 4
        // 4
    }

    /// <summary>A view model exposes a command as <c>IReactiveCommand&lt;TParam, TResult&gt;</c>, hiding whether it is a plain or combined command.</summary>
    /// <returns>A task that completes once the search has run.</returns>
    public static async Task ExposeCommandThroughInterface()
    {
        LibraryDesk desk = new();
        DeskConsoleViewModel console = new()
        {
            SearchCommand = ReactiveCommand.Create<string, IReadOnlyList<Book>>(desk.Search),
        };

        IReactiveCommand<string, IReadOnlyList<Book>> search = console.SearchCommand!;
        IReadOnlyList<Book> matches = await search.Execute("Fowler");

        Console.WriteLine(matches[0].Title);

        // Output:
        // Refactoring
    }

    /// <summary>A method that only needs to know whether a command is busy or ready accepts the non-generic <c>IReactiveCommand</c>.</summary>
    /// <returns>A task that completes once both commands have run.</returns>
    public static async Task AcceptAnyCommand()
    {
        LibraryDesk desk = new();
        using ReactiveCommand<RxVoid, int> countShelf = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using CombinedReactiveCommand<RxVoid, int> endOfDay = ReactiveCommand.CreateCombined([countShelf]);

        _ = await countShelf.Execute();
        Console.WriteLine(await DescribeAsync(countShelf));

        _ = await endOfDay.Execute();
        Console.WriteLine(await DescribeAsync(endOfDay));

        // Output:
        // Not busy, ready
        // Not busy, ready
    }

    /// <summary>Describes whether any command reachable through <c>IReactiveCommand</c> is currently busy, and whether it is ready to run.</summary>
    /// <param name="command">The command to check.</param>
    /// <returns>A task producing a description of the command's current activity.</returns>
    private static async Task<string> DescribeAsync(IReactiveCommand command)
    {
        bool isExecuting = await command.IsExecuting.FirstAsync();
        bool canExecute = await command.CanExecute.FirstAsync();
        return $"{(isExecuting ? "Busy" : "Not busy")}, {(canExecute ? "ready" : "not ready")}";
    }
}
