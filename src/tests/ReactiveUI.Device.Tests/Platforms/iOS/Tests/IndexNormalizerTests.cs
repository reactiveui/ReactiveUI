// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

namespace ReactiveUI.Device.Tests;

/// <summary>Tests <see cref="IndexNormalizer"/>, which orders collection updates the way UIKit batch updates need them.</summary>
/// <remarks>Each update is written as <c>A</c> (add) or <c>D</c> (delete) followed by an index; a colon separates updates.</remarks>
public class IndexNormalizerTests
{
    /// <summary>Deletes come first against the source indexes, inserts follow against the result, and cancelling pairs drop out.</summary>
    /// <param name="input">The updates in the order the client made them.</param>
    /// <param name="expected">The normalized updates.</param>
    /// <returns>A task representing the test.</returns>
    [Test]
    [Arguments("", "")]
    [Arguments("D0:D0", "D0:D1")]
    [Arguments("D0:D0:D0", "D0:D1:D2")]
    [Arguments("D2:D0:D1", "D2:D0:D3")]
    [Arguments("D1:D0", "D1:D0")]
    [Arguments("D0:D1", "D0:D2")]
    [Arguments("D0:D5:D10", "D0:D6:D12")]
    [Arguments("D5:D0:D10", "D5:D0:D12")]
    [Arguments("A0:D1", "A0:D0")]
    [Arguments("D0:A0", "D0:A0")]
    [Arguments("D0:A1", "D0:A1")]
    [Arguments("A0:D0", "")]
    [Arguments("A0:A0:D0", "A0")]
    [Arguments("A1:A1:D0", "A1:A0:D0")]
    [Arguments("A1:D0:A1", "A0:D0:A1")]
    [Arguments("A1:D1", "")]
    [Arguments("A0:A1:D0", "A0")]
    [Arguments("A0:A0", "A1:A0")]
    [Arguments("A0:A0:A0", "A2:A1:A0")]
    [Arguments("A0:A1", "A0:A1")]
    [Arguments("A1:A0", "A2:A0")]
    [Arguments("A0:A10:D5:A6:D3:D6", "A0:A8:D4:A5:D2:D6")]
    [Arguments("A0:A10:D5:A6:D7:D6", "A0:A8:D4:D6")]
    [Arguments("A0:A0:A0:D0:D1", "A0")]
    [Arguments("A0:A1:A2:D0:D1", "A0")]
    [Arguments("A0:A10:D5:D7", "A0:A8:D4:D7")]
    [Arguments("A0:A0:D2:A3:D4", "A1:A0:D0:A3:D2")]
    [Arguments("D0:A0:D1", "D0:A0:D1")]
    [Arguments("A0:D1:D0", "D0")]
    [Arguments("A1:D1:D1", "D1")]
    [Arguments("D0:A0:A0:A5:A2:D3:A5:D2:A3:A1", "D0:A2:A0:A7:D1:A6:A4:A1")]
    [Arguments("A2:A5:D2", "A4")]
    [Arguments("A2:D3:A5:D2", "D2:A4")]
    [Arguments("A5:A2:D3:A5:D2", "A5:D2:A4")]
    [Arguments("A5:A2:A5:D2", "A6:A4")]
    [Arguments("A7:A2:D3:A6:A2", "A9:A3:D2:A7:A2")]
    [Arguments("D0:D0:A6:A0:D5:D0:D4:A4:A0:A6", "D0:D1:A7:D6:D7:A5:A0:A6")]
    [Arguments("D0:D0:A6:D5:D4:A4:A0:A6", "D0:D1:A7:D7:D6:A5:A0:A6")]
    [Arguments("D0:D0:A6:D5:D4", "D0:D1:A4:D7:D6")]
    public async Task Normalize_OrdersAndDeduplicatesUpdates(string input, string expected)
    {
        var updates = input.Split(':', StringSplitOptions.RemoveEmptyEntries).Select(Parse);

        var normalized = IndexNormalizer.Normalize(updates);

        await Assert.That(string.Join(':', normalized.Select(static update => update!.ToString()))).IsEqualTo(expected);
    }

    /// <summary>Parses one update token such as <c>A3</c> or <c>D0</c>.</summary>
    /// <param name="token">The token.</param>
    /// <returns>The update.</returns>
    private static Update Parse(string token)
    {
        var index = int.Parse(token.AsSpan(1), CultureInfo.InvariantCulture);
        return token[0] is 'A' ? Update.CreateAdd(index) : Update.CreateDelete(index);
    }
}
