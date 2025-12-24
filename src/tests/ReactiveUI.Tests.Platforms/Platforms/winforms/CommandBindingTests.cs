// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
            await Assert.That(fixture.GetAffinityForObject(input.GetType(), true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject(input.GetType(), false)).IsGreaterThan(0);
        }

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            input.PerformClick();

            using (Assert.Multiple())
            {
                await Assert.That(commandExecuted).IsTrue();
                await Assert.That(ea).IsNotNull();
            }
        }
    }

    /// <summary>
    /// Tests that the command binder binds to custom control.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderBindsToCustomControl()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create<int>(_ => { });
        var input = new CustomClickableControl();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.GetAffinityForObject(input.GetType(), true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject(input.GetType(), false)).IsGreaterThan(0);
        }

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

            using (Assert.Multiple())
            {
                await Assert.That(commandExecuted).IsTrue();
                await Assert.That(ea).IsNotNull();
            }
        }
    }

    /// <summary>
    /// Tests that the command binder binds to custom component.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderBindsToCustomComponent()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create<int>(_ => { });
        var input = new CustomClickableComponent();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.GetAffinityForObject(input.GetType(), true)).IsGreaterThan(0);
            await Assert.That(fixture.GetAffinityForObject(input.GetType(), false)).IsGreaterThan(0);
        }

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

            using (Assert.Multiple())
            {
                await Assert.That(commandExecuted).IsTrue();
                await Assert.That(ea).IsNotNull();
            }
        }
    }

    /// <summary>
    /// Tests that the command binder affects enabled.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderAffectsEnabledState()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Subject<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(static () => { }, canExecute);
        var input = new Button();

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            canExecute.OnNext(true);
            await Assert.That(input.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(input.Enabled).IsFalse();
        }
    }

    /// <summary>
    /// Tests that the command binder affects enabled state for components.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CommandBinderAffectsEnabledStateForComponents()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Subject<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(static () => { }, canExecute);
        var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            canExecute.OnNext(true);
            await Assert.That(input.Enabled).IsTrue();

            canExecute.OnNext(false);
            await Assert.That(input.Enabled).IsFalse();
        }
    }
}
