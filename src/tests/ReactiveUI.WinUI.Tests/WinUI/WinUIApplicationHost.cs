// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using TUnit.Core.Exceptions;
using WinRT;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Owns the test process's XAML application and marshals test bodies onto its UI thread.</summary>
/// <remarks>
/// <para>
/// A WinUI 3 <see cref="DependencyObject"/> can only be constructed on a thread whose XAML core has been
/// initialized, and <see cref="Application.Start"/> is the only API that initializes one. It never returns
/// until the application exits, so it is given a dedicated STA background thread; the process can still
/// terminate normally once the run finishes because that thread is a background thread.
/// </para>
/// <para>
/// Reaching the Windows App SDK at all also requires the framework package to be resolved first. This test
/// host is unpackaged, so <see cref="WindowsAppRuntimeBootstrap"/> does that on first use, before anything
/// here touches a WinRT type.
/// </para>
/// </remarks>
internal static class WinUIApplicationHost
{
    /// <summary>Starts the XAML application once, on first use, and yields its UI thread's dispatcher queue.</summary>
    private static readonly Lazy<Task<DispatcherQueue>> Startup = new(Start);

    /// <summary>Runs <paramref name="body"/> on the XAML application's UI thread and awaits its completion.</summary>
    /// <param name="body">The work to run on the UI thread.</param>
    /// <returns>A task that completes when <paramref name="body"/> has completed, faulting with its exception.</returns>
    /// <exception cref="SkipTestException">No usable Windows App Runtime was found.</exception>
    /// <exception cref="InvalidOperationException">The dispatcher queue refused to accept the work.</exception>
    /// <remarks>
    /// Every executor that needs XAML funnels through here, so this is the one place the runtime prerequisite has
    /// to be enforced — including for test classes written later. Skipping before the application is started keeps
    /// a machine without the runtime reporting a reason per test instead of dying inside the native loader.
    /// </remarks>
    internal static async Task RunOnUIThreadAsync(Func<ValueTask> body)
    {
        WindowsAppRuntimeBootstrap.SkipIfUnavailable();

        var queue = await Startup.Value.ConfigureAwait(false);
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!queue.TryEnqueue(() => _ = InvokeAsync(body, completion)))
        {
            throw new InvalidOperationException("The WinUI dispatcher queue refused to accept the test body.");
        }

        await completion.Task.ConfigureAwait(false);
    }

    /// <summary>Starts the XAML application on a dedicated STA thread.</summary>
    /// <returns>A task yielding the dispatcher queue of the thread hosting the application.</returns>
    private static Task<DispatcherQueue> Start()
    {
        var startup = new TaskCompletionSource<DispatcherQueue>(TaskCreationOptions.RunContinuationsAsynchronously);
        var thread = new Thread(() => RunApplication(startup)) { IsBackground = true, Name = "WinUI test UI thread" };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return startup.Task;
    }

    /// <summary>Runs the XAML message loop for the lifetime of the test process.</summary>
    /// <param name="startup">Signalled with the UI thread's dispatcher queue once XAML is initialized.</param>
    private static void RunApplication(TaskCompletionSource<DispatcherQueue> startup)
    {
        try
        {
            ComWrappersSupport.InitializeComWrappers();

            Application.Start(callbackParameters =>
            {
                _ = callbackParameters;

                var queue = DispatcherQueue.GetForCurrentThread()
                    ?? throw new InvalidOperationException("XAML started without a dispatcher queue on its own thread.");

                SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(queue));
                _ = new WinUITestApplication();
                startup.SetResult(queue);
            });
        }
        catch (Exception ex)
        {
            _ = startup.TrySetException(ex);
        }
    }

    /// <summary>Invokes the test body on the UI thread and relays its outcome to the caller.</summary>
    /// <param name="body">The work to run.</param>
    /// <param name="completion">The caller's completion source.</param>
    /// <returns>A task that completes once the outcome has been relayed.</returns>
    private static async Task InvokeAsync(Func<ValueTask> body, TaskCompletionSource completion)
    {
        try
        {
            await body().ConfigureAwait(true);
            completion.SetResult();
        }
        catch (Exception ex)
        {
            completion.SetException(ex);
        }
    }
}
