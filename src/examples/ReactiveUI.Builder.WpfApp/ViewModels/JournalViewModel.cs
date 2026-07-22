// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>Shows the persisted transaction journal and the running total of today's approved takings.</summary>
public sealed class JournalViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Backs the derived <see cref="TodayTotal"/> property.</summary>
    private readonly ObservableAsPropertyHelper<decimal> _todayTotal;

    /// <summary>Initializes a new instance of the <see cref="JournalViewModel"/> class.</summary>
    /// <param name="hostScreen">The screen hosting this view model.</param>
    [SuppressMessage(
        "Correctness",
        "SST2403:Do not let 'this' escape from a constructor",
        Justification = "Single-threaded WPF sample view model; ToProperty binds the OAPH to this after all fields are set.")]
    public JournalViewModel(IScreen hostScreen)
    {
        ArgumentExceptionHelper.ThrowIfNull(hostScreen);
        HostScreen = hostScreen;
        UrlPathSegment = "journal";
        Transactions = ((TerminalState)RxSuspension.SuspensionHost.AppState!).Journal;

        _todayTotal = Transactions.ObserveCollectionChanges()
            .Select(_ => ComputeTodayTotal())
            .ToProperty(this, nameof(TodayTotal), ComputeTodayTotal());

        GoBack = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
        Clear = ReactiveCommand.Create(Transactions.Clear);
    }

    /// <inheritdoc/>
    public string UrlPathSegment { get; }

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the persisted transactions, most recent first.</summary>
    public ObservableCollection<Transaction> Transactions { get; }

    /// <summary>Gets the total value of today's approved transactions.</summary>
    public decimal TodayTotal => _todayTotal.Value;

    /// <summary>Gets the command that navigates back to the terminal.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> GoBack { get; }

    /// <summary>Gets the command that clears the journal.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Clear { get; }

    /// <summary>Gets the current UTC date.</summary>
    /// <returns>Today's date.</returns>
#if NET8_0_OR_GREATER
    private static DateOnly UtcToday() =>
        DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime);
#else
    private static DateTime UtcToday() =>
        DateTimeOffset.UtcNow.UtcDateTime.Date;
#endif

    /// <summary>Sums today's approved transactions without LINQ allocations.</summary>
    /// <returns>The total value of today's approved takings.</returns>
    private decimal ComputeTodayTotal()
    {
        var today = UtcToday();
        decimal total = 0M;
        foreach (var transaction in Transactions)
        {
#if NET8_0_OR_GREATER
            if (transaction.Approved && DateOnly.FromDateTime(transaction.Timestamp.UtcDateTime) == today)
#else
            if (transaction.Approved && transaction.Timestamp.UtcDateTime.Date == today)
#endif
            {
                total += transaction.Amount;
            }
        }

        return total;
    }
}
