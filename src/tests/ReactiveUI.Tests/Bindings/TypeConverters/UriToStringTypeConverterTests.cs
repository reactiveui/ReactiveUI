// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.TypeConverters;

/// <summary>
/// Tests for converting Uri to strings.
/// </summary>
public class UriToStringTypeConverterTests
{
    [Test]
    public async Task GetAffinityForObjects_Returns2()
    {
        var converter = new UriToStringTypeConverter();
        var affinity = converter.GetAffinityForObjects();
        await Assert.That(affinity).IsEqualTo(2);
    }

    [Test]
    public async Task TryConvert_AbsoluteUri_Succeeds()
    {
        var converter = new UriToStringTypeConverter();
        var value = new Uri("https://reactiveui.net/docs");

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("https://reactiveui.net/docs");
    }

    [Test]
    public async Task TryConvert_RelativeUri_Succeeds()
    {
        var converter = new UriToStringTypeConverter();
        var value = new Uri("/path/to/resource", UriKind.Relative);

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsTrue();
        await Assert.That(output).IsEqualTo("/path/to/resource");
    }

    [Test]
    public async Task TryConvert_Null_ReturnsFalse()
    {
        var converter = new UriToStringTypeConverter();
        Uri? value = null;

        var result = converter.TryConvert(value, null, out var output);

        await Assert.That(result).IsFalse();
    }
}
