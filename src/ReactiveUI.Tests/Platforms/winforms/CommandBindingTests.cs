// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;
using ReactiveUI.Testing;
using ReactiveUI.Winforms;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Command binding tests.
/// </summary>
public class CommandBindingTests
{
    /// <summary>
    /// Tests that the command binder binds to button.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CommandBinderBindsToButtonAsync()
    {
        using var testSequencer = new TestSequencer();
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.CreateRunInBackground<int>(_ => { });
        var input = new Button();

        Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
        Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);
        var commandExecuted = false;
        object? ea = null;
        cmd.Subscribe(async o =>
        {
            ea = o;
            commandExecuted = true;
            await testSequencer.AdvancePhaseAsync("Phase 1");
        });

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            input.PerformClick();
            await testSequencer.AdvancePhaseAsync("Phase 1");
            Assert.True(commandExecuted);
            Assert.NotNull(ea);
        }
    }

    /// <summary>
    /// Tests that the command binder binds to custom control.
    /// </summary>
    [Fact]
    public void CommandBinderBindsToCustomControl()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create<int>(_ => { });
        var input = new CustomClickableControl();

        Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
        Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);
        var commandExecuted = false;
        object? ea = null;
        cmd.Subscribe(o =>
        {
            ea = o;
            commandExecuted = true;
        });

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            input.PerformClick();

            Assert.True(commandExecuted);
            Assert.NotNull(ea);
        }
    }

    /// <summary>
    /// Tests that the command binder binds to custom component.
    /// </summary>
    [Fact]
    public void CommandBinderBindsToCustomComponent()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create<int>(_ => { });
        var input = new CustomClickableComponent();

        Assert.True(fixture.GetAffinityForObject(input.GetType(), true) > 0);
        Assert.True(fixture.GetAffinityForObject(input.GetType(), false) > 0);
        var commandExecuted = false;
        object? ea = null;
        cmd.Subscribe(o =>
        {
            ea = o;
            commandExecuted = true;
        });

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            input.PerformClick();

            Assert.True(commandExecuted);
            Assert.NotNull(ea);
        }
    }

    /// <summary>
    /// Tests that the command binder affects enabled.
    /// </summary>
    [Fact]
    public void CommandBinderAffectsEnabledState()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Subject<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(() => { }, canExecute);
        var input = new Button();

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            canExecute.OnNext(true);
            Assert.True(input.Enabled);

            canExecute.OnNext(false);
            Assert.False(input.Enabled);
        }
    }

    /// <summary>
    /// Tests that the command binder affects enabled state for components.
    /// </summary>
    [Fact]
    public void CommandBinderAffectsEnabledStateForComponents()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Subject<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(() => { }, canExecute);
        var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            canExecute.OnNext(true);
            Assert.True(input.Enabled);

            canExecute.OnNext(false);
            Assert.False(input.Enabled);
        }
    }
}
