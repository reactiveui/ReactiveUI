// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.DefaultExceptionHandler;

/// <summary>Shows <c>WhereNotNull</c> dropping the misses from a lookup, so only accounts that exist reach the subscriber.</summary>
public static class WhereNotNullExamples
{
    /// <summary>Selecting an id that has no account produces null; <c>WhereNotNull</c> filters it out and converts the nullability.</summary>
    public static void KeepOnlyTheSelectedAccount()
    {
        BankAccountService server = new();
        using Signal<string> selectedAccountId = new();
        List<BankAccount> selectedAccounts = [];

        using IDisposable subscription = selectedAccountId
            .Select(server.FindAccount)
            .WhereNotNull()
            .Subscribe(selectedAccounts.Add);

        selectedAccountId.OnNext("checking-01");
        selectedAccountId.OnNext("unknown-99");
        selectedAccountId.OnNext("savings-02");

        foreach (BankAccount account in selectedAccounts)
        {
            Console.WriteLine(account.Owner);
        }

        // Output:
        // Ada Lovelace
        // Grace Hopper
    }
}
