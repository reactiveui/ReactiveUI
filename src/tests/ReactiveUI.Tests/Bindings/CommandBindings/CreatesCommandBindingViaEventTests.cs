// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Bindings.CommandBindings;

/// <summary>Tests for <see cref="CreatesCommandBindingViaEvent"/> event-driven command binding behavior.</summary>
public class CreatesCommandBindingViaEventTests
{
    /// <summary>Verifies that the command is no longer executed after the binding is disposed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_AfterDispose_DoesNotExecuteCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using (var binding = binder.BindCommandToObject(command, target, Signal.Emit<object?>(null)))
        {
            // Binding is active
        }

        target.RaiseClick();
        await Assert.That(wasCalled).IsFalse();
    }

    /// <summary>Verifies that the command's CanExecute state gates execution from the event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_ChecksCanExecute()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var executionCount = 0;
        var canExecute = new BehaviorSignal<bool>(true);
        var command = ReactiveCommand.Create(() => executionCount++, canExecute);

        using var binding = binder.BindCommandToObject(command, target, Signal.Emit<object?>(null));

        target.RaiseClick();
        await Assert.That(executionCount).IsEqualTo(1);

        canExecute.OnNext(false);
        target.RaiseClick();
        await Assert.That(executionCount).IsEqualTo(1); // Should not execute when CanExecute is false
    }

    /// <summary>Verifies that multiple event raises execute the command multiple times.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_MultipleClicks_ExecutesMultipleTimes()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var executionCount = 0;
        var command = ReactiveCommand.Create(() => executionCount++, outputScheduler: Sequencer.Immediate);

        using var binding = binder.BindCommandToObject(command, target, Signal.Emit<object?>(null));

        target.RaiseClick();
        target.RaiseClick();
        target.RaiseClick();

        const int ExpectedExecutionCount = 3;
        await Assert.That(executionCount).IsEqualTo(ExpectedExecutionCount);
    }

    /// <summary>Verifies that the command receives the latest parameter value when the event fires.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_UpdatesParameter_UsesLatestParameter()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        object? receivedParameter = null;
        var command = ReactiveCommand.Create((Action<object?>)(param => receivedParameter = param));
        var parameter = new BehaviorSignal<object?>("first");

        using var binding = binder.BindCommandToObject(command, target, parameter);

        parameter.OnNext("second");
        target.RaiseClick();
        await Assert.That(receivedParameter).IsEqualTo("second");
    }

    /// <summary>Verifies that raising the Click event executes the bound command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithClickEvent_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using var binding = binder.BindCommandToObject(command, target, Signal.Emit<object?>(null));

        target.RaiseClick();
        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that binding to an explicitly named event executes the command when that event fires.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithExplicitEvent_BindsToSpecifiedEvent()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using var binding = binder.BindCommandToObject<ClickableControl, EventArgs>(
            command,
            target,
            Signal.Emit<object?>(null),
            "Click");

        target.RaiseClick();
        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that raising the MouseUp event executes the bound command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithMouseUpEvent_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new MouseUpControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using var binding = binder.BindCommandToObject(command, target, Signal.Emit<object?>(null));

        target.RaiseMouseUp();
        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that binding to a target with no suitable events throws an exception.</summary>
    [Test]
    public void BindCommandToObject_WithNoEvents_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new object();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);

        Assert.Throws<Exception>(() =>
            binder.BindCommandToObject(command, target, Signal.Emit<object?>(null)));
    }

    /// <summary>Verifies that binding to a null target throws an <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void BindCommandToObject_WithNullTarget_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControl>(command, null, Signal.Emit<object?>(null)));
    }

    /// <summary>Verifies that the configured parameter is passed to the command when the event fires.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithParameter_PassesParameterToCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        object? receivedParameter = null;
        var command = ReactiveCommand.Create(
            (Action<object?>)(param => receivedParameter = param),
            outputScheduler: Sequencer.Immediate);
        var parameter = new BehaviorSignal<object?>("test");

        using var binding = binder.BindCommandToObject(command, target, parameter);

        target.RaiseClick();
        await Assert.That(receivedParameter).IsEqualTo("test");
    }

    /// <summary>Verifies that the generic affinity check returns 3 for a target exposing a Click event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_Generic_WithClickEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(false);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultEvent);
    }

    /// <summary>Verifies that the generic affinity check returns 5 when an event target is requested.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_Generic_WithEventTarget_Returns5()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(true);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.Explicit);
    }

    /// <summary>Verifies that the affinity check returns 3 for a target exposing a Click event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task GetAffinityForObject_WithClickEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(false);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultEvent);
    }

    /// <summary>Verifies that the affinity check returns 5 when an event target is requested.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task GetAffinityForObject_WithEventTarget_Returns5()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(true);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.Explicit);
    }

    /// <summary>Verifies that the affinity check returns 3 for a target exposing a MouseUp event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_WithMouseUpEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<MouseUpControl>(false);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultEvent);
    }

    /// <summary>Verifies that the affinity check returns 0 for a target with no suitable events.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_WithNoEvents_Returns0()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<string>(false);
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Verifies that binding a null command returns a non-null, disposable result.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithNullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();

        var binding = binder.BindCommandToObject(null, target, Signal.Emit<object?>(null));

        await Assert.That(binding).IsNotNull();
        binding?.Dispose(); // Should not throw
    }

    /// <summary>Verifies that binding a null command with an explicit event name returns a non-null, disposable result.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithExplicitEventAndNullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();

        var binding = binder.BindCommandToObject<ClickableControl, EventArgs>(
            null,
            target,
            Signal.Emit<object?>(null),
            "Click");

        await Assert.That(binding).IsNotNull();
        binding?.Dispose(); // Should not throw
    }

    /// <summary>Verifies that binding with a null event name throws an <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void BindCommandToObject_WithNullEventName_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControl, EventArgs>(
                command,
                target,
                Signal.Emit<object?>(null),
                null!));
    }

    /// <summary>Verifies that binding with an empty event name throws an <see cref="ArgumentException"/>.</summary>
    [Test]
    public void BindCommandToObject_WithEmptyEventName_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);

        Assert.Throws<ArgumentException>(() =>
            binder.BindCommandToObject<ClickableControl, EventArgs>(
                command,
                target,
                Signal.Emit<object?>(null),
                string.Empty));
    }

    /// <summary>Verifies that binding via explicit add/remove handler delegates executes the command on the event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
            command,
            target,
            Signal.Emit<object?>(null),
            handler => target.GenericClick += handler,
            handler => target.GenericClick -= handler);

        target.RaiseGenericClick();
        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that, after disposal, an add/remove handler binding no longer executes the command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_AfterDispose_DoesNotExecute()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using (var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
                   command,
                   target,
                   Signal.Emit<object?>(null),
                   handler => target.GenericClick += handler,
                   handler => target.GenericClick -= handler))
        {
            // Binding active
        }

        target.RaiseGenericClick();
        await Assert.That(wasCalled).IsFalse();
    }

    /// <summary>Verifies that an add/remove handler binding passes the configured parameter to the command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_PassesParameter()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        object? receivedParameter = null;
        var command = ReactiveCommand.Create(
            (Action<object?>)(param => receivedParameter = param),
            outputScheduler: Sequencer.Immediate);
        var parameter = new BehaviorSignal<object?>("testParam");

        using var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
            command,
            target,
            parameter,
            handler => target.GenericClick += handler,
            handler => target.GenericClick -= handler);

        target.RaiseGenericClick();
        await Assert.That(receivedParameter).IsEqualTo("testParam");
    }

    /// <summary>Verifies that an add/remove handler binding with a null command returns a non-null, disposable result.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_NullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();

        var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
            null,
            target,
            Signal.Emit<object?>(null),
            handler => target.GenericClick += handler,
            handler => target.GenericClick -= handler);

        await Assert.That(binding).IsNotNull();
        binding.Dispose(); // Should not throw
    }

    /// <summary>Verifies that an add/remove handler binding with a null target throws an <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void BindCommandToObject_WithAddRemoveHandlers_NullTarget_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
                command,
                null,
                Signal.Emit<object?>(null),
                handler => { },
                handler => { }));
    }

    /// <summary>Verifies that an add/remove handler binding with a null add handler throws an <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void BindCommandToObject_WithAddRemoveHandlers_NullAddHandler_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
                command,
                target,
                Signal.Emit<object?>(null),
                null!,
                handler => { }));
    }

    /// <summary>Verifies that an add/remove handler binding with a null remove handler throws an <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void BindCommandToObject_WithAddRemoveHandlers_NullRemoveHandler_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: Sequencer.Immediate);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
                command,
                target,
                Signal.Emit<object?>(null),
                handler => { },
                null!));
    }

    /// <summary>Verifies that the EventHandler-based add/remove overload executes the command on the event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventHandlerOverload_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using var binding = binder.BindCommandToObject(
            command,
            target,
            Signal.Emit<object?>(null),
            handler => target.Click += handler,
            handler => target.Click -= handler);

        target.RaiseClick();
        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that the EventHandler-based overload with a null command returns a non-null, disposable result.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventHandlerOverload_NullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();

        var binding = binder.BindCommandToObject(
            null,
            target,
            Signal.Emit<object?>(null),
            handler => target.Click += handler,
            handler => target.Click -= handler);

        await Assert.That(binding).IsNotNull();
        binding.Dispose(); // Should not throw
    }

    /// <summary>Verifies that raising the TouchUpInside event executes the bound command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithTouchUpInsideEvent_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new TouchUpInsideControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: Sequencer.Immediate);

        using var binding = binder.BindCommandToObject(command, target, Signal.Emit<object?>(null));

        target.RaiseTouchUpInside();
        await Assert.That(wasCalled).IsTrue();
    }

    /// <summary>Verifies that the affinity check returns 3 for a target exposing a TouchUpInside event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_WithTouchUpInsideEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<TouchUpInsideControl>(false);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.DefaultEvent);
    }

    /// <summary>Test control exposing a Click event.</summary>
    private sealed class ClickableControl
    {
        /// <summary>Occurs when the control is clicked.</summary>
        public event EventHandler? Click;

        /// <summary>Raises the <see cref="Click"/> event.</summary>
        public void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Test control exposing a MouseUp event.</summary>
    private sealed class MouseUpControl
    {
        /// <summary>Occurs when the mouse button is released over the control.</summary>
        public event EventHandler? MouseUp;

        /// <summary>Raises the <see cref="MouseUp"/> event.</summary>
        public void RaiseMouseUp() => MouseUp?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Test control exposing a TouchUpInside event.</summary>
    private sealed class TouchUpInsideControl
    {
        /// <summary>Occurs when a touch is released inside the control.</summary>
        public event EventHandler? TouchUpInside;

        /// <summary>Raises the <see cref="TouchUpInside"/> event.</summary>
        public void RaiseTouchUpInside() => TouchUpInside?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Test control exposing a strongly typed generic click event.</summary>
    private sealed class ClickableControlWithGenericEvent
    {
        /// <summary>Occurs when the control is clicked.</summary>
        public event EventHandler<EventArgs>? GenericClick;

        /// <summary>Raises the <see cref="GenericClick"/> event.</summary>
        public void RaiseGenericClick() => GenericClick?.Invoke(this, EventArgs.Empty);
    }
}
