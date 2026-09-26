// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using WinRT;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>The unpackaged process entry point.</summary>
public static class Program
{
    /// <summary>The process's command-line arguments, read by the static startup callback below.</summary>
    private static string[] _startupArgs = [];

    /// <summary>
    /// Resolves the Windows App Runtime, then starts the WinUI application on this thread. This project sets
    /// <c>WindowsAppSdkBootstrapInitialize=false</c> so a missing runtime is reported here, on purpose, rather than
    /// through the SDK's automatic module initializer calling <see cref="Environment.Exit(int)"/> before <c>Main</c>
    /// has a chance to say why.
    /// </summary>
    /// <param name="args">The process's command-line arguments; pass <c>--smoke</c> to drive the app and exit.</param>
    [STAThread]
    private static void Main(string[] args)
    {
        _startupArgs = args;
        bool initialized = Bootstrap.TryInitialize(
            Microsoft.WindowsAppSDK.Release.MajorMinor,
            Microsoft.WindowsAppSDK.Release.VersionTag,
            new PackageVersion(Microsoft.WindowsAppSDK.Runtime.Version.UInt64),
            Bootstrap.InitializeOptions.None,
            out int hresult);

        if (!initialized)
        {
            Console.WriteLine($"No Windows App Runtime framework package matched (HRESULT 0x{hresult:X8}).");
            Environment.Exit(hresult);
            return;
        }

        try
        {
            ComWrappersSupport.InitializeComWrappers();
            Application.Start(static callbackParameters =>
            {
                _ = callbackParameters;
                DispatcherQueueSynchronizationContext context = new(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                _ = new WeatherStationApp(_startupArgs);
            });
        }
        finally
        {
            Bootstrap.Shutdown();
        }
    }
}
