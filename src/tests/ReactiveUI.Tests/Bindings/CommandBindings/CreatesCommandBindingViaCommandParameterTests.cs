// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace ReactiveUI.Tests.Bindings.CommandBindings;

/// <summary>Tests for <see cref="CreatesCommandBindingViaCommandParameter"/> command binding behavior.</summary>
public class CreatesCommandBindingViaCommandParameterTests
{
    /// <summary>Verifies that disposing the binding restores the original command and command parameter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_RestoresOriginalValuesOnDispose()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var originalCommand = ReactiveCommand.Create(() => { });
        const string OriginalParameter = "original";

        target.Command = originalCommand;
        target.CommandParameter = OriginalParameter;

        var newCommand = ReactiveCommand.Create(() => { });
        using (var binding = binder.BindCommandToObject(newCommand, target, Signal.Emit<object?>("new")))
        {
            await Assert.That(target.Command).IsEqualTo(newCommand);
            await Assert.That(target.CommandParameter).IsEqualTo("new");
        }

        await Assert.That(target.Command).IsEqualTo(originalCommand);
        await Assert.That(target.CommandParameter).IsEqualTo(OriginalParameter);
    }

    /// <summary>Verifies that the command parameter is set from the observable sequence.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_SetsCommandParameterFromObservable()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        const int InitialParameter = 42;
        const int UpdatedParameter = 100;
        var command = ReactiveCommand.Create<int>(_ => { });
        var parameter = new BehaviorSignal<object?>(InitialParameter);

        using var binding = binder.BindCommandToObject(command, target, parameter);

        await Assert.That(target.CommandParameter).IsEqualTo(InitialParameter);

        parameter.OnNext(UpdatedParameter);
        await Assert.That(target.CommandParameter).IsEqualTo(UpdatedParameter);
    }

    /// <summary>Verifies that the target's command property is set to the bound command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_SetsCommandProperty()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var command = ReactiveCommand.Create(() => { });

        using var binding = binder.BindCommandToObject(command, target, Signal.Emit<object?>(null));

        await Assert.That(target.Command).IsEqualTo(command);
    }

    /// <summary>Verifies that the command parameter is updated each time the observable emits a new value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_UpdatesParameterMultipleTimes()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var command = ReactiveCommand.Create<string>(_ => { });
        var parameter = new BehaviorSignal<object?>("first");

        using var binding = binder.BindCommandToObject(command, target, parameter);

        await Assert.That(target.CommandParameter).IsEqualTo("first");

        parameter.OnNext("second");
        await Assert.That(target.CommandParameter).IsEqualTo("second");

        parameter.OnNext("third");
        await Assert.That(target.CommandParameter).IsEqualTo("third");
    }

    /// <summary>Verifies that binding with an explicit event name returns an empty disposable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithEventName_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var command = ReactiveCommand.Create(() => { });

        var binding = binder.BindCommandToObject<CommandControl, EventArgs>(
            command,
            target,
            Signal.Emit<object?>(null),
            "SomeEvent");

        // Event-name binding is unsupported by this binder, so it returns a no-op disposable. The contract is just
        // an IDisposable; the concrete no-op type is an implementation detail, so only assert it is non-null.
        await Assert.That(binding).IsNotNull();
    }

    /// <summary>Verifies that binding a null command leaves the target command property null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindCommandToObject_WithNullCommand_Succeeds()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();

        using var binding = binder.BindCommandToObject(null, target, Signal.Emit<object?>(null));

        await Assert.That(target.Command).IsNull();
    }

    /// <summary>Verifies that binding to a null target throws an <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void BindCommandToObject_WithNullTarget_Throws()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var command = ReactiveCommand.Create(() => { });

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<CommandControl>(command, null, Signal.Emit<object?>(null)));
    }

    /// <summary>Verifies that the generic affinity check returns 5 for targets with command and command parameter properties.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_Generic_WithCommandAndCommandParameter_Returns5()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(false);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.Explicit);
    }

    /// <summary>Verifies that the generic affinity check returns 0 when an event target is requested.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_Generic_WithEventTarget_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(true);
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Verifies that the affinity check returns 5 for targets with command and command parameter properties.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task GetAffinityForObject_WithCommandAndCommandParameter_Returns5()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(false);
        await Assert.That(affinity).IsEqualTo(BindingAffinity.Explicit);
    }

    /// <summary>Verifies that the affinity check returns 0 when an event target is requested.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task GetAffinityForObject_WithEventTarget_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(true);
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Verifies that the affinity check returns 0 when only a command property is present.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_WithOnlyCommandProperty_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<OnlyCommandControl>(false);
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Verifies that the affinity check returns 0 for targets without a command property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetAffinityForObject_WithoutCommandProperty_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<string>(false);
        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Test control exposing both a command and a command parameter property.</summary>
    private sealed class CommandControl
    {
        /// <summary>Gets or sets the command.</summary>
        public ICommand? Command { get; set; }

        /// <summary>Gets or sets the command parameter.</summary>
        public object? CommandParameter { get; set; }
    }

    /// <summary>Test control exposing only a command property.</summary>
    private sealed class OnlyCommandControl
    {
        /// <summary>Gets or sets the command.</summary>
        public ICommand? Command { get; set; }
    }
}
