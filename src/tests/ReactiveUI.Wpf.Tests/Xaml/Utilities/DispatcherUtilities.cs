// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace ReactiveUI.Tests.Xaml.Utilities;

/// <summary>
/// Helper utility to handle dispatcher in tests.
/// </summary>
public static class DispatcherUtilities
{
    /// <summary>
    /// Makes the dispatcher perform the events to keep it running.
    /// </summary>
    public static void DoEvents()
    {
        var frame = new DispatcherFrame();
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
        Dispatcher.PushFrame(frame);
    }

    /// <summary>
    /// Gets the frame to exit.
    /// </summary>
    /// <param name="f">Unused frame object..</param>
    /// <returns>Unused return value.</returns>
    public static object? ExitFrame(object f)
    {
        if (f is not DispatcherFrame frame)
        {
            return null;
        }

        frame.Continue = false;

        return null;
    }
}
