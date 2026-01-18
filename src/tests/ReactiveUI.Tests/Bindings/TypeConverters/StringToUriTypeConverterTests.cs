// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting strings to Uri.
/// </summary>
public class StringToUriTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new StringToUriTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_AbsoluteUri_Succeeds()
    {
        var converter = new StringToUriTypeConverter();
        var expected = new Uri("https://reactiveui.net/docs");

        var result = converter.TryConvert("https://reactiveui.net/docs", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(expected);
    }

    [Test]
    public async Task TryConvert_RelativeUri_Succeeds()
    {
        var converter = new StringToUriTypeConverter();
        var expected = new Uri("/path/to/resource", UriKind.Relative);

        var result = converter.TryConvert("/path/to/resource", null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo(expected);
    }

    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new StringToUriTypeConverter();

        var result = converter.TryConvert(null, null, out var output);

        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task TryConvert_EmptyString_CreatesRelativeUri()
    {
        var converter = new StringToUriTypeConverter();

        var result = converter.TryConvert(string.Empty, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsNotNull();
    }
}
