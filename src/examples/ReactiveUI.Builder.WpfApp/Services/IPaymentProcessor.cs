// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>Authorizes card payments for the terminal.</summary>
public interface IPaymentProcessor
{
    /// <summary>Authorizes a payment for the supplied amount.</summary>
    /// <param name="amount">The amount to authorize.</param>
    /// <param name="cancellationToken">A token used to cancel the authorization.</param>
    /// <returns>The completed <see cref="Transaction"/>.</returns>
    Task<Transaction> AuthorizeAsync(decimal amount, CancellationToken cancellationToken);
}
