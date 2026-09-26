// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.Commands;

/// <summary>Shows how a running command is cancelled, by disposing its execution or from a second command.</summary>
public static class CancellationExamples
{
    /// <summary>How long a GitHub search takes in these examples, long enough to cancel it.</summary>
    private static readonly TimeSpan _slowNetwork = TimeSpan.FromSeconds(30);

    /// <summary>Disposing an execution cancels the token the task was given; the command stops executing.</summary>
    /// <returns>A task that completes when the command has stopped.</returns>
    public static async Task CancelByDisposing()
    {
        InMemoryGitHubApi api = new() { Latency = _slowNetwork };
        using RepositorySearchViewModel viewModel = new(api);
        Task<bool> stopped = viewModel.Search.IsExecuting.Where(static executing => !executing).Skip(1).FirstAsync().ToTask();

        IDisposable execution = viewModel.Search.Execute("platform").Subscribe(static _ => { }, static _ => { });
        Console.WriteLine(viewModel.IsSearching);

        execution.Dispose();
        _ = await stopped;

        Console.WriteLine(viewModel.IsSearching);
        Console.WriteLine(viewModel.Results.Count);

        // Output:
        // True
        // False
        // 0
    }

    /// <summary>A cancel button runs a second command; the search is built to stop when that command fires.</summary>
    /// <returns>A task that completes when the search has been cancelled.</returns>
    public static async Task CancelFromAnotherCommand()
    {
        InMemoryGitHubApi api = new() { Latency = _slowNetwork };
        using ReactiveCommand<RxVoid, RxVoid>? cancel = ReactiveCommand.Create(static () => { });
        using ReactiveCommand<string, IReadOnlyList<Repository>> search = ReactiveCommand.CreateFromObservable<string, IReadOnlyList<Repository>>(
            query => Signal.FromAsync(cancellationToken => api.SearchRepositoriesAsync(query, cancellationToken)).TakeUntil(cancel));
        using IDisposable canCancel = search.IsExecuting.Subscribe(static executing => Console.WriteLine($"Searching: {executing}"));
        Task<bool> stopped = search.IsExecuting.Where(static executing => !executing).Skip(1).FirstAsync().ToTask();

        using IDisposable execution = search.Execute("platform").Subscribe(static _ => { });
        _ = await cancel.Execute();
        _ = await stopped;

        Console.WriteLine(api.RequestCount);

        // Output:
        // Searching: False
        // Searching: True
        // Searching: False
        // 1
    }
}
