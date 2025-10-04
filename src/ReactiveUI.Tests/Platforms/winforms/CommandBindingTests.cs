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
[TestFixture]
public class CommandBindingTests
{
    /// <summary>
    /// Tests that the command binder binds to button.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CommandBinderBindsToButtonAsync()
    {
        using var testSequencer = new TestSequencer();
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.CreateRunInBackground<int>(_ => { });
        var input = new Button();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.GetAffinityForObject(input.GetType(), true), Is.GreaterThan(0));
            Assert.That(fixture.GetAffinityForObject(input.GetType(), false), Is.GreaterThan(0));
        }

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
            using (Assert.EnterMultipleScope())
            {
                Assert.That(commandExecuted, Is.True);
                Assert.That(ea, Is.Not.Null);
            }
        }
    }

    /// <summary>
    /// Tests that the command binder binds to custom control.
    /// </summary>
    [Test]
    public void CommandBinderBindsToCustomControl()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create<int>(_ => { });
        var input = new CustomClickableControl();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.GetAffinityForObject(input.GetType(), true), Is.GreaterThan(0));
            Assert.That(fixture.GetAffinityForObject(input.GetType(), false), Is.GreaterThan(0));
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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(commandExecuted, Is.True);
                Assert.That(ea, Is.Not.Null);
            }
        }
    }

    /// <summary>
    /// Tests that the command binder binds to custom component.
    /// </summary>
    [Test]
    public void CommandBinderBindsToCustomComponent()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var cmd = ReactiveCommand.Create<int>(_ => { });
        var input = new CustomClickableComponent();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.GetAffinityForObject(input.GetType(), true), Is.GreaterThan(0));
            Assert.That(fixture.GetAffinityForObject(input.GetType(), false), Is.GreaterThan(0));
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

            using (Assert.EnterMultipleScope())
            {
                Assert.That(commandExecuted, Is.True);
                Assert.That(ea, Is.Not.Null);
            }
        }
    }

    /// <summary>
    /// Tests that the command binder affects enabled.
    /// </summary>
    [Test]
    public void CommandBinderAffectsEnabledState()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Subject<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(static () => { }, canExecute);
        var input = new Button();

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            canExecute.OnNext(true);
            Assert.That(input.Enabled, Is.True);

            canExecute.OnNext(false);
            Assert.That(input.Enabled, Is.False);
        }
    }

    /// <summary>
    /// Tests that the command binder affects enabled state for components.
    /// </summary>
    [Test]
    public void CommandBinderAffectsEnabledStateForComponents()
    {
        var fixture = new CreatesWinformsCommandBinding();
        var canExecute = new Subject<bool>();
        canExecute.OnNext(true);

        var cmd = ReactiveCommand.Create(static () => { }, canExecute);
        var input = new ToolStripButton(); // ToolStripButton is a Component, not a Control

        using (fixture.BindCommandToObject(cmd, input, Observable.Return((object)5)))
        {
            canExecute.OnNext(true);
            Assert.That(input.Enabled, Is.True);

            canExecute.OnNext(false);
            Assert.That(input.Enabled, Is.False);
        }
    }
}
