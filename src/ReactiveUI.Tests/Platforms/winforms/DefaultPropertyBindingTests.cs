// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows.Forms;

using DynamicData;

using ReactiveUI.Winforms;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests default propery binding.
/// </summary>
public class DefaultPropertyBindingTests
{
    /// <summary>
    /// Tests Winforms creates observable for property works for textboxes.
    /// </summary>
    [Fact]
    public void WinformsCreatesObservableForPropertyWorksForTextboxes()
    {
        var input = new TextBox();
        var fixture = new WinformsCreatesObservableForProperty();

        Assert.NotEqual(0, fixture.GetAffinityForObject(typeof(TextBox), "Text"));

        Expression<Func<TextBox, string>> expression = x => x.Text;

        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        Assert.Equal(0, output.Count);

        input.Text = "Foo";
        Assert.Equal(1, output.Count);
        Assert.Equal(input, output[0].Sender);
        Assert.Equal("Text", output[0].GetPropertyName());

        dispose.Dispose();

        input.Text = "Bar";
        Assert.Equal(1, output.Count);
    }

    /// <summary>
    /// Tests that Winform creates observable for property works for components.
    /// </summary>
    [Fact]
    public void WinformsCreatesObservableForPropertyWorksForComponents()
    {
        var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control
        var fixture = new WinformsCreatesObservableForProperty();

        Assert.NotEqual(0, fixture.GetAffinityForObject(typeof(ToolStripButton), "Checked"));

        Expression<Func<ToolStripButton, bool>> expression = x => x.Checked;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        Assert.Equal(0, output.Count);

        input.Checked = true;
        Assert.Equal(1, output.Count);
        Assert.Equal(input, output[0].Sender);
        Assert.Equal("Checked", output[0].GetPropertyName());

        dispose.Dispose();

        // Since we disposed the derived list, we should no longer receive updates
        input.Checked = false;
        Assert.Equal(1, output.Count);
    }

    /// <summary>
    /// Tests that winforms creates observable for property works for third party controls.
    /// </summary>
    [Fact]
    public void WinformsCreatesObservableForPropertyWorksForThirdPartyControls()
    {
        var input = new AThirdPartyNamespace.ThirdPartyControl();
        var fixture = new WinformsCreatesObservableForProperty();

        Assert.NotEqual(0, fixture.GetAffinityForObject(typeof(AThirdPartyNamespace.ThirdPartyControl), "Value"));

        Expression<Func<AThirdPartyNamespace.ThirdPartyControl, string?>> expression = x => x.Value;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        Assert.Equal(0, output.Count);

        input.Value = "Foo";
        Assert.Equal(1, output.Count);
        Assert.Equal(input, output[0].Sender);
        Assert.Equal("Value", output[0].GetPropertyName());

        dispose.Dispose();

        input.Value = "Bar";
        Assert.Equal(1, output.Count);
    }

    /// <summary>
    /// Tests that Winforms controled can bind to View Model.
    /// </summary>
    [Fact]
    public void CanBindViewModelToWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        vm.SomeText = "Foo";
        Assert.NotEqual(vm.SomeText, view.Property3.Text);

        var disp = view.Bind(vm, x => x.SomeText, x => x.Property3.Text);
        vm.SomeText = "Bar";
        Assert.Equal(vm.SomeText, view.Property3.Text);

        view.Property3.Text = "Bar2";
        Assert.Equal(vm.SomeText, view.Property3.Text);

        var disp2 = view.Bind(vm, x => x.SomeDouble, x => x.Property3.Text);
        vm.SomeDouble = 123.4;

        Assert.Equal(vm.SomeDouble.ToString(CultureInfo.CurrentCulture), view.Property3.Text);
    }

    /// <summary>
    /// Smoke tests the WinForm controls.
    /// </summary>
    [Fact]
    public void SmokeTestWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        var disp = new CompositeDisposable(new[]
        {
            view.Bind(vm, x => x.Property1, x => x.Property1.Text),
            view.Bind(vm, x => x.Property2, x => x.Property2.Text),
            view.Bind(vm, x => x.Property3, x => x.Property3.Text),
            view.Bind(vm, x => x.Property4, x => x.Property4.Text),
            view.Bind(vm, x => x.BooleanProperty, x => x.BooleanProperty.Checked),
        });

        vm.Property1 = "FOOO";
        Assert.Equal(vm.Property1, view.Property1.Text);

        vm.Property2 = "FOOO1";
        Assert.Equal(vm.Property2, view.Property2.Text);

        vm.Property3 = "FOOO2";
        Assert.Equal(vm.Property3, view.Property3.Text);

        vm.Property4 = "FOOO3";
        Assert.Equal(vm.Property4, view.Property4.Text);

        vm.BooleanProperty = false;
        Assert.Equal(vm.BooleanProperty, view.BooleanProperty.Checked);
        vm.BooleanProperty = true;
        Assert.Equal(vm.BooleanProperty, view.BooleanProperty.Checked);

        disp.Dispose();
    }

    [Fact]
    public void PanelSetMethodBindingConverter_GetAffinityForObjects()
    {
        var fixture = new PanelSetMethodBindingConverter();
        var test1 = fixture.GetAffinityForObjects(typeof(List<Control>), typeof(Control.ControlCollection));
        var test2 = fixture.GetAffinityForObjects(typeof(List<TextBox>), typeof(Control.ControlCollection));
        var test3 = fixture.GetAffinityForObjects(typeof(List<Label>), typeof(Control.ControlCollection));
        var test4 = fixture.GetAffinityForObjects(typeof(Control.ControlCollection), typeof(IEnumerable<GridItem>));

        Assert.Equal(0, test1);
        Assert.Equal(10, test2);
        Assert.Equal(10, test3);
        Assert.Equal(0, test4);
    }
}
