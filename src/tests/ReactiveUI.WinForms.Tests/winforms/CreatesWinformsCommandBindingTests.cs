// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for <see cref="CreatesWinformsCommandBinding"/>.</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class CreatesWinformsCommandBindingTests
{
    /// <summary>The affinity returned for WinForms controls.</summary>
    private const int HighAffinity = 10;

    /// <summary>The affinity returned for objects exposing a matching event.</summary>
    private const int EventTargetAffinity = 6;

    /// <summary>The affinity returned for component-derived objects.</summary>
    private const int ComponentAffinity = 4;

    /// <summary>The affinity returned when no binding is possible.</summary>
    private const int NoAffinity = 0;

    /// <summary>A command parameter value used by the tests.</summary>
    private const int ParameterValue42 = 42;

    /// <summary>A command parameter value used by the tests.</summary>
    private const int ParameterValue99 = 99;

    /// <summary>A command parameter value used by the tests.</summary>
    private const int ParameterValue123 = 123;

    /// <summary>Tests that GetAffinityForObject returns high affinity for WinForms controls.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_WinFormsControl_ReturnsHighAffinity()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<Button>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(HighAffinity);
    }

    /// <summary>Tests that GetAffinityForObject returns affinity for custom control with Click event.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_CustomControlWithClickEvent_ReturnsHighAffinity()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<CustomClickableControl>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(HighAffinity);
    }

    /// <summary>Tests that GetAffinityForObject returns 6 when hasEventTarget is true for non-control type.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_NonControlWithEventTarget_Returns6()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<CustomClickableComponent>(hasEventTarget: true);

        await Assert.That(affinity).IsEqualTo(EventTargetAffinity);
    }

    /// <summary>Tests that GetAffinityForObject returns 4 for component with Click event but no explicit target.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ComponentWithClickEvent_Returns4()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<CustomClickableComponent>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(ComponentAffinity);
    }

    /// <summary>Tests that GetAffinityForObject returns 0 for type with no matching events.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_TypeWithNoMatchingEvents_Returns0()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<NoClickEventComponent>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(NoAffinity);
    }

    /// <summary>Tests that BindCommandToObject throws ArgumentNullException when target is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NullTarget_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });

        var act = () => fixture.BindCommandToObject<Button>(cmd, null!, Signal.Emit<object?>(null));

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that BindCommandToObject returns empty disposable when command is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NullCommand_ReturnsEmptyDisposable()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var button = new Button();

        var result = fixture.BindCommandToObject<Button>(null, button, Signal.Emit<object?>(null));

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Tests that BindCommandToObject executes command on button click.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ButtonClick_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        using var binding = fixture.BindCommandToObject(cmd, button, Signal.Emit<object?>(null));
        button.PerformClick();
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Tests that BindCommandToObject passes command parameter.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithParameter_PassesParameterToCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        object? receivedParam = null;
        var cmd = ReactiveCommand.Create<int>(p => receivedParam = p, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        using var binding = fixture.BindCommandToObject(cmd, button, Signal.Emit<object?>(ParameterValue42));
        button.PerformClick();
        await Assert.That(receivedParam).IsEqualTo(ParameterValue42);
    }

    /// <summary>Tests that BindCommandToObject updates parameter reactively.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ParameterUpdates_UsesLatestParameter()
    {
        var fixture = new CreatesWinformsCommandBinding();
        object? receivedParam = null;
        var cmd = ReactiveCommand.Create<int>(p => receivedParam = p, outputScheduler: Sequencer.Immediate);
        var button = new Button();
        var paramSubject = new BehaviorSignal<object?>(1);

        using var binding = fixture.BindCommandToObject(cmd, button, paramSubject);
        button.PerformClick();
        await Assert.That(receivedParam).IsEqualTo(1);

        paramSubject.OnNext(ParameterValue99);
        button.PerformClick();
        await Assert.That(receivedParam).IsEqualTo(ParameterValue99);
    }

    /// <summary>Tests that disposing the binding prevents command execution.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_Disposed_PreventsCommandExecution()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executionCount = 0;
        var cmd = ReactiveCommand.Create(() => executionCount++, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        var binding = fixture.BindCommandToObject(cmd, button, Signal.Emit<object?>(null));
        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);

        binding?.Dispose();
        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>Tests that BindCommandToObject returns null for type with no default event.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NoDefaultEvent_ReturnsNull()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });
        var component = new NoClickEventComponent();

        var result = fixture.BindCommandToObject(cmd, component, Signal.Emit<object?>(null));

        await Assert.That(result).IsNull();
    }

    /// <summary>Tests that command does not execute when CanExecute returns false.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_CanExecuteFalse_DoesNotExecuteCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var canExecute = new BehaviorSignal<bool>(false);
        var cmd = ReactiveCommand.Create(() => executed = true, canExecute, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        using var binding = fixture.BindCommandToObject(cmd, button, Signal.Emit<object?>(null));
        button.PerformClick();
        await Assert.That(executed).IsFalse();
    }

    /// <summary>Tests that AOT-safe generic EventHandler overload binds correctly.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: Sequencer.Immediate);
        var control = new GenericEventControl();

        using var binding = fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            cmd,
            control,
            Signal.Emit<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h);
        control.RaiseCustomEvent();
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Tests that AOT-safe generic EventHandler overload with null command returns empty disposable.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_NullCommand_ReturnsEmptyDisposable()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var control = new GenericEventControl();

        var result = fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            null,
            control,
            Signal.Emit<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Tests that AOT-safe generic EventHandler overload throws on null target.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_NullTarget_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });
        var control = new GenericEventControl();

        var act = () => fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            cmd,
            null!,
            Signal.Emit<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that AOT-safe generic EventHandler overload throws on null addHandler.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_NullAddHandler_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });
        var control = new GenericEventControl();

        var act = () => fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            cmd,
            control,
            Signal.Emit<object?>(null),
            null!,
            h => control.CustomEvent -= h);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that AOT-safe generic EventHandler overload throws on null removeHandler.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_NullRemoveHandler_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });
        var control = new GenericEventControl();

        var act = () => fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            cmd,
            control,
            Signal.Emit<object?>(null),
            h => control.CustomEvent += h,
            null!);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that disposing the generic EventHandler binding prevents further execution.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_Disposed_PreventsExecution()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executionCount = 0;
        var cmd = ReactiveCommand.Create(() => executionCount++, outputScheduler: Sequencer.Immediate);
        var control = new GenericEventControl();

        var binding = fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            cmd,
            control,
            Signal.Emit<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h);

        control.RaiseCustomEvent();
        await Assert.That(executionCount).IsEqualTo(1);

        binding?.Dispose();

        control.RaiseCustomEvent();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>Tests that generic EventHandler binds Enabled property for Components.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_Component_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSignal<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: Sequencer.Immediate);
        var component = new EnabledComponent();

        using var binding = fixture.BindCommandToObject<EnabledComponent, CustomEventArgs>(
            cmd,
            component,
            Signal.Emit<object?>(null),
            h => component.CustomEvent += h,
            h => component.CustomEvent -= h);
        await Assert.That(component.Enabled).IsTrue();

        canExecute.OnNext(false);
        await Assert.That(component.Enabled).IsFalse();

        canExecute.OnNext(true);
        await Assert.That(component.Enabled).IsTrue();
    }

    /// <summary>Tests that AOT-safe non-generic EventHandler overload binds correctly.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        using var binding = fixture.BindCommandToObject(
            cmd,
            button,
            Signal.Emit<object?>(null),
            h => button.Click += h,
            h => button.Click -= h);
        button.PerformClick();
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Tests that AOT-safe non-generic EventHandler overload with null command returns empty disposable.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_NullCommand_ReturnsEmptyDisposable()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var button = new Button();

        var result = fixture.BindCommandToObject(
            null,
            button,
            Signal.Emit<object?>(null),
            h => button.Click += h,
            h => button.Click -= h);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Tests that AOT-safe non-generic EventHandler overload throws on null target.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_NullTarget_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });
        var button = new Button();

        var act = () => fixture.BindCommandToObject<Button>(
            cmd,
            null!,
            Signal.Emit<object?>(null),
            h => button.Click += h,
            h => button.Click -= h);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that disposing the non-generic EventHandler binding prevents further execution.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_Disposed_PreventsExecution()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executionCount = 0;
        var cmd = ReactiveCommand.Create(() => executionCount++, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        var binding = fixture.BindCommandToObject(
            cmd,
            button,
            Signal.Emit<object?>(null),
            h => button.Click += h,
            h => button.Click -= h);

        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);

        binding?.Dispose();

        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>Tests that non-generic EventHandler binds Enabled property for Components.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_Component_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSignal<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: Sequencer.Immediate);
        var component = new CustomClickableComponentWithEnabled();

        using var binding = fixture.BindCommandToObject(
            cmd,
            component,
            Signal.Emit<object?>(null),
            h => component.Click += h,
            h => component.Click -= h);
        await Assert.That(component.Enabled).IsTrue();

        canExecute.OnNext(false);
        await Assert.That(component.Enabled).IsFalse();

        canExecute.OnNext(true);
        await Assert.That(component.Enabled).IsTrue();
    }

    /// <summary>Tests that non-generic EventHandler passes command parameter correctly.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_PassesParameter()
    {
        var fixture = new CreatesWinformsCommandBinding();
        object? receivedParam = null;
        var cmd = ReactiveCommand.Create<int>(p => receivedParam = p, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        using var binding = fixture.BindCommandToObject(
            cmd,
            button,
            Signal.Emit<object?>(ParameterValue123),
            h => button.Click += h,
            h => button.Click -= h);
        button.PerformClick();
        await Assert.That(receivedParam).IsEqualTo(ParameterValue123);
    }

    /// <summary>Tests that BindCommandToObject with event name binds correctly.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: Sequencer.Immediate);
        var control = new CustomClickableControl();

        using var binding = fixture.BindCommandToObject<CustomClickableControl, MouseEventArgs>(cmd, control, Signal.Emit<object?>(null), "MouseUp");
        control.RaiseMouseUpEvent(new(MouseButtons.Left, 1, 0, 0, 0));
        await Assert.That(executed).IsTrue();
    }

    /// <summary>Tests that BindCommandToObject with event name throws on null command.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_NullCommand_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var control = new CustomClickableControl();

        var act = () => fixture.BindCommandToObject<CustomClickableControl, MouseEventArgs>(null!, control, Signal.Emit<object?>(null), "MouseUp");

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that BindCommandToObject with event name throws on null target.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_NullTarget_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });

        var act = () => fixture.BindCommandToObject<CustomClickableControl, MouseEventArgs>(cmd, null!, Signal.Emit<object?>(null), "MouseUp");

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>Tests that BindCommandToObject with event name binds Enabled for Components.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_Component_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSignal<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: Sequencer.Immediate);
        var toolStripButton = new ToolStripButton();

        using var binding = fixture.BindCommandToObject<ToolStripButton, EventArgs>(cmd, toolStripButton, Signal.Emit<object?>(null), "Click");
        await Assert.That(toolStripButton.Enabled).IsTrue();

        canExecute.OnNext(false);
        await Assert.That(toolStripButton.Enabled).IsFalse();
    }

    /// <summary>Tests that initial Enabled state is set correctly based on CanExecute.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_InitialCanExecuteFalse_SetsEnabledFalse()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSignal<bool>(false);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: Sequencer.Immediate);
        var button = new Button();

        using var binding = fixture.BindCommandToObject(cmd, button, Signal.Emit<object?>(null));
        await Assert.That(button.Enabled).IsFalse();
    }

    /// <summary>Tests that ToolStripButton (Component with Enabled property) is properly controlled.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ToolStripButton_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSignal<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: Sequencer.Immediate);
        var toolStripButton = new ToolStripButton();

        using var binding = fixture.BindCommandToObject(cmd, toolStripButton, Signal.Emit<object?>(null));
        await Assert.That(toolStripButton.Enabled).IsTrue();

        canExecute.OnNext(false);
        await Assert.That(toolStripButton.Enabled).IsFalse();

        canExecute.OnNext(true);
        await Assert.That(toolStripButton.Enabled).IsTrue();
    }

    /// <summary>Tests that component with Click event binds correctly via reflection fallback.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ComponentWithClickEvent_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: Sequencer.Immediate);
        var component = new CustomClickableComponent();

        using var binding = fixture.BindCommandToObject(cmd, component, Signal.Emit<object?>(null));
        component.PerformClick();
        await Assert.That(executed).IsTrue();
    }
}
