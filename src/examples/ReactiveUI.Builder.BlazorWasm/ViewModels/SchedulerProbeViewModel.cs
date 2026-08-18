// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Builder;

namespace ReactiveUI.Builder.BlazorWasm.ViewModels;

/// <summary>View model that verifies work executes through the configured browser event-loop scheduler.</summary>
[DebuggerDisplay("{Status}")]
public sealed class SchedulerProbeViewModel : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="SchedulerProbeViewModel"/> class.</summary>
    public SchedulerProbeViewModel() => Run = ReactiveCommand.CreateFromTask(ScheduleAsync);

    /// <summary>Gets the command that schedules a browser event-loop callback.</summary>
    public ReactiveCommand<RxVoid, RxVoid> Run { get; }

    /// <summary>Gets the current scheduler verification status.</summary>
    public string Status
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Ready";

    /// <summary>Schedules work through ReactiveUI's configured main scheduler.</summary>
    /// <returns>A task that completes after the event-loop callback runs.</returns>
    private async Task ScheduleAsync()
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var scheduled = RxSchedulers.MainThreadScheduler.Schedule(
            completion,
            static (_, source) =>
            {
                source.SetResult();
                return EmptyDisposable.Instance;
            });

        await completion.Task;

        Status = ReferenceEquals(
            RxSchedulers.MainThreadScheduler,
            BlazorReactiveUIBuilderExtensions.BlazorWasmScheduler)
            ? "WASM scheduler executed successfully"
            : "Unexpected scheduler configured";
    }
}
