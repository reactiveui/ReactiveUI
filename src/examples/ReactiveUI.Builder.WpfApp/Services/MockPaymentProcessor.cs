// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using ReactiveUI.Builder.WpfApp.Models;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>A fake payment processor that simulates an authorization round-trip with a small delay and canned rules.</summary>
public sealed class MockPaymentProcessor : IPaymentProcessor
{
    /// <summary>The amount above which a transaction is declined for exceeding the floor limit.</summary>
    private const decimal FloorLimit = 1000M;

    /// <summary>The simulated authorization round-trip duration, in milliseconds.</summary>
    private const int AuthorizationDelayMilliseconds = 1400;

    /// <summary>The number of cents in one currency unit.</summary>
    private const long CentsPerUnit = 100;

    /// <summary>The cents value that triggers a canned "do not honour" decline.</summary>
    private const long DeclineCents = 13;

    /// <summary>The modulus used to derive the masked last four card digits.</summary>
    private const long LastFourModulus = 10_000;

    /// <summary>The modulus used to derive the six-digit authorization code.</summary>
    private const long AuthModulus = 1_000_000;

    /// <summary>The card schemes a card might be issued under.</summary>
    private static readonly string[] Schemes = ["VISA", "MASTERCARD", "AMEX", "EFTPOS"];

    /// <summary>A monotonically increasing counter used to vary the fabricated card and authorization details.</summary>
    private long _sequence;

    /// <inheritdoc/>
    public async Task<Transaction> AuthorizeAsync(decimal amount, CancellationToken cancellationToken)
    {
        await Task.Delay(AuthorizationDelayMilliseconds, cancellationToken).ConfigureAwait(true);

        var sequence = Interlocked.Increment(ref _sequence);
        var cents = (long)Math.Round(amount * CentsPerUnit);
        var declinedForLimit = amount > FloorLimit;
        var declinedByIssuer = Math.Abs(cents) % CentsPerUnit == DeclineCents;
        var approved = !declinedForLimit && !declinedByIssuer;

        return new Transaction
        {
            Reference = $"RX{sequence.ToString("D8", CultureInfo.InvariantCulture)}",
            Amount = amount,
            Approved = approved,
            Outcome = BuildOutcome(approved, declinedForLimit, sequence),
            CardScheme = Schemes[(int)(sequence % Schemes.Length)],
            CardLast4 = (cents % LastFourModulus).ToString("D4", CultureInfo.InvariantCulture),
            Timestamp = UtcNow(),
        };
    }

    /// <summary>Builds the outcome message for a completed authorization.</summary>
    /// <param name="approved">Whether the transaction was approved.</param>
    /// <param name="declinedForLimit">Whether the decline was caused by the floor limit.</param>
    /// <param name="sequence">The transaction sequence number used to derive the authorization code.</param>
    /// <returns>The outcome message.</returns>
    private static string BuildOutcome(bool approved, bool declinedForLimit, long sequence)
    {
        if (!approved)
        {
            return declinedForLimit ? "DECLINED · EXCEEDS FLOOR LIMIT" : "DECLINED · DO NOT HONOUR";
        }

        var authCode = (sequence % AuthModulus).ToString("D6", CultureInfo.InvariantCulture);
        return $"APPROVED · AUTH {authCode}";
    }

    /// <summary>Gets the current UTC instant.</summary>
    /// <returns>The current UTC instant.</returns>
    private static DateTimeOffset UtcNow() =>
#if NET8_0_OR_GREATER
        TimeProvider.System.GetUtcNow();
#else
        DateTimeOffset.UtcNow;
#endif
}
