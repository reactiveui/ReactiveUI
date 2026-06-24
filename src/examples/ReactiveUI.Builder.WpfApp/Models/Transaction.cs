// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.WpfApp.Models;

/// <summary>A single (mock) EFTPOS transaction recorded in the terminal journal.</summary>
public sealed class Transaction
{
    /// <summary>Gets or sets the bank authorization reference.</summary>
    public string Reference { get; set; } = string.Empty;

    /// <summary>Gets or sets the transaction amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Gets or sets a value indicating whether the transaction was approved.</summary>
    public bool Approved { get; set; }

    /// <summary>Gets or sets the outcome message (an approval code or a decline reason).</summary>
    public string Outcome { get; set; } = string.Empty;

    /// <summary>Gets or sets the card scheme used (for example, <c>VISA</c>).</summary>
    public string CardScheme { get; set; } = string.Empty;

    /// <summary>Gets or sets the masked last four digits of the card.</summary>
    public string CardLast4 { get; set; } = string.Empty;

    /// <summary>Gets or sets the moment the transaction completed.</summary>
    public DateTimeOffset Timestamp { get; set; }
}
