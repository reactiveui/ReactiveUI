// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Input;

namespace ReactiveUI.Tests.Bindings.CommandBindings;

public class CreatesCommandBindingViaCommandParameterTests
{
    [Test]
    public async Task GetAffinityForObject_WithCommandAndCommandParameter_Returns5()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(hasEventTarget: false);
        await Assert.That(affinity).IsEqualTo(5);
    }

    [Test]
    public async Task GetAffinityForObject_WithEventTarget_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(hasEventTarget: true);
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task GetAffinityForObject_WithoutCommandProperty_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<string>(hasEventTarget: false);
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task GetAffinityForObject_WithOnlyCommandProperty_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<OnlyCommandControl>(hasEventTarget: false);
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task GetAffinityForObject_Generic_WithCommandAndCommandParameter_Returns5()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(hasEventTarget: false);
        await Assert.That(affinity).IsEqualTo(5);
    }

    [Test]
    public async Task GetAffinityForObject_Generic_WithEventTarget_Returns0()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var affinity = binder.GetAffinityForObject<CommandControl>(hasEventTarget: true);
        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    public async Task BindCommandToObject_SetsCommandProperty()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var command = ReactiveCommand.Create(() => { });

        using var binding = binder.BindCommandToObject(command, target, Observable.Return<object?>(null));

        await Assert.That(target.Command).IsEqualTo(command);
    }

    [Test]
    public async Task BindCommandToObject_SetsCommandParameterFromObservable()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var command = ReactiveCommand.Create<int>(_ => { });
        var parameter = new BehaviorSubject<object?>(42);

        using var binding = binder.BindCommandToObject(command, target, parameter);

        await Assert.That(target.CommandParameter).IsEqualTo(42);

        parameter.OnNext(100);
        await Assert.That(target.CommandParameter).IsEqualTo(100);
    }

    [Test]
    public async Task BindCommandToObject_RestoresOriginalValuesOnDispose()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var originalCommand = ReactiveCommand.Create(() => { });
        var originalParameter = "original";

        target.Command = originalCommand;
        target.CommandParameter = originalParameter;

        var newCommand = ReactiveCommand.Create(() => { });
        using (var binding = binder.BindCommandToObject(newCommand, target, Observable.Return<object?>("new")))
        {
            await Assert.That(target.Command).IsEqualTo(newCommand);
            await Assert.That(target.CommandParameter).IsEqualTo("new");
        }

        await Assert.That(target.Command).IsEqualTo(originalCommand);
        await Assert.That(target.CommandParameter).IsEqualTo(originalParameter);
    }

    [Test]
    public void BindCommandToObject_WithNullTarget_Throws()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var command = ReactiveCommand.Create(() => { });

        Assert.Throws<ArgumentNullException>(() =>
            binder.BindCommandToObject<CommandControl>(command, null, Observable.Return<object?>(null)));
    }

    [Test]
    public async Task BindCommandToObject_WithNullCommand_Succeeds()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();

        using var binding = binder.BindCommandToObject(null, target, Observable.Return<object?>(null));

        await Assert.That(target.Command).IsNull();
    }

    [Test]
    public async Task BindCommandToObject_WithEventName_ReturnsEmptyDisposable()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var command = ReactiveCommand.Create(() => { });

        var binding = binder.BindCommandToObject<CommandControl, EventArgs>(command, target, Observable.Return<object?>(null), "SomeEvent");

        await Assert.That(binding).IsEqualTo(Disposable.Empty);
    }

    [Test]
    public async Task BindCommandToObject_UpdatesParameterMultipleTimes()
    {
        var binder = new CreatesCommandBindingViaCommandParameter();
        var target = new CommandControl();
        var command = ReactiveCommand.Create<string>(_ => { });
        var parameter = new BehaviorSubject<object?>("first");

        using var binding = binder.BindCommandToObject(command, target, parameter);

        await Assert.That(target.CommandParameter).IsEqualTo("first");

        parameter.OnNext("second");
        await Assert.That(target.CommandParameter).IsEqualTo("second");

        parameter.OnNext("third");
        await Assert.That(target.CommandParameter).IsEqualTo("third");
    }

    private class CommandControl
    {
        public ICommand? Command { get; set; }

        public object? CommandParameter { get; set; }
    }

    private class OnlyCommandControl
    {
        public ICommand? Command { get; set; }
    }
}
