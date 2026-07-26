// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Interfaces;

namespace ReactiveUI.WinUI.Tests;

/// <summary>Runs tests on a shared STA thread initialized with a WinUI XAML application.</summary>
public sealed class WinUITestExecutor : ITestExecutor
{
    /// <summary>Receives the shared dispatcher after the WinUI application initializes.</summary>
    private static readonly TaskCompletionSource<DispatcherQueue> DispatcherReady =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Initializes static members of the <see cref="WinUITestExecutor"/> class.</summary>
    static WinUITestExecutor() => StartDispatcher();

    /// <inheritdoc/>
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var dispatcher = await DispatcherReady.Task.ConfigureAwait(false);
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!dispatcher.TryEnqueue(() => _ = ExecuteAsync(action, completion)))
        {
            throw new InvalidOperationException("The WinUI test dispatcher is unavailable.");
        }

        await completion.Task.ConfigureAwait(false);
    }

    /// <summary>Stops the shared WinUI application after the test assembly completes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    internal static async Task StopAsync()
    {
        var dispatcher = await DispatcherReady.Task.ConfigureAwait(false);
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!dispatcher.TryEnqueue(() =>
            {
                Application.Current.Exit();
                completion.SetResult();
            }))
        {
            return;
        }

        await completion.Task.ConfigureAwait(false);
    }

    /// <summary>Starts the shared WinUI application thread.</summary>
    private static void StartDispatcher()
    {
        var thread = new Thread(static () =>
            Application.Start(static ignored =>
            {
                _ = new TestApplication();
                DispatcherReady.SetResult(DispatcherQueue.GetForCurrentThread());
            }));
        thread.IsBackground = true;
        thread.Name = "WinUI test dispatcher";
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    /// <summary>Executes a test and reports its completion to the calling test thread.</summary>
    /// <param name="action">The test action.</param>
    /// <param name="completion">Receives the test result.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private static async Task ExecuteAsync(Func<ValueTask> action, TaskCompletionSource completion)
    {
        var helper = new AppBuilderTestHelper();
        try
        {
            helper.Initialize(static builder => _ = builder.WithCoreServices());
            try
            {
                await action().ConfigureAwait(true);
            }
            finally
            {
                helper.CleanUp();
            }

            completion.SetResult();
        }
        catch (Exception exception)
        {
            completion.SetException(exception);
        }
    }

    /// <summary>Minimal WinUI application used to initialize XAML for the test process.</summary>
    private sealed class TestApplication : Application;
}
