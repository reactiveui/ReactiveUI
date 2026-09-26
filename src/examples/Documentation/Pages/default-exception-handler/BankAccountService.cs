// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Documentation.DefaultExceptionHandler;

/// <summary>Stands in for a bank's account server; every balance lookup fails while the server is marked down.</summary>
[System.Diagnostics.DebuggerDisplay("ServerIsDown = {ServerIsDown}")]
public sealed class BankAccountService : IEnableLogger
{
    /// <summary>The accounts the server knows about.</summary>
    private static readonly BankAccount[] _accounts =
    [
        new("checking-01", "Ada Lovelace", 500.00M),
        new("savings-02", "Grace Hopper", 1200.50M),
    ];

    /// <summary>Gets or sets a value indicating whether the server is down, so every balance lookup fails.</summary>
    public bool ServerIsDown { get; set; }

    /// <summary>Loads the balance for an account.</summary>
    /// <param name="accountId">The account to load.</param>
    /// <returns>A signal that emits the balance, or fails with <see cref="AccountServiceException"/>.</returns>
    public IObservable<decimal> LoadBalance(string accountId)
    {
        if (ServerIsDown)
        {
            return Signal.Fail<decimal>(new AccountServiceException($"The bank server could not load account '{accountId}'."));
        }

        BankAccount? account = FindAccount(accountId);

        return account is null
            ? Signal.Fail<decimal>(new AccountServiceException($"No account '{accountId}'."))
            : Signal.Emit(account.Balance);
    }

    /// <summary>Finds the account with the given id.</summary>
    /// <param name="accountId">The account id to search for.</param>
    /// <returns>The matching account, or null if no account has that id.</returns>
    public BankAccount? FindAccount(string accountId) =>
        Array.Find(_accounts, account => account.Id == accountId);
}
