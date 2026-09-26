// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.DefaultExceptionHandler;

/// <summary>A bank account the account server can look up and report a balance for.</summary>
/// <param name="Id">The account number.</param>
/// <param name="Owner">The account holder's name.</param>
/// <param name="Balance">The account balance.</param>
[System.Diagnostics.DebuggerDisplay("{Owner}: {Balance}")]
public sealed record BankAccount(string Id, string Owner, decimal Balance);
