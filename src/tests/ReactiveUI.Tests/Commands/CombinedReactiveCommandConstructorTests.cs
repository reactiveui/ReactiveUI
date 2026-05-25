// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveUI.Tests.Commands;

/// <summary>
/// Tests for the <see cref="CombinedReactiveCommand{TParam, TResult}"/> constructor overloads that are only reachable
/// by direct construction (the public <c>CreateCombined</c> factory routes solely through the three-argument form).
/// </summary>
public class CombinedReactiveCommandConstructorTests
{
    /// <summary>Verifies the (childCommands, canExecute) constructor builds a usable combined command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanExecuteConstructor_BuildsCommand()
    {
        var child = ReactiveCommand.Create(static () => { }, outputScheduler: ImmediateScheduler.Instance);
        IEnumerable<ReactiveCommandBase<Unit, Unit>> children = [child];
        IObservable<bool> canExecute = Observable.Return(true);

        using var combined = new CombinedReactiveCommand<Unit, Unit>(children, canExecute);

        await Assert.That(combined).IsNotNull();
    }

    /// <summary>Verifies the (childCommands, outputScheduler) constructor builds a usable combined command.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SchedulerConstructor_BuildsCommand()
    {
        var child = ReactiveCommand.Create(static () => { }, outputScheduler: ImmediateScheduler.Instance);
        IEnumerable<ReactiveCommandBase<Unit, Unit>> children = [child];

        using var combined = new CombinedReactiveCommand<Unit, Unit>(children, ImmediateScheduler.Instance);

        await Assert.That(combined).IsNotNull();
    }
}
