// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFX_CORE
#else

#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests that WhenAny dependency objects.
/// </summary>
[TestFixture]
public class WhenAnyThroughDependencyObjectTests
{
    /// <summary>
    /// Tests that WhenAny through a view shouldn't give null values.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(output, Is.Empty);
            Assert.That(fixture.ViewModel, Is.Null);
        }

        fixture.WhenAnyValue(x => x.ViewModel!.Child!.IsNotNullString).Subscribe(output.Add);

        fixture.ViewModel = vm;
        Assert.That(output, Has.Count.EqualTo(1));

        fixture.ViewModel.Child.IsNotNullString = "Bar";
        Assert.That(output, Has.Count.EqualTo(2));
        new[] { "Foo", "Bar" }.AssertAreEqual(output);
    }
}
