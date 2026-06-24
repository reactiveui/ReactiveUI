// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ReactiveUI.Builder.WpfApp.Models;
using ReactiveUI.Builder.WpfApp.Services;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>
/// The point-of-sale terminal. Digits are entered on a keypad to build an amount, then an asynchronous
/// <see cref="PayCommand"/> (a <c>ReactiveCommand.CreateFromTask</c>) authorizes a mock payment, records it in the
/// persisted journal and surfaces the result.
/// </summary>
public sealed class TerminalViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>The largest amount, in cents, the keypad will accept ($99,999.99).</summary>
    private const long MaximumAmountCents = 9_999_999;

    /// <summary>The number of cents in one currency unit.</summary>
    private const decimal CentsPerUnit = 100m;

    /// <summary>The base used when shifting digits onto the entered amount.</summary>
    private const long Radix = 10;

    /// <summary>The processor that authorizes payments.</summary>
    private readonly IPaymentProcessor _processor;

    /// <summary>Backs the derived <see cref="Amount"/> property.</summary>
    private readonly ObservableAsPropertyHelper<decimal> _amount;

    /// <summary>Backs the derived <see cref="FormattedAmount"/> property.</summary>
    private readonly ObservableAsPropertyHelper<string> _formattedAmount;

    /// <summary>Backs the derived <see cref="IsBusy"/> property.</summary>
    private readonly ObservableAsPropertyHelper<bool> _isBusy;

    /// <summary>Initializes a new instance of the <see cref="TerminalViewModel"/> class.</summary>
    /// <param name="hostScreen">The screen hosting this view model.</param>
    /// <param name="processor">The payment processor used to authorize transactions.</param>
    [SuppressMessage(
        "Major Code Smell",
        "S3366:Make sure the use of this in constructors is safe",
        Justification = "Single-threaded WPF view model fully constructed before the reactive pipelines run.")]
    public TerminalViewModel(IScreen hostScreen, IPaymentProcessor processor)
    {
        ArgumentExceptionHelper.ThrowIfNull(processor);
        HostScreen = hostScreen;
        UrlPathSegment = "terminal";
        _processor = processor;

        _amount = this.WhenAnyValue(x => x.AmountCents, static cents => cents / CentsPerUnit)
            .ToProperty(this, nameof(Amount));

        _formattedAmount = this.WhenAnyValue(x => x.Amount, static amount => amount.ToString("C", CultureInfo.CurrentCulture))
            .ToProperty(this, nameof(FormattedAmount));

        AppendDigit = ReactiveCommand.Create<string>(AppendDigitImpl);
        Backspace = ReactiveCommand.Create(RemoveLastDigit);
        Clear = ReactiveCommand.Create(ClearAmount);

        var canPay = this.WhenAnyValue(x => x.AmountCents, static cents => cents > 0);
        PayCommand = ReactiveCommand.CreateFromTask(ct => _processor.AuthorizeAsync(Amount, ct), canPay);
        _isBusy = PayCommand.IsExecuting.ToProperty(this, nameof(IsBusy));

        _ = PayCommand.WitnessOn(RxSchedulers.MainThreadScheduler).Subscribe(transaction =>
        {
            Journal.Insert(0, transaction);
            LastTransaction = transaction;
            AmountCents = 0;
            MessageBus.Current.SendMessage(transaction);
        });

        ViewJournal = ReactiveCommand.CreateFromObservable(
            () => HostScreen.Router.Navigate.Execute(new JournalViewModel(HostScreen)));
    }

    /// <inheritdoc/>
    public string UrlPathSegment { get; }

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets or sets the amount entered on the keypad, in cents.</summary>
    public long AmountCents
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the most recently completed transaction.</summary>
    public Transaction? LastTransaction
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the entered amount as a currency value.</summary>
    public decimal Amount => _amount.Value;

    /// <summary>Gets the entered amount formatted as currency for display.</summary>
    public string FormattedAmount => _formattedAmount.Value;

    /// <summary>Gets a value indicating whether a payment is currently being authorized.</summary>
    public bool IsBusy => _isBusy.Value;

    /// <summary>Gets the command that appends a pressed digit to the amount.</summary>
    public ReactiveCommand<string, RxVoid> AppendDigit { get; }

    /// <summary>Gets the command that removes the last entered digit.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Backspace { get; }

    /// <summary>Gets the command that clears the entered amount.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Clear { get; }

    /// <summary>Gets the command that authorizes the payment for the entered amount.</summary>
    public ReactiveCommand<RxVoid, Transaction> PayCommand { get; }

    /// <summary>Gets the command that navigates to the transaction journal.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> ViewJournal { get; }

    /// <summary>Gets the persisted transaction journal from the current suspension state.</summary>
    private static ObservableCollection<Transaction> Journal =>
        ((TerminalState)RxSuspension.SuspensionHost.AppState!).Journal;

    /// <summary>Appends a pressed keypad digit to the amount, ignoring presses that would overflow the limit.</summary>
    /// <param name="digit">The digit that was pressed.</param>
    private void AppendDigitImpl(string digit)
    {
        if (!long.TryParse(digit, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        var next = (AmountCents * Radix) + value;
        if (next > MaximumAmountCents)
        {
            return;
        }

        AmountCents = next;
    }

    /// <summary>Removes the last entered digit from the amount.</summary>
    private void RemoveLastDigit() => AmountCents /= Radix;

    /// <summary>Clears the entered amount.</summary>
    private void ClearAmount() => AmountCents = 0;
}
