// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
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

    /// <summary>Read-only dependency properties notify through both generated and runtime observations.</summary>
    /// <param name="runtimeFallback">Whether to observe through the runtime fallback.</param>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [Arguments(false)]
    [Arguments(true)]
    public async Task WhenAnyValue_ReadOnlyDependencyProperty_DeliversChanges(bool runtimeFallback)
    {
        var view = new ReadOnlyFixture();
        var values = new List<bool>();
        var changes = runtimeFallback
            ? view.WhenAnyValueUnsafe(static x => x.Enabled)
            : view.WhenAnyValue(static x => x.Enabled);
        var subscription = changes.Subscribe(values.Add);

        view.SetEnabled(true);
        subscription.Dispose();
        view.SetEnabled(false);

        await Assert.That(values).IsEquivalentTo([false, true]);
    }

    /// <summary>A dependency object whose value can only be written with its registration key.</summary>
    public sealed class ReadOnlyFixture : DependencyObject
    {
        /// <summary>Identifies the read-only dependency property.</summary>
        public static readonly DependencyProperty EnabledProperty;

        /// <summary>The key required to change the dependency property's value.</summary>
        private static readonly DependencyPropertyKey _enabledPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Enabled),
            typeof(bool),
            typeof(ReadOnlyFixture),
            new(false));

        /// <summary>Initializes static members of the <see cref="ReadOnlyFixture"/> class.</summary>
        static ReadOnlyFixture() => EnabledProperty = _enabledPropertyKey.DependencyProperty;

        /// <summary>Gets the value exposed to observation.</summary>
        public bool Enabled => (bool)GetValue(EnabledProperty);

        /// <summary>Changes the value using the private registration key.</summary>
        /// <param name="value">The next value.</param>
        public void SetEnabled(bool value) => SetValue(_enabledPropertyKey, value);
    }
}
