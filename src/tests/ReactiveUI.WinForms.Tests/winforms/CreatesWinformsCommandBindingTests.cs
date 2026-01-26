// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using ReactiveUI.Winforms;
using ReactiveUI.WinForms.Tests.Winforms.Mocks;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests for <see cref="CreatesWinformsCommandBinding"/>.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class CreatesWinformsCommandBindingTests
{
    /// <summary>
    /// Tests that GetAffinityForObject returns high affinity for WinForms controls.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_WinFormsControl_ReturnsHighAffinity()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<Button>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForObject returns affinity for custom control with Click event.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_CustomControlWithClickEvent_ReturnsHighAffinity()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<CustomClickableControl>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForObject returns 6 when hasEventTarget is true for non-control type.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_NonControlWithEventTarget_Returns6()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<CustomClickableComponent>(hasEventTarget: true);

        await Assert.That(affinity).IsEqualTo(6);
    }

    /// <summary>
    /// Tests that GetAffinityForObject returns 4 for component with Click event but no explicit target.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ComponentWithClickEvent_Returns4()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<CustomClickableComponent>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(4);
    }

    /// <summary>
    /// Tests that GetAffinityForObject returns 0 for type with no matching events.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_TypeWithNoMatchingEvents_Returns0()
    {
        var fixture = new CreatesWinformsCommandBinding();

        var affinity = fixture.GetAffinityForObject<NoClickEventComponent>(hasEventTarget: false);

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    /// Tests that BindCommandToObject throws ArgumentNullException when target is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NullTarget_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });

        var act = () => fixture.BindCommandToObject<Button>(cmd, null!, Observable.Return<object?>(null));

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that BindCommandToObject returns empty disposable when command is null.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NullCommand_ReturnsEmptyDisposable()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var button = new Button();

        var result = fixture.BindCommandToObject<Button>(null, button, Observable.Return<object?>(null));

        await Assert.That(result).IsEqualTo(Disposable.Empty);
    }

    /// <summary>
    /// Tests that BindCommandToObject executes command on button click.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ButtonClick_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        using (fixture.BindCommandToObject(cmd, button, Observable.Return<object?>(null)))
        {
            button.PerformClick();
            await Assert.That(executed).IsTrue();
        }
    }

    /// <summary>
    /// Tests that BindCommandToObject passes command parameter.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithParameter_PassesParameterToCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        object? receivedParam = null;
        var cmd = ReactiveCommand.Create<int>(p => receivedParam = p, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        using (fixture.BindCommandToObject(cmd, button, Observable.Return<object?>(42)))
        {
            button.PerformClick();
            await Assert.That(receivedParam).IsEqualTo(42);
        }
    }

    /// <summary>
    /// Tests that BindCommandToObject updates parameter reactively.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ParameterUpdates_UsesLatestParameter()
    {
        var fixture = new CreatesWinformsCommandBinding();
        object? receivedParam = null;
        var cmd = ReactiveCommand.Create<int>(p => receivedParam = p, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();
        var paramSubject = new BehaviorSubject<object?>(1);

        using (fixture.BindCommandToObject(cmd, button, paramSubject))
        {
            button.PerformClick();
            await Assert.That(receivedParam).IsEqualTo(1);

            paramSubject.OnNext(99);
            button.PerformClick();
            await Assert.That(receivedParam).IsEqualTo(99);
        }
    }

    /// <summary>
    /// Tests that disposing the binding prevents command execution.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_Disposed_PreventsCommandExecution()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executionCount = 0;
        var cmd = ReactiveCommand.Create(() => executionCount++, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        var binding = fixture.BindCommandToObject(cmd, button, Observable.Return<object?>(null));
        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);

        binding?.Dispose();
        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>
    /// Tests that BindCommandToObject returns null for type with no default event.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NoDefaultEvent_ReturnsNull()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });
        var component = new NoClickEventComponent();

        var result = fixture.BindCommandToObject(cmd, component, Observable.Return<object?>(null));

        await Assert.That(result).IsNull();
    }

    /// <summary>
    /// Tests that command does not execute when CanExecute returns false.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_CanExecuteFalse_DoesNotExecuteCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var canExecute = new BehaviorSubject<bool>(false);
        var cmd = ReactiveCommand.Create(() => executed = true, canExecute, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        using (fixture.BindCommandToObject(cmd, button, Observable.Return<object?>(null)))
        {
            button.PerformClick();
            await Assert.That(executed).IsFalse();
        }
    }

    /// <summary>
    /// Tests that AOT-safe generic EventHandler overload binds correctly.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: ImmediateScheduler.Instance);
        var control = new GenericEventControl();

        using (fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            cmd,
            control,
            Observable.Return<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h))
        {
            control.RaiseCustomEvent();
            await Assert.That(executed).IsTrue();
        }
    }

    /// <summary>
    /// Tests that AOT-safe generic EventHandler overload with null command returns empty disposable.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_NullCommand_ReturnsEmptyDisposable()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var control = new GenericEventControl();

        var result = fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            null,
            control,
            Observable.Return<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h);

        await Assert.That(result).IsEqualTo(Disposable.Empty);
    }

    /// <summary>
    /// Tests that AOT-safe generic EventHandler overload throws on null target.
    /// </summary>
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
            Observable.Return<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that AOT-safe generic EventHandler overload throws on null addHandler.
    /// </summary>
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
            Observable.Return<object?>(null),
            null!,
            h => control.CustomEvent -= h);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that AOT-safe generic EventHandler overload throws on null removeHandler.
    /// </summary>
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
            Observable.Return<object?>(null),
            h => control.CustomEvent += h,
            null!);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that disposing the generic EventHandler binding prevents further execution.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_Disposed_PreventsExecution()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executionCount = 0;
        var cmd = ReactiveCommand.Create(() => executionCount++, outputScheduler: ImmediateScheduler.Instance);
        var control = new GenericEventControl();

        var binding = fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(
            cmd,
            control,
            Observable.Return<object?>(null),
            h => control.CustomEvent += h,
            h => control.CustomEvent -= h);

        control.RaiseCustomEvent();
        await Assert.That(executionCount).IsEqualTo(1);

        binding?.Dispose();

        control.RaiseCustomEvent();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>
    /// Tests that generic EventHandler binds Enabled property for Components.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_GenericEventHandler_Component_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSubject<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: ImmediateScheduler.Instance);
        var component = new EnabledComponent();

        using (fixture.BindCommandToObject<EnabledComponent, CustomEventArgs>(
            cmd,
            component,
            Observable.Return<object?>(null),
            h => component.CustomEvent += h,
            h => component.CustomEvent -= h))
        {
            await Assert.That(component.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(component.Enabled).IsFalse();

            canExecute.OnNext(true);
            await Assert.That(component.Enabled).IsTrue();
        }
    }

    /// <summary>
    /// Tests that AOT-safe non-generic EventHandler overload binds correctly.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        using (fixture.BindCommandToObject(
            cmd,
            button,
            Observable.Return<object?>(null),
            h => button.Click += h,
            h => button.Click -= h))
        {
            button.PerformClick();
            await Assert.That(executed).IsTrue();
        }
    }

    /// <summary>
    /// Tests that AOT-safe non-generic EventHandler overload with null command returns empty disposable.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_NullCommand_ReturnsEmptyDisposable()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var button = new Button();

        var result = fixture.BindCommandToObject(
            null,
            button,
            Observable.Return<object?>(null),
            h => button.Click += h,
            h => button.Click -= h);

        await Assert.That(result).IsEqualTo(Disposable.Empty);
    }

    /// <summary>
    /// Tests that AOT-safe non-generic EventHandler overload throws on null target.
    /// </summary>
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
            Observable.Return<object?>(null),
            h => button.Click += h,
            h => button.Click -= h);

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that disposing the non-generic EventHandler binding prevents further execution.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_Disposed_PreventsExecution()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executionCount = 0;
        var cmd = ReactiveCommand.Create(() => executionCount++, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        var binding = fixture.BindCommandToObject(
            cmd,
            button,
            Observable.Return<object?>(null),
            h => button.Click += h,
            h => button.Click -= h);

        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);

        binding?.Dispose();

        button.PerformClick();
        await Assert.That(executionCount).IsEqualTo(1);
    }

    /// <summary>
    /// Tests that non-generic EventHandler binds Enabled property for Components.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_Component_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSubject<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: ImmediateScheduler.Instance);
        var component = new CustomClickableComponentWithEnabled();

        using (fixture.BindCommandToObject(
            cmd,
            component,
            Observable.Return<object?>(null),
            h => component.Click += h,
            h => component.Click -= h))
        {
            await Assert.That(component.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(component.Enabled).IsFalse();

            canExecute.OnNext(true);
            await Assert.That(component.Enabled).IsTrue();
        }
    }

    /// <summary>
    /// Tests that non-generic EventHandler passes command parameter correctly.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_NonGenericEventHandler_PassesParameter()
    {
        var fixture = new CreatesWinformsCommandBinding();
        object? receivedParam = null;
        var cmd = ReactiveCommand.Create<int>(p => receivedParam = p, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        using (fixture.BindCommandToObject(
            cmd,
            button,
            Observable.Return<object?>(123),
            h => button.Click += h,
            h => button.Click -= h))
        {
            button.PerformClick();
            await Assert.That(receivedParam).IsEqualTo(123);
        }
    }

    /// <summary>
    /// Tests that BindCommandToObject with event name binds correctly.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: ImmediateScheduler.Instance);
        var control = new CustomClickableControl();

        using (fixture.BindCommandToObject<CustomClickableControl, System.Windows.Forms.MouseEventArgs>(cmd, control, Observable.Return<object?>(null), "MouseUp"))
        {
            control.RaiseMouseUpEvent(new System.Windows.Forms.MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            await Assert.That(executed).IsTrue();
        }
    }

    /// <summary>
    /// Tests that BindCommandToObject with event name throws on null command.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_NullCommand_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var control = new CustomClickableControl();

        var act = () => fixture.BindCommandToObject<CustomClickableControl, System.Windows.Forms.MouseEventArgs>(null!, control, Observable.Return<object?>(null), "MouseUp");

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that BindCommandToObject with event name throws on null target.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_NullTarget_ThrowsArgumentNullException()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create(() => { });

        var act = () => fixture.BindCommandToObject<CustomClickableControl, System.Windows.Forms.MouseEventArgs>(cmd, null!, Observable.Return<object?>(null), "MouseUp");

        await Assert.That(act).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that BindCommandToObject with event name binds Enabled for Components.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_Component_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSubject<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: ImmediateScheduler.Instance);
        var toolStripButton = new ToolStripButton();

        using (fixture.BindCommandToObject<ToolStripButton, EventArgs>(cmd, toolStripButton, Observable.Return<object?>(null), "Click"))
        {
            await Assert.That(toolStripButton.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(toolStripButton.Enabled).IsFalse();
        }
    }

    /// <summary>
    /// Tests that initial Enabled state is set correctly based on CanExecute.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_InitialCanExecuteFalse_SetsEnabledFalse()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSubject<bool>(false);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: ImmediateScheduler.Instance);
        var button = new Button();

        using (fixture.BindCommandToObject(cmd, button, Observable.Return<object?>(null)))
        {
            await Assert.That(button.Enabled).IsFalse();
        }
    }

    /// <summary>
    /// Tests that ToolStripButton (Component with Enabled property) is properly controlled.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ToolStripButton_BindsEnabledProperty()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new BehaviorSubject<bool>(true);
        var cmd = ReactiveCommand.Create(() => { }, canExecute, outputScheduler: ImmediateScheduler.Instance);
        var toolStripButton = new ToolStripButton();

        using (fixture.BindCommandToObject(cmd, toolStripButton, Observable.Return<object?>(null)))
        {
            await Assert.That(toolStripButton.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(toolStripButton.Enabled).IsFalse();

            canExecute.OnNext(true);
            await Assert.That(toolStripButton.Enabled).IsTrue();
        }
    }

    /// <summary>
    /// Tests that component with Click event binds correctly via reflection fallback.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task BindCommandToObject_ComponentWithClickEvent_ExecutesCommand()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create(() => executed = true, outputScheduler: ImmediateScheduler.Instance);
        var component = new CustomClickableComponent();

        using (fixture.BindCommandToObject(cmd, component, Observable.Return<object?>(null)))
        {
            component.PerformClick();
            await Assert.That(executed).IsTrue();
        }
    }
}
