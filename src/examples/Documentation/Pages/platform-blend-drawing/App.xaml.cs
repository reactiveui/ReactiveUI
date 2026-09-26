// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows;
using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.PlatformBlendDrawing;

/// <summary>
/// Application entry point. Configures ReactiveUI for WPF, adds <see cref="Drawing.Registrations"/>
/// through <c>WithDrawing</c>, then shows the main window — unless started with <c>--smoke</c>, in which case it
/// drives the dashboard itself, prints what it observed, and exits.
/// </summary>
[DebuggerDisplay("App")]
public partial class App : Application
{
    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().WithDrawing().BuildApp();

        // ReactiveUI.Drawing.Registrations only registers an IBitmapLoader on Windows (or .NET Framework); this
        // process is a real WPF app on Windows, so WithDrawing's registration is now resolvable.
        IBitmapLoader? bitmapLoader = Locator.Current.GetService<IBitmapLoader>();
        Console.WriteLine($"IBitmapLoader after WithDrawing(): {bitmapLoader?.GetType().Name ?? "(none)"}");

        if (e.Args.Contains("--smoke"))
        {
            SmokeTest.Run();
            return;
        }

        MainWindow = new MainWindow();
        MainWindow.Show();
    }
}
