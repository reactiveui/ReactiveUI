// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
[TestFixture]
public class DefaultPropertyBindingTests
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultPropertyBindingTests"/> class.
    /// </summary>
    public DefaultPropertyBindingTests()
    {
        RxApp.EnsureInitialized();
    }

    /// <summary>
    /// Tests Winforms creates observable for property works for textboxes.
    /// </summary>
    [Test]
    public void WinformsCreatesObservableForPropertyWorksForTextboxes()
    {
        var input = new TextBox();
        var fixture = new WinformsCreatesObservableForProperty();

        Assert.That(fixture.GetAffinityForObject(typeof(TextBox), "Text"), Is.Not.Zero);

        Expression<Func<TextBox, string>> expression = x => x.Text;

        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        Assert.That(output, Is.Empty);

        input.Text = "Foo";
        Assert.That(output, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(output[0].Sender, Is.EqualTo(input));
            Assert.That(output[0].GetPropertyName(), Is.EqualTo("Text"));
        }

        dispose.Dispose();

        input.Text = "Bar";
        Assert.That(output, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that Winform creates observable for property works for components.
    /// </summary>
    [Test]
    public void WinformsCreatesObservableForPropertyWorksForComponents()
    {
        var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control
        var fixture = new WinformsCreatesObservableForProperty();

        Assert.That(fixture.GetAffinityForObject(typeof(ToolStripButton), "Checked"), Is.Not.Zero);

        Expression<Func<ToolStripButton, bool>> expression = x => x.Checked;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        Assert.That(output, Is.Empty);

        input.Checked = true;
        Assert.That(output, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(output[0].Sender, Is.EqualTo(input));
            Assert.That(output[0].GetPropertyName(), Is.EqualTo("Checked"));
        }

        dispose.Dispose();

        // Since we disposed the derived list, we should no longer receive updates
        input.Checked = false;
        Assert.That(output, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that winforms creates observable for property works for third party controls.
    /// </summary>
    [Test]
    public void WinformsCreatesObservableForPropertyWorksForThirdPartyControls()
    {
        var input = new AThirdPartyNamespace.ThirdPartyControl();
        var fixture = new WinformsCreatesObservableForProperty();

        Assert.That(fixture.GetAffinityForObject(typeof(AThirdPartyNamespace.ThirdPartyControl), "Value"), Is.Not.Zero);

        Expression<Func<AThirdPartyNamespace.ThirdPartyControl, string?>> expression = x => x.Value;
        var propertyName = expression.Body.GetMemberInfo()?.Name ?? throw new InvalidOperationException("propertyName should not be null.");
        var dispose = fixture.GetNotificationForProperty(input, expression.Body, propertyName).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var output).Subscribe();
        Assert.That(output, Is.Empty);

        input.Value = "Foo";
        Assert.That(output, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(output[0].Sender, Is.EqualTo(input));
            Assert.That(output[0].GetPropertyName(), Is.EqualTo("Value"));
        }

        dispose.Dispose();

        input.Value = "Bar";
        Assert.That(output, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that Winforms controled can bind to View Model.
    /// </summary>
    [Test]
    public void CanBindViewModelToWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        vm.SomeText = "Foo";
        Assert.That(view.Property3.Text, Is.Not.EqualTo(vm.SomeText));

        var disp = view.Bind(vm, x => x.SomeText, x => x.Property3.Text);
        vm.SomeText = "Bar";
        Assert.That(view.Property3.Text, Is.EqualTo(vm.SomeText));

        view.Property3.Text = "Bar2";
        Assert.That(view.Property3.Text, Is.EqualTo(vm.SomeText));

        var disp2 = view.Bind(vm, x => x.SomeDouble, x => x.Property3.Text);
        vm.SomeDouble = 123.4;

        Assert.That(view.Property3.Text, Is.EqualTo(vm.SomeDouble.ToString(CultureInfo.CurrentCulture)));
    }

    /// <summary>
    /// Smoke tests the WinForm controls.
    /// </summary>
    [Test]
    public void SmokeTestWinformControls()
    {
        var vm = new FakeWinformViewModel();
        var view = new FakeWinformsView { ViewModel = vm };

        var disp = new CompositeDisposable(
        [
            view.Bind(vm, x => x.Property1, x => x.Property1.Text),
            view.Bind(vm, x => x.Property2, x => x.Property2.Text),
            view.Bind(vm, x => x.Property3, x => x.Property3.Text),
            view.Bind(vm, x => x.Property4, x => x.Property4.Text),
            view.Bind(vm, x => x.BooleanProperty, x => x.BooleanProperty.Checked),
        ]);

        vm.Property1 = "FOOO";
        Assert.That(view.Property1.Text, Is.EqualTo(vm.Property1));

        vm.Property2 = "FOOO1";
        Assert.That(view.Property2.Text, Is.EqualTo(vm.Property2));

        vm.Property3 = "FOOO2";
        Assert.That(view.Property3.Text, Is.EqualTo(vm.Property3));

        vm.Property4 = "FOOO3";
        Assert.That(view.Property4.Text, Is.EqualTo(vm.Property4));

        vm.BooleanProperty = false;
        Assert.That(view.BooleanProperty.Checked, Is.EqualTo(vm.BooleanProperty));
        vm.BooleanProperty = true;
        Assert.That(view.BooleanProperty.Checked, Is.EqualTo(vm.BooleanProperty));

        disp.Dispose();
    }

    [Test]
    public void PanelSetMethodBindingConverter_GetAffinityForObjects()
    {
        var fixture = new PanelSetMethodBindingConverter();
        var test1 = fixture.GetAffinityForObjects(typeof(List<Control>), typeof(Control.ControlCollection));
        var test2 = fixture.GetAffinityForObjects(typeof(List<TextBox>), typeof(Control.ControlCollection));
        var test3 = fixture.GetAffinityForObjects(typeof(List<Label>), typeof(Control.ControlCollection));
        var test4 = fixture.GetAffinityForObjects(typeof(Control.ControlCollection), typeof(IEnumerable<GridItem>));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(test1, Is.Zero);
            Assert.That(test2, Is.EqualTo(10));
            Assert.That(test3, Is.EqualTo(10));
            Assert.That(test4, Is.Zero);
        }
    }
}
