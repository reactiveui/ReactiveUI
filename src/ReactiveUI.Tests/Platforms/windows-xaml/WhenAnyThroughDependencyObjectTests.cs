// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFX_CORE
#else

using FactAttribute = Xunit.WpfFactAttribute;

#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests that WhenAny dependency objects.
/// </summary>
public class WhenAnyThroughDependencyObjectTests
{
    /// <summary>
    /// Tests that WhenAny through a view shouldn't give null values.
    /// </summary>
    [Fact]
    public void WhenAnyThroughAViewShouldntGiveNullValues()
    {
        var vm = new HostTestFixture()
        {
            Child = new TestFixture
            {
                IsNotNullString = "Foo",
                IsOnlyOneWord = "Baz",
                PocoProperty = "Bamf"
            },
        };

        var fixture = new HostTestView();

        var output = new List<string?>();

        Assert.Equal(0, output.Count);
        Assert.Null(fixture.ViewModel);

        fixture.WhenAnyValue(x => x.ViewModel!.Child!.IsNotNullString).Subscribe(output.Add);

        fixture.ViewModel = vm;
        Assert.Equal(1, output.Count);

        fixture.ViewModel.Child.IsNotNullString = "Bar";
        Assert.Equal(2, output.Count);
        new[] { "Foo", "Bar" }.AssertAreEqual(output);
    }
}
