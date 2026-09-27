// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>
/// A receipt printer that watches a borrow command's results directly, by implementing <see cref="IObserver{T}"/>
/// itself, rather than through an operator such as <c>Subscribe(Action&lt;T&gt;)</c>.
/// </summary>
public sealed class BookReceiptPrinter : IObserver<Book>
{
    /// <inheritdoc/>
    public void OnNext(Book value) => Console.WriteLine($"Printed a receipt for {value.Title}");

    /// <inheritdoc/>
    public void OnError(Exception error) => Console.WriteLine($"Printer jammed: {error.Message}");

    /// <inheritdoc/>
    public void OnCompleted() => Console.WriteLine("No more receipts to print");
}
