// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>Shows every <c>CreateFromTask</c> overload: with and without a parameter, a result, and a <see cref="CancellationToken"/>.</summary>
public static class CommandFactoryTaskExamples
{
    /// <summary><c>CreateFromTask(Func&lt;Task&gt;, ...)</c>: no parameter, no result.</summary>
    /// <returns>A task that completes once every closing has run.</returns>
    public static async Task ReturnAllBooksTask()
    {
        LibraryDesk desk = new();

        _ = desk.Borrow(1);
        _ = desk.Borrow(2);
        using ReactiveCommand<RxVoid, RxVoid> returnAll = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksAsync);
        _ = await returnAll.Execute();
        Console.WriteLine(desk.LoanCount);

        _ = desk.Borrow(3);
        using ReactiveCommand<RxVoid, RxVoid> returnAllWhileOpen = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksAsync, desk.WhenAnyValue(d => d.IsOpen));
        _ = await returnAllWhileOpen.Execute();
        Console.WriteLine(desk.LoanCount);

        // The ISequencer argument picks where completion is delivered; omitted, it defaults to RxSchedulers.MainThreadScheduler.
        _ = desk.Borrow(4);
        using ReactiveCommand<RxVoid, RxVoid> returnAllImmediate = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksAsync, Sequencer.Immediate);
        _ = await returnAllImmediate.Execute();
        Console.WriteLine(desk.LoanCount);

        _ = desk.Borrow(1);
        using ReactiveCommand<RxVoid, RxVoid> returnAllGuarded = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksAsync, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);
        _ = await returnAllGuarded.Execute();
        Console.WriteLine(desk.LoanCount);

        // Output:
        // 0
        // 0
        // 0
        // 0
    }

    /// <summary><c>CreateFromTask&lt;TResult&gt;(Func&lt;Task&lt;TResult&gt;&gt;, ...)</c>: no parameter, a result.</summary>
    /// <returns>A task that completes once every closing has run.</returns>
    public static async Task ReturnAllBooksWithCountTask()
    {
        LibraryDesk desk = new();

        _ = desk.Borrow(1);
        _ = desk.Borrow(2);
        using ReactiveCommand<RxVoid, int> returnAll = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksWithCountAsync);
        Console.WriteLine(await returnAll.Execute());

        _ = desk.Borrow(3);
        using ReactiveCommand<RxVoid, int> returnAllWhileOpen = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksWithCountAsync, desk.WhenAnyValue(d => d.IsOpen));
        Console.WriteLine(await returnAllWhileOpen.Execute());

        using ReactiveCommand<RxVoid, int> returnAllImmediate = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksWithCountAsync, Sequencer.Immediate);
        Console.WriteLine(await returnAllImmediate.Execute());

        _ = desk.Borrow(1);
        using ReactiveCommand<RxVoid, int> returnAllGuarded = ReactiveCommand.CreateFromTask(desk.ReturnAllBooksWithCountAsync, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);
        Console.WriteLine(await returnAllGuarded.Execute());

        // Output:
        // 2
        // 1
        // 0
        // 1
    }

    /// <summary><c>CreateFromTask(Func&lt;CancellationToken, Task&gt;, ...)</c>: no parameter, no result, cancellable.</summary>
    /// <returns>A task that completes once every refresh has run.</returns>
    public static async Task RefreshLocalCacheTask()
    {
        LibraryDesk desk = new();

        using ReactiveCommand<RxVoid, RxVoid> refresh = ReactiveCommand.CreateFromTask(desk.RefreshLocalCacheAsync);
        using ReactiveCommand<RxVoid, RxVoid> refreshWhileOpen = ReactiveCommand.CreateFromTask(desk.RefreshLocalCacheAsync, desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<RxVoid, RxVoid> refreshImmediate = ReactiveCommand.CreateFromTask(desk.RefreshLocalCacheAsync, Sequencer.Immediate);
        using ReactiveCommand<RxVoid, RxVoid> refreshGuarded = ReactiveCommand.CreateFromTask(desk.RefreshLocalCacheAsync, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        _ = await refresh.Execute();
        _ = await refreshWhileOpen.Execute();
        _ = await refreshImmediate.Execute();
        _ = await refreshGuarded.Execute();

        Console.WriteLine("Local cache refreshed 4 times");

        // Output:
        // Local cache refreshed 4 times
    }

    /// <summary>
    /// <c>CreateFromTask&lt;TResult&gt;(Func&lt;CancellationToken, Task&lt;TResult&gt;&gt;, ...)</c>: no parameter, a result,
    /// cancellable. Disposing an execution cancels the token the sync was given, which is why this overload exists
    /// for a download that can take a while.
    /// </summary>
    /// <returns>A task that completes once the sync has finished and a cancelled one has stopped.</returns>
    public static async Task SyncCentralCatalogueTask()
    {
        LibraryDesk desk = new();

        using ReactiveCommand<RxVoid, int> sync = ReactiveCommand.CreateFromTask(desk.SyncWithCentralCatalogueAsync);
        using ReactiveCommand<RxVoid, int> syncWhileOpen = ReactiveCommand.CreateFromTask(desk.SyncWithCentralCatalogueAsync, desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<RxVoid, int> syncImmediate = ReactiveCommand.CreateFromTask(desk.SyncWithCentralCatalogueAsync, Sequencer.Immediate);
        using ReactiveCommand<RxVoid, int> syncGuarded = ReactiveCommand.CreateFromTask(desk.SyncWithCentralCatalogueAsync, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        Console.WriteLine(await sync.Execute());
        Console.WriteLine(await syncWhileOpen.Execute());
        Console.WriteLine(await syncImmediate.Execute());
        Console.WriteLine(await syncGuarded.Execute());

        Task<bool> stopped = sync.IsExecuting.Where(static executing => !executing).Skip(1).FirstAsync().ToTask();
        IDisposable execution = sync.Execute().Subscribe(static _ => { }, static _ => { });
        execution.Dispose();
        _ = await stopped;

        Console.WriteLine("Sync cancelled");

        // Output:
        // 4
        // 4
        // 4
        // 4
        // Sync cancelled
    }

    /// <summary><c>CreateFromTask&lt;TParam&gt;(Func&lt;TParam, Task&gt;, ...)</c>: a parameter, no result.</summary>
    /// <returns>A task that completes once every receipt has been sent.</returns>
    public static async Task SendRenewalReceiptTask()
    {
        LibraryDesk desk = new();

        Func<int, Task> sendReceipt = desk.SendRenewalReceiptAsync;
        using ReactiveCommand<int, RxVoid> receipt = ReactiveCommand.CreateFromTask(sendReceipt);
        using ReactiveCommand<int, RxVoid> receiptWhileOpen = ReactiveCommand.CreateFromTask(sendReceipt, desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<int, RxVoid> receiptImmediate = ReactiveCommand.CreateFromTask(sendReceipt, Sequencer.Immediate);
        using ReactiveCommand<int, RxVoid> receiptGuarded = ReactiveCommand.CreateFromTask(sendReceipt, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        _ = await receipt.Execute(1);
        _ = await receiptWhileOpen.Execute(2);
        _ = await receiptImmediate.Execute(3);
        _ = await receiptGuarded.Execute(4);

        Console.WriteLine(string.Join(", ", desk.RenewalReceipts));

        // Output:
        // 1, 2, 3, 4
    }

    /// <summary><c>CreateFromTask&lt;TParam, TResult&gt;(Func&lt;TParam, Task&lt;TResult&gt;&gt;, ...)</c>: a parameter, a result.</summary>
    /// <returns>A task that completes once every renewal has run.</returns>
    public static async Task RenewLoanTask()
    {
        LibraryDesk desk = new();
        _ = desk.Borrow(1);

        using ReactiveCommand<int, DateTimeOffset> renew = ReactiveCommand.CreateFromTask<int, DateTimeOffset>(desk.RenewLoanAsync);
        using ReactiveCommand<int, DateTimeOffset> renewWhileOpen = ReactiveCommand.CreateFromTask<int, DateTimeOffset>(desk.RenewLoanAsync, desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<int, DateTimeOffset> renewImmediate = ReactiveCommand.CreateFromTask<int, DateTimeOffset>(desk.RenewLoanAsync, Sequencer.Immediate);
        using ReactiveCommand<int, DateTimeOffset> renewGuarded = ReactiveCommand.CreateFromTask<int, DateTimeOffset>(desk.RenewLoanAsync, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        // Every renewal extends the same loan by 14 more days, so the gap between each due date stays 14 days no
        // matter how long the renewal itself took to run.
        DateTimeOffset firstRenewal = await renew.Execute(1);
        DateTimeOffset secondRenewal = await renewWhileOpen.Execute(1);
        DateTimeOffset thirdRenewal = await renewImmediate.Execute(1);
        DateTimeOffset fourthRenewal = await renewGuarded.Execute(1);

        Console.WriteLine((secondRenewal - firstRenewal).Days);
        Console.WriteLine((thirdRenewal - secondRenewal).Days);
        Console.WriteLine((fourthRenewal - thirdRenewal).Days);

        // Output:
        // 14
        // 14
        // 14
    }

    /// <summary><c>CreateFromTask&lt;TParam&gt;(Func&lt;TParam, CancellationToken, Task&gt;, ...)</c>: a parameter, no result, cancellable.</summary>
    /// <returns>A task that completes once every receipt has been sent.</returns>
    public static async Task SendRenewalReceiptCancellableTask()
    {
        LibraryDesk desk = new();

        Func<int, CancellationToken, Task> sendReceipt = desk.SendRenewalReceiptAsync;
        using ReactiveCommand<int, RxVoid> receipt = ReactiveCommand.CreateFromTask(sendReceipt);
        using ReactiveCommand<int, RxVoid> receiptWhileOpen = ReactiveCommand.CreateFromTask(sendReceipt, desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<int, RxVoid> receiptImmediate = ReactiveCommand.CreateFromTask(sendReceipt, Sequencer.Immediate);
        using ReactiveCommand<int, RxVoid> receiptGuarded = ReactiveCommand.CreateFromTask(sendReceipt, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

        _ = await receipt.Execute(1);
        _ = await receiptWhileOpen.Execute(2);
        _ = await receiptImmediate.Execute(3);
        _ = await receiptGuarded.Execute(4);

        Console.WriteLine(string.Join(", ", desk.RenewalReceipts));

        // Output:
        // 1, 2, 3, 4
    }

    /// <summary><c>CreateFromTask&lt;TParam, TResult&gt;(Func&lt;TParam, CancellationToken, Task&lt;TResult&gt;&gt;, ...)</c>: a parameter, a result, cancellable.</summary>
    /// <returns>A task that completes once every book has been lent.</returns>
    public static async Task BorrowBookCancellableTask()
    {
        LibraryDesk desk = new();

        using ReactiveCommand<int, Book> borrow = ReactiveCommand.CreateFromTask<int, Book>(desk.BorrowAsync);
        using ReactiveCommand<int, Book> borrowWhileOpen = ReactiveCommand.CreateFromTask<int, Book>(desk.BorrowAsync, desk.WhenAnyValue(d => d.IsOpen));
        using ReactiveCommand<int, Book> borrowImmediate = ReactiveCommand.CreateFromTask<int, Book>(desk.BorrowAsync, Sequencer.Immediate);
        using ReactiveCommand<int, Book> borrowGuarded = ReactiveCommand.CreateFromTask<int, Book>(desk.BorrowAsync, desk.WhenAnyValue(d => d.IsOpen), Sequencer.Immediate);

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
}
