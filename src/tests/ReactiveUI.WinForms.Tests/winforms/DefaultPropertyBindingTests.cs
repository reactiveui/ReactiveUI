// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Forms;
using DynamicData;
using ReactiveUI.Winforms;
using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests default propery binding.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because it calls RxAppBuilder.EnsureInitialized()
/// in the constructor, which initializes global static state including the service locator.
/// This state must not be concurrently initialized by parallel tests.
/// </remarks>
[NotInParallel]
[TestExecutor<WinFormsViewsTestExecutor>]

public class DefaultPropertyBindingTests
{
    /// <summary>
    /// Tests Winforms creates observable for property works for textboxes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinformsCreatesObservableForPropertyWorksForTextboxes()
    {
        var input = new TextBox();
        var fixture = new WinformsCreatesObservableForProperty();

        await Assert.That(fixture.GetAffinityForObject(typeof(TextBox), "Text")).IsNotEqualTo(0);

        Expression<Func<TextBox, string>> expression = static x => x.Text;

        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        await Assert.That(output).IsEmpty();

        input.Text = "Foo";
        await Assert.That(output).Count().IsEqualTo(1);
        using (Assert.Multiple())
        {
            await Assert.That(output[0].Sender).IsEqualTo(input);
            await Assert.That(output[0].GetPropertyName()).IsEqualTo("Text");
        }

        dispose.Dispose();

        input.Text = "Bar";
        await Assert.That(output).Count().IsEqualTo(1);
    }

    /// <summary>
    /// Tests that Winform creates observable for property works for components.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinformsCreatesObservableForPropertyWorksForComponents()
    {
        var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control
        var fixture = new WinformsCreatesObservableForProperty();

        await Assert.That(fixture.GetAffinityForObject(typeof(ToolStripButton), "Checked")).IsNotEqualTo(0);

        Expression<Func<ToolStripButton, bool>> expression = static x => x.Checked;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        await Assert.That(output).IsEmpty();

        input.Checked = true;
        await Assert.That(output).Count().IsEqualTo(1);
        using (Assert.Multiple())
        {
            await Assert.That(output[0].Sender).IsEqualTo(input);
            await Assert.That(output[0].GetPropertyName()).IsEqualTo("Checked");
        }

        dispose.Dispose();

        // Since we disposed the derived list, we should no longer receive updates
        input.Checked = false;
        await Assert.That(output).Count().IsEqualTo(1);
    }

    /// <summary>
    /// Tests that winforms creates observable for property works for third party controls.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinformsCreatesObservableForPropertyWorksForThirdPartyControls()
    {
        var input = new ThirdPartyControl();
        var fixture = new WinformsCreatesObservableForProperty();

        await Assert.That(fixture.GetAffinityForObject(typeof(ThirdPartyControl), "Value")).IsNotEqualTo(0);

        Expression<Func<ThirdPartyControl, string?>> expression = static x => x.Value;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        await Assert.That(output).IsEmpty();

        input.Value = "Foo";
        await Assert.That(output).Count().IsEqualTo(1);
        using (Assert.Multiple())
        {
            await Assert.That(output[0].Sender).IsEqualTo(input);
            await Assert.That(output[0].GetPropertyName()).IsEqualTo("Value");
        }

        dispose.Dispose();

        input.Value = "Bar";
        await Assert.That(output).Count().IsEqualTo(1);
    }

    /// <summary>
    /// Tests that Winforms controled can bind to View Model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanBindViewModelToWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        vm.SomeText = "Foo";
        await Assert.That(view.Property3.Text).IsNotEqualTo(vm.SomeText);

        var disp = view.Bind(vm, static x => x.SomeText, static x => x.Property3.Text);
        vm.SomeText = "Bar";
        await Assert.That(view.Property3.Text).IsEqualTo(vm.SomeText);

        view.Property3.Text = "Bar2";
        await Assert.That(view.Property3.Text).IsEqualTo(vm.SomeText);

        var disp2 = view.Bind(vm, static x => x.SomeDouble, static x => x.Property3.Text);
        vm.SomeDouble = 123.4;

        await Assert.That(view.Property3.Text).IsEqualTo(vm.SomeDouble.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Smoke tests the WinForm controls.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SmokeTestWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        var disp = new CompositeDisposable(
        [
            view.Bind(vm, static x => x.Property1, static x => x.Property1.Text),
            view.Bind(vm, static x => x.Property2, static x => x.Property2.Text),
            view.Bind(vm, static x => x.Property3, static x => x.Property3.Text),
            view.Bind(vm, static x => x.Property4, static x => x.Property4.Text),
            view.Bind(vm, static x => x.BooleanProperty, static x => x.BooleanProperty.Checked)]);

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
            await Assert.That(test2).IsEqualTo(10);
            await Assert.That(test3).IsEqualTo(10);
            await Assert.That(test4).IsEqualTo(0);
        }
    }

    [Test]
    public async Task WinformsCreatesObservableForProperty_GetAffinityForObject_Returns_Zero_For_BeforeChanged()
    {
        var fixture = new WinformsCreatesObservableForProperty();
        var affinity = fixture.GetAffinityForObject(typeof(TextBox), "Text", beforeChanged: true);

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task WinformsCreatesObservableForProperty_GetAffinityForObject_Returns_Zero_For_NonComponent()
    {
        var fixture = new WinformsCreatesObservableForProperty();
        var affinity = fixture.GetAffinityForObject(typeof(string), "Length", beforeChanged: false);

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task WinformsCreatesObservableForProperty_GetAffinityForObject_Returns_Zero_For_NonExistent_Event()
    {
        var fixture = new WinformsCreatesObservableForProperty();
        var affinity = fixture.GetAffinityForObject(typeof(TextBox), "NonExistentProperty", beforeChanged: false);

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task WinformsCreatesObservableForProperty_GetNotificationForProperty_Throws_For_NonExistent_Event()
    {
        var input = new TextBox();
        var fixture = new WinformsCreatesObservableForProperty();

        Expression<Func<TextBox, string>> expression = static x => x.Text;
        var propertyName = "NonExistentProperty"; // Property with no corresponding event

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
        {
            var observable = fixture.GetNotificationForProperty(input, expression.Body, propertyName)
                .ObserveOn(ImmediateScheduler.Instance);

            // Need to subscribe to actually execute the observable creation
            observable.Subscribe();
            return Task.CompletedTask;
        });

        await Assert.That(exception!.Message).Contains("Could not find a valid event");
    }
}
