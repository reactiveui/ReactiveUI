// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Advanced;

namespace ReactiveUI.Documentation.Commands;

/// <summary>Shows the members a <c>CombinedReactiveCommand&lt;TParam, TResult&gt;</c> owner can call directly, without going through a base or interface member.</summary>
public static class CombinedCommandMembersExamples
{
    /// <summary>
    /// <c>Execute(TParam)</c>, <c>Subscribe</c>, <c>CanExecute</c> and <c>ThrownExceptions</c> on a
    /// <see cref="CombinedReactiveCommand{TParam, TResult}"/> itself, rather than through <see cref="ReactiveCommand"/>'s
    /// factory result or the <see cref="IReactiveCommand"/> interface.
    /// </summary>
    /// <returns>A task that completes once the combined command has run and reported its state.</returns>
    public static async Task ReadCombinedCommandMembersDirectly()
    {
        LibraryDesk desk = new();
        using ReactiveCommand<RxVoid, int> countShelf = ReactiveCommand.Create(() => desk.Search(string.Empty).Count);
        using LoggingCombinedCommand<RxVoid, int> tally = new([countShelf], canExecute: null, outputScheduler: Sequencer.Immediate);

        List<IList<int>> results = [];
        IObserver<IList<int>> witness = Witness.Create<IList<int>>(results.Add);
        using IDisposable subscription = tally.Subscribe(witness);
        using IDisposable exceptionSubscription = tally.ThrownExceptions.Subscribe(static _ => { });

        bool canExecute = await tally.CanExecute.FirstAsync();
        IList<int> firstRun = await tally.Execute(default);

        Console.WriteLine(canExecute);
        Console.WriteLine(firstRun[0]);
        Console.WriteLine(results.Count);

        // Output:
        // Combined command running
        // True
        // 4
        // 1
    }
}
