// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.WhenAny.Mockups;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests that WhenAny dependency objects.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class WhenAnyThroughDependencyObjectTests
{
    /// <summary>
    /// Tests that WhenAny through a view shouldn't give null values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyThroughAViewShouldntGiveNullValues()
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

        using (Assert.Multiple())
        {
            await Assert.That(output).IsEmpty();
            await Assert.That(fixture.ViewModel).IsNull();
        }

        fixture.WhenAnyValue(static x => x.ViewModel!.Child!.IsNotNullString).Subscribe(output.Add);

        fixture.ViewModel = vm;
        await Assert.That(output).Count().IsEqualTo(1);

        fixture.ViewModel.Child.IsNotNullString = "Bar";
        await Assert.That(output).Count().IsEqualTo(2);
        await new[] { "Foo", "Bar" }.AssertAreEqual(output);
    }
}
