// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.BlazorServer.Models;

namespace ReactiveUI.Builder.BlazorServer.Services;

/// <summary>
/// Service to manage ReactiveUI app lifecycle in Blazor Server.
/// </summary>
public sealed class ReactiveUiAppHostedService : IHostedService
{
    /// <summary>
    /// Initializes the application state and starts required services asynchronously.
    /// </summary>
    /// <remarks>This method loads any previously persisted application state and notifies listeners if the
    /// state changes. It also starts network and lifetime coordination services required for the application's
    /// operation. If loading the persisted state fails, the application continues with a new state instance.
    /// </remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        RxSuspension.SuspensionHost.CreateNewAppState = static () => new ChatState();

        // Set an initial state instantly (same idea as WPF to avoid blocking)
        RxSuspension.SuspensionHost.AppState = new ChatState();

        var lifetime = Locator.Current.GetService<AppLifetimeCoordinator>();
        var count = lifetime?.Increment() ?? 1;
        Trace.WriteLine($"[Blazor] Instance started. Count={count} Id={AppInstance.Id}");
    }

    /// <summary>
    /// Performs application shutdown tasks asynchronously, including saving application state and releasing network
    /// resources.
    /// </summary>
    /// <remarks>If this is the last running instance, the method saves the current application state before
    /// disposing of network resources. Subsequent calls after all instances have exited will not trigger additional
    /// state saves.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the shutdown operation.</param>
    /// <returns>A task that represents the asynchronous shutdown operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var lifetime = Locator.Current.GetService<AppLifetimeCoordinator>();
        var remaining = lifetime?.Decrement() ?? 0;
        Trace.WriteLine($"[Blazor] Instance exiting. Remaining={remaining} Id={AppInstance.Id}");
    }
}
