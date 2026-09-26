// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows;
using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// Application entry point. Configures ReactiveUI for WPF, wires up <see cref="AutoSuspendHelper"/>, then shows the
/// main window — unless started with <c>--smoke</c>, in which case it drives the app itself and exits.
/// </summary>
[DebuggerDisplay("App")]
public partial class App : Application
{
    /// <summary>How long the app waits after losing focus before it persists its state.</summary>
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromSeconds(20);

    /// <summary>Saves the grade book whenever the suspension host asks for it.</summary>
    private IDisposable? _persistSubscription;

    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _ = RxAppBuilder.CreateReactiveUIBuilder().WithWpf().BuildApp();

        // The school office's library registers its view with the service locator only, so the window's notice
        // board shows it through the Unsafe twins.
        AppLocator.CurrentMutable.Register<IViewFor<OfficeNoticeViewModel>>(static () => new OfficeNoticeView());

        WpfBuilderExtensionsExamples.ShowTheAppBuilderOverload();
        WpfBuilderExtensionsExamples.ShowTheIndividualExtensions();
        WpfBuilderExtensionsExamples.AddTheUnsafeTemplateHook();

        AutoSuspendHelper autoSuspendHelper = new(this) { IdleTimeout = IdleTimeout };
        Console.WriteLine($"Auto-suspend idle timeout: {autoSuspendHelper.IdleTimeout}");
        RxSuspension.SuspensionHost.CreateNewAppState = static () => new GradeBookState();
        _persistSubscription = RxSuspension.SuspensionHost.ShouldPersistState.Subscribe(static token =>
        {
            Console.WriteLine("Saving grade book state...");
            token.Dispose();
        });

        if (e.Args.Contains("--smoke"))
        {
            SmokeTest.Run();
            return;
        }

        MainWindow = new MainWindow();
        MainWindow.Show();
    }

    /// <inheritdoc/>
    protected override void OnExit(ExitEventArgs e)
    {
        _persistSubscription?.Dispose();
        base.OnExit(e);
    }
}
