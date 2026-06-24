// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Builder.BlazorServer.Models;
using Splat;

namespace ReactiveUI.Builder.BlazorServer.Services;

/// <summary>Service to manage ReactiveUI app lifecycle in Blazor Server.</summary>
public sealed class ReactiveUiAppHostedService : IHostedService
{
    /// <summary>The suspension driver used to load and persist application state to disk.</summary>
    private FileJsonSuspensionDriver? _driver;

    /// <summary>Initializes the application state and starts required services asynchronously.</summary>
    /// <remarks>This method loads any previously persisted application state and notifies listeners if the
    /// state changes. It also starts network and lifetime coordination services required for the application's
    /// operation. If loading the persisted state fails, the application continues with a new state instance.
    /// </remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RxSuspension.SuspensionHost.CreateNewAppState = static () => new ChatState();

        // Set an initial state instantly (same idea as WPF to avoid blocking)
        RxSuspension.SuspensionHost.AppState = new ChatState();

        var statePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ReactiveUI.Builder.BlazorServer",
            "state.json");
        _ = Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);

        _driver = new(statePath);

        // Set an initial state instantly to avoid blocking UI
        RxSuspension.SuspensionHost.AppState = new ChatState();

        // Load persisted state asynchronously and update UI when ready
        _ = _driver
            .LoadState()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Subscribe(Witness.Create<object?>(
                static stateObj =>
                {
                    RxSuspension.SuspensionHost.AppState = stateObj;
                    MessageBus.Current.SendMessage(new ChatStateChanged());
                    Trace.TraceInformation("[App] State loaded");
                },
                static ex => Trace.TraceInformation($"[App] State load failed: {ex.Message}")));

        var lifetime = Locator.Current.GetService<AppLifetimeCoordinator>();
        var count = lifetime?.Increment() ?? 1;
        Trace.TraceInformation($"[Blazor] Instance started. Count={count} Id={AppInstance.Id}");

        return Task.CompletedTask;
    }

    /// <summary>Performs application shutdown tasks asynchronously, including saving application state and releasing network resources.</summary>
    /// <remarks>If this is the last running instance, the method saves the current application state before
    /// disposing of network resources. Subsequent calls after all instances have exited will not trigger additional
    /// state saves.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the shutdown operation.</param>
    /// <returns>A task that represents the asynchronous shutdown operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var lifetime = Locator.Current.GetService<AppLifetimeCoordinator>();
        var remaining = lifetime?.Decrement() ?? 0;
        Trace.TraceInformation($"[Blazor] Instance exiting. Remaining={remaining} Id={AppInstance.Id}");

        // Only the last instance persists the final state to the central store
        if (remaining != 0 || _driver is null || RxSuspension.SuspensionHost.AppState is null)
        {
            return;
        }

        await _driver.SaveState(RxSuspension.SuspensionHost.AppState);
    }
}
