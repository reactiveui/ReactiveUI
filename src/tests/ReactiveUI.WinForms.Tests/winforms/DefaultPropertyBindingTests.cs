// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests default propery binding.</summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because it calls RxAppBuilder.EnsureInitialized()
/// in the constructor, which initializes global static state including the service locator.
/// This state must not be concurrently initialized by parallel tests.
/// </remarks>
[NotInParallel]
[TestExecutor<WinFormsViewsTestExecutor>]

public class DefaultPropertyBindingTests
{
    /// <summary>The timeout in seconds used when waiting for binding propagation.</summary>
    private const int TimeoutSeconds = 5;

    /// <summary>A sample double value used for binding tests.</summary>
    private const double SampleDouble = 123.4;

    /// <summary>The expected affinity for a matching panel binding converter.</summary>
    private const int ExpectedAffinity = 10;

    /// <summary>Verifies generated observation of a WinForms component delivers its initial value and changes.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValue_ToolStripButtonChecked_DeliversInitialAndChange()
    {
        using var button = new ToolStripButton();
        var values = new List<bool>();
        using var subscription = button.WhenAnyValue(static x => x.Checked).Subscribe(values.Add);

        await Assert.That(values).IsEquivalentTo([false]);

        button.Checked = true;

        await Assert.That(values).IsEquivalentTo([false, true]);
    }

    /// <summary>Tests that Winforms controled can bind to View Model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanBindViewModelToWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        vm.SomeText = "Foo";
        await Assert.That(view.Property3.Text).IsNotEqualTo(vm.SomeText);

        _ = view.Bind(vm, static x => x.SomeText, static x => x.Property3.Text);
        vm.SomeText = "Bar";
        await Assert.That(view.Property3.Text).IsEqualTo(vm.SomeText);

        // Set up observable to wait for ViewModel property change before setting View property
        var viewModelPropertyUpdated = vm.WhenAnyValue(static x => x.SomeText)
            .Where(static x => x == "Bar2")
            .Timeout(TimeSpan.FromSeconds(TimeoutSeconds))
            .FirstAsync();

        view.Property3.Text = "Bar2";

        // Wait for the two-way binding to propagate to the ViewModel
        await viewModelPropertyUpdated;
        await Assert.That(vm.SomeText).IsEqualTo("Bar2");

        _ = view.Bind(vm, static x => x.SomeDouble, static x => x.Property3.Text);
        vm.SomeDouble = SampleDouble;

        await Assert.That(view.Property3.Text).IsEqualTo(vm.SomeDouble.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>Smoke tests the WinForm controls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SmokeTestWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        var disp = new MultipleDisposable(
            view.Bind(vm, static x => x.Property1, static x => x.Property1.Text),
            view.Bind(vm, static x => x.Property2, static x => x.Property2.Text),
            view.Bind(vm, static x => x.Property3, static x => x.Property3.Text),
            view.Bind(vm, static x => x.Property4, static x => x.Property4.Text),
            view.Bind(vm, static x => x.BooleanProperty, static x => x.BooleanProperty.Checked));

        vm.Property1 = "FOOO";
        await Assert.That(view.Property1.Text).IsEqualTo(vm.Property1);

        vm.Property2 = "FOOO1";
        await Assert.That(view.Property2.Text).IsEqualTo(vm.Property2);

        vm.Property3 = "FOOO2";
        await Assert.That(view.Property3.Text).IsEqualTo(vm.Property3);

        vm.Property4 = "FOOO3";
        await Assert.That(view.Property4.Text).IsEqualTo(vm.Property4);

        vm.BooleanProperty = false;
        await Assert.That(view.BooleanProperty.Checked).IsEqualTo(vm.BooleanProperty);
        vm.BooleanProperty = true;
        await Assert.That(view.BooleanProperty.Checked).IsEqualTo(vm.BooleanProperty);

        disp.Dispose();
    }

    /// <summary>Tests that PanelSetMethodBindingConverter returns the expected affinity for various object types.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task PanelSetMethodBindingConverter_GetAffinityForObjects()
    {
        var fixture = new PanelSetMethodBindingConverter();
        var test1 = fixture.GetAffinityForObjects(typeof(List<Control>), typeof(Control.ControlCollection));
        var test2 = fixture.GetAffinityForObjects(typeof(List<TextBox>), typeof(Control.ControlCollection));
        var test3 = fixture.GetAffinityForObjects(typeof(List<Label>), typeof(Control.ControlCollection));
        var test4 = fixture.GetAffinityForObjects(typeof(Control.ControlCollection), typeof(IEnumerable<GridItem>));

        using (Assert.Multiple())
        {
            await Assert.That(test1).IsEqualTo(0);
            await Assert.That(test2).IsEqualTo(ExpectedAffinity);
            await Assert.That(test3).IsEqualTo(ExpectedAffinity);
            await Assert.That(test4).IsEqualTo(0);
        }
    }
}
