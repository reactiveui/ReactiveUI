// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>Exercises generated Binding observation against WPF dependency properties through ReactiveUI.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class DependencyObjectObservationIntegrationTests
{
    /// <summary>The expected number of values after one property change.</summary>
    private const int ExpectedCountTwo = 2;

    /// <summary>The expected number of values after two property changes.</summary>
    private const int ExpectedCountThree = 3;

    /// <summary>Verifies that generated observation delivers the initial value and later dependency-property changes.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValue_DependencyProperty_DeliversInitialAndChanges()
    {
        var view = new DepObjFixture();
        var values = view.WhenAnyValue(static x => x.TestString).Collect();

        await Assert.That(values).Count().IsEqualTo(1);
        await Assert.That(values[0]).IsNull();

        view.TestString = "Foo";
        view.TestString = "Bar";

        await Assert.That(values).Count().IsEqualTo(ExpectedCountThree);
        await Assert.That(values[1]).IsEqualTo("Foo");
        await Assert.That(values[2]).IsEqualTo("Bar");
    }

    /// <summary>Verifies that generated observation follows a dependency property inherited by a WPF control.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValue_ListBoxSelectedItem_DeliversChanges()
    {
        var view = new ListBox();
        _ = view.Items.Add("Foo");
        _ = view.Items.Add("Bar");

        var values = view.WhenAnyValue(static x => x.SelectedItem).Collect();
        await Assert.That(values).Count().IsEqualTo(1);

        view.SelectedIndex = 1;
        await Assert.That(values).Count().IsEqualTo(ExpectedCountTwo);
        await Assert.That(values[0]).IsNull();
        await Assert.That(values[1]).IsEqualTo("Bar");
    }

    /// <summary>Verifies that disposing a generated observation stops later dependency-property deliveries.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValue_DisposedSubscription_StopsDelivery()
    {
        var view = new DepObjFixture();
        var values = new List<string?>();
        var subscription = view.WhenAnyValue(static x => x.TestString).Subscribe(values.Add);

        view.TestString = "First";
        subscription.Dispose();
        view.TestString = "Second";

        await Assert.That(values).Count().IsEqualTo(ExpectedCountTwo);
        await Assert.That(values[0]).IsNull();
        await Assert.That(values[1]).IsEqualTo("First");
    }
}
