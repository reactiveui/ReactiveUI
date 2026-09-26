// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.OS;

namespace ReactiveUI.Device.Tests;

/// <summary>Runs test code on the Android main (UI) thread, where views must be created and touched.</summary>
internal static class MainThread
{
    /// <summary>The handler bound to the main looper.</summary>
    private static readonly Handler MainHandler = new(Looper.MainLooper!);

    /// <summary>Gets a value indicating whether the calling thread is the main thread.</summary>
    internal static bool IsCurrent => Looper.MainLooper!.IsCurrentThread;

    /// <summary>Runs <paramref name="action"/> on the main thread and waits for it.</summary>
    /// <param name="action">The work to run.</param>
    /// <returns>A task that completes when the work has run.</returns>
    internal static Task RunAsync(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return RunAsync<object?>(() =>
        {
            action();
            return null;
        });
    }

    /// <summary>Runs <paramref name="func"/> on the main thread and returns its result.</summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="func">The work to run.</param>
    /// <returns>A task carrying the result.</returns>
    internal static Task<T> RunAsync<T>(Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        if (IsCurrent)
        {
            return Task.FromResult(func());
        }

        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!MainHandler.Post(() =>
        {
            try
            {
                completion.SetResult(func());
            }
            catch (Exception ex)
            {
                completion.SetException(ex);
            }
        }))
        {
            completion.SetException(new InvalidOperationException("The main looper rejected the work."));
        }

        return completion.Task;
    }
}
