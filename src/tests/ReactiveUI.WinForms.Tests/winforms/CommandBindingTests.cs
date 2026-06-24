// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.WinForms.Tests.Winforms.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Command binding tests.</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]

public class CommandBindingTests
{
    /// <summary>The command parameter value used by the binding tests.</summary>
    private const int CommandParameter = 5;

    /// <summary>Tests that the command binder binds to button.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CommandBinderBindsToButtonAsync()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var commandExecuted = false;
        object? ea = null;

        var cmd = ReactiveCommand.Create<int>(x =>
        {
            ea = x;
            commandExecuted = true;
        });

        var input = new Button();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.GetAffinityForObject<Button>(true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject<Button>(false)).IsGreaterThan(0);
        }

        using (fixture.BindCommandToObject(cmd, input, Signal.Emit((object)CommandParameter)))
        {
            input.PerformClick();

            using (Assert.Multiple())
            {
                await Assert.That(commandExecuted).IsTrue();
                await Assert.That(ea).IsNotNull();
            }
        }
    }

    /// <summary>Tests that the command binder binds to custom control.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderBindsToCustomControl()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var commandExecuted = false;
        object? ea = null;

        var cmd = ReactiveCommand.Create<int>(
            x =>
            {
                ea = x;
                commandExecuted = true;
            },
            outputScheduler: Sequencer.Immediate);

        var input = new CustomClickableControl();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.GetAffinityForObject<CustomClickableControl>(true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject<CustomClickableControl>(false)).IsGreaterThan(0);
        }

        using (fixture.BindCommandToObject(cmd, input, Signal.Emit((object)CommandParameter)))
        {
            input.PerformClick();

            using (Assert.Multiple())
            {
                await Assert.That(commandExecuted).IsTrue();
                await Assert.That(ea).IsNotNull();
            }
        }
    }

    /// <summary>Tests that the command binder binds to custom component.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderBindsToCustomComponent()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var commandExecuted = false;
        object? ea = null;

        var cmd = ReactiveCommand.Create<int>(
            x =>
            {
                ea = x;
                commandExecuted = true;
            },
            outputScheduler: Sequencer.Immediate);

        var input = new CustomClickableComponent();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.GetAffinityForObject<CustomClickableComponent>(true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject<CustomClickableComponent>(false)).IsGreaterThan(0);
        }

        using (fixture.BindCommandToObject(cmd, input, Signal.Emit((object)CommandParameter)))
        {
            input.PerformClick();

            using (Assert.Multiple())
            {
                await Assert.That(commandExecuted).IsTrue();
                await Assert.That(ea).IsNotNull();
            }
        }
    }

    /// <summary>Tests that the command binder affects enabled.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderAffectsEnabledState()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Signal<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(static () => { }, canExecute);
        var input = new Button();

        using (fixture.BindCommandToObject(cmd, input, Signal.Emit((object)CommandParameter)))
        {
            canExecute.OnNext(true);
            await Assert.That(input.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(input.Enabled).IsFalse();
        }
    }

    /// <summary>Tests that the command binder affects enabled state for components.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderAffectsEnabledStateForComponents()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Signal<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(static () => { }, canExecute);
        var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control

        using (fixture.BindCommandToObject(cmd, input, Signal.Emit((object)CommandParameter)))
        {
            canExecute.OnNext(true);
            await Assert.That(input.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(input.Enabled).IsFalse();
        }
    }

    /// <summary>Binding to a named event invokes the command when that event fires.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_BindsToNamedEvent()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create<int>(_ => executed = true);
        var input = new Button();

        using (fixture.BindCommandToObject<Button, EventArgs>(cmd, input, Signal.Emit((object)CommandParameter), nameof(Button.Click)))
        {
            input.PerformClick();

            await Assert.That(executed).IsTrue();
        }
    }

    /// <summary>Binding via non-generic add/remove handlers invokes the command when the event fires.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithNonGenericHandlers_BindsToEvent()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create<int>(_ => executed = true);
        var input = new Button();

        using (fixture.BindCommandToObject<Button>(cmd, input, Signal.Emit((object)CommandParameter), h => input.Click += h, h => input.Click -= h))
        {
            input.PerformClick();

            await Assert.That(executed).IsTrue();
        }
    }

    /// <summary>Binding via generic add/remove handlers invokes the command when the typed event fires.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithGenericHandlers_BindsToTypedEvent()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var executed = false;
        var cmd = ReactiveCommand.Create<int>(_ => executed = true);
        using var control = new GenericEventControl();

        using (fixture.BindCommandToObject<GenericEventControl, CustomEventArgs>(cmd, control, Signal.Emit((object)CommandParameter), h => control.CustomEvent += h, h => control.CustomEvent -= h))
        {
            control.RaiseCustomEvent();

            await Assert.That(executed).IsTrue();
        }
    }
}
