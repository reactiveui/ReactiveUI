// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.CommandBindings;

public class CreatesCommandBindingViaEventTests
{
    [Test]
    public async Task BindCommandToObject_AfterDispose_DoesNotExecuteCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using (var binding = binder.BindCommandToObject(command, target, Observable.Return<object?>(null)))
        {
            // Binding is active
        }

        target.RaiseClick();
        await Assert.That(wasCalled).IsFalse();
    }

    [Test]
    public async Task BindCommandToObject_ChecksCanExecute()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var executionCount = 0;
        var canExecute = new BehaviorSubject<bool>(true);
        var command = ReactiveCommand.Create(() => executionCount++, canExecute);

        using var binding = binder.BindCommandToObject(command, target, Observable.Return<object?>(null));

        target.RaiseClick();
        await Assert.That(executionCount).IsEqualTo(1);

        canExecute.OnNext(false);
        target.RaiseClick();
        await Assert.That(executionCount).IsEqualTo(1); // Should not execute when CanExecute is false
    }

    [Test]
    public async Task BindCommandToObject_MultipleClicks_ExecutesMultipleTimes()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var executionCount = 0;
        var command = ReactiveCommand.Create(() => executionCount++, outputScheduler: ImmediateScheduler.Instance);

        using var binding = binder.BindCommandToObject(command, target, Observable.Return<object?>(null));

        target.RaiseClick();
        target.RaiseClick();
        target.RaiseClick();

        await Assert.That(executionCount).IsEqualTo(3);
    }

    [Test]
    public async Task BindCommandToObject_UpdatesParameter_UsesLatestParameter()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        object? receivedParameter = null;
        var command = ReactiveCommand.Create<object?>(param => receivedParameter = param);
        var parameter = new BehaviorSubject<object?>("first");

        using var binding = binder.BindCommandToObject(command, target, parameter);

        parameter.OnNext("second");
        target.RaiseClick();
        await Assert.That(receivedParameter).IsEqualTo("second");
    }

    [Test]
    public async Task BindCommandToObject_WithClickEvent_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using var binding = binder.BindCommandToObject(command, target, Observable.Return<object?>(null));

        target.RaiseClick();
        await Assert.That(wasCalled).IsTrue();
    }

    [Test]
    public async Task BindCommandToObject_WithExplicitEvent_BindsToSpecifiedEvent()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using var binding = binder.BindCommandToObject<ClickableControl, EventArgs>(
            command,
            target,
            Observable.Return<object?>(null),
            "Click");

        target.RaiseClick();
        await Assert.That(wasCalled).IsTrue();
    }

    [Test]
    public async Task BindCommandToObject_WithMouseUpEvent_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new MouseUpControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using var binding = binder.BindCommandToObject(command, target, Observable.Return<object?>(null));

        target.RaiseMouseUp();
        await Assert.That(wasCalled).IsTrue();
    }

    [Test]
    public void BindCommandToObject_WithNoEvents_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new object();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);

        Assert.Throws<Exception>(() =>
            binder.BindCommandToObject(command, target, Observable.Return<object?>(null)));
    }

    [Test]
    public void BindCommandToObject_WithNullTarget_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControl>(command, null, Observable.Return<object?>(null)));
    }

    [Test]
    public async Task BindCommandToObject_WithParameter_PassesParameterToCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        object? receivedParameter = null;
        var command = ReactiveCommand.Create<object?>(param => receivedParameter = param, outputScheduler: ImmediateScheduler.Instance);
        var parameter = new BehaviorSubject<object?>("test");

        using var binding = binder.BindCommandToObject(command, target, parameter);

        target.RaiseClick();
        await Assert.That(receivedParameter).IsEqualTo("test");
    }

    [Test]
    public async Task GetAffinityForObject_Generic_WithClickEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(false);
        await Assert.That(affinity).IsEqualTo(3);
    }

    [Test]
    public async Task GetAffinityForObject_Generic_WithEventTarget_Returns5()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(true);
        await Assert.That(affinity).IsEqualTo(5);
    }

    [Test]
    public async Task GetAffinityForObject_WithClickEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(false);
        await Assert.That(affinity).IsEqualTo(3);
    }

    [Test]
    public async Task GetAffinityForObject_WithEventTarget_Returns5()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<ClickableControl>(true);
        await Assert.That(affinity).IsEqualTo(5);
    }

    [Test]
    public async Task GetAffinityForObject_WithMouseUpEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<MouseUpControl>(false);
        await Assert.That(affinity).IsEqualTo(3);
    }

    [Test]
    public async Task GetAffinityForObject_WithNoEvents_Returns0()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<string>(false);
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task BindCommandToObject_WithNullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();

        var binding = binder.BindCommandToObject<ClickableControl>(null, target, Observable.Return<object?>(null));

        await Assert.That(binding).IsNotNull();
        binding?.Dispose(); // Should not throw
    }

    [Test]
    public async Task BindCommandToObject_WithExplicitEventAndNullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();

        var binding = binder.BindCommandToObject<ClickableControl, EventArgs>(
            null,
            target,
            Observable.Return<object?>(null),
            "Click");

        await Assert.That(binding).IsNotNull();
        binding?.Dispose(); // Should not throw
    }

    [Test]
    public void BindCommandToObject_WithNullEventName_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControl, EventArgs>(
                command,
                target,
                Observable.Return<object?>(null),
                null!));
    }

    [Test]
    public void BindCommandToObject_WithEmptyEventName_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);

        Assert.Throws<ArgumentException>(() =>
            binder.BindCommandToObject<ClickableControl, EventArgs>(
                command,
                target,
                Observable.Return<object?>(null),
                string.Empty));
    }

    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
            command,
            target,
            Observable.Return<object?>(null),
            handler => target.GenericClick += handler,
            handler => target.GenericClick -= handler);

        target.RaiseGenericClick();
        await Assert.That(wasCalled).IsTrue();
    }

    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_AfterDispose_DoesNotExecute()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using (var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
            command,
            target,
            Observable.Return<object?>(null),
            handler => target.GenericClick += handler,
            handler => target.GenericClick -= handler))
        {
            // Binding active
        }

        target.RaiseGenericClick();
        await Assert.That(wasCalled).IsFalse();
    }

    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_PassesParameter()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        object? receivedParameter = null;
        var command = ReactiveCommand.Create<object?>(param => receivedParameter = param, outputScheduler: ImmediateScheduler.Instance);
        var parameter = new BehaviorSubject<object?>("testParam");

        using var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
            command,
            target,
            parameter,
            handler => target.GenericClick += handler,
            handler => target.GenericClick -= handler);

        target.RaiseGenericClick();
        await Assert.That(receivedParameter).IsEqualTo("testParam");
    }

    [Test]
    public async Task BindCommandToObject_WithAddRemoveHandlers_NullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();

        var binding = binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
            null,
            target,
            Observable.Return<object?>(null),
            handler => target.GenericClick += handler,
            handler => target.GenericClick -= handler);

        await Assert.That(binding).IsNotNull();
        binding?.Dispose(); // Should not throw
    }

    [Test]
    public void BindCommandToObject_WithAddRemoveHandlers_NullTarget_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
                command,
                null,
                Observable.Return<object?>(null),
                handler => { },
                handler => { }));
    }

    [Test]
    public void BindCommandToObject_WithAddRemoveHandlers_NullAddHandler_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
                command,
                target,
                Observable.Return<object?>(null),
                null!,
                handler => { }));
    }

    [Test]
    public void BindCommandToObject_WithAddRemoveHandlers_NullRemoveHandler_Throws()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControlWithGenericEvent();
        var command = ReactiveCommand.Create(() => { }, outputScheduler: ImmediateScheduler.Instance);

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<ClickableControlWithGenericEvent, EventArgs>(
                command,
                target,
                Observable.Return<object?>(null),
                handler => { },
                null!));
    }

    [Test]
    public async Task BindCommandToObject_WithEventHandlerOverload_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using var binding = binder.BindCommandToObject(
            command,
            target,
            Observable.Return<object?>(null),
            handler => target.Click += handler,
            handler => target.Click -= handler);

        target.RaiseClick();
        await Assert.That(wasCalled).IsTrue();
    }

    [Test]
    public async Task BindCommandToObject_WithEventHandlerOverload_NullCommand_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new ClickableControl();

        var binding = binder.BindCommandToObject(
            null,
            target,
            Observable.Return<object?>(null),
            handler => target.Click += handler,
            handler => target.Click -= handler);

        await Assert.That(binding).IsNotNull();
        binding.Dispose(); // Should not throw
    }

    [Test]
    public async Task BindCommandToObject_WithTouchUpInsideEvent_ExecutesCommand()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var target = new TouchUpInsideControl();
        var wasCalled = false;
        var command = ReactiveCommand.Create(() => wasCalled = true, outputScheduler: ImmediateScheduler.Instance);

        using var binding = binder.BindCommandToObject(command, target, Observable.Return<object?>(null));

        target.RaiseTouchUpInside();
        await Assert.That(wasCalled).IsTrue();
    }

    [Test]
    public async Task GetAffinityForObject_WithTouchUpInsideEvent_Returns3()
    {
        var binder = new CreatesCommandBindingViaEvent();
        var affinity = binder.GetAffinityForObject<TouchUpInsideControl>(false);
        await Assert.That(affinity).IsEqualTo(3);
    }

    private class ClickableControl
    {
        public event EventHandler? Click;

        public void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);
    }

    private class MouseUpControl
    {
        public event EventHandler? MouseUp;

        public void RaiseMouseUp() => MouseUp?.Invoke(this, EventArgs.Empty);
    }

    private class TouchUpInsideControl
    {
        public event EventHandler? TouchUpInside;

        public void RaiseTouchUpInside() => TouchUpInside?.Invoke(this, EventArgs.Empty);
    }

    private class ClickableControlWithGenericEvent
    {
        public event EventHandler<EventArgs>? GenericClick;

        public void RaiseGenericClick() => GenericClick?.Invoke(this, EventArgs.Empty);
    }
}
