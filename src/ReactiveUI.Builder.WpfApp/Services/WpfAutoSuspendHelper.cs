// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace ReactiveUI.Builder.WpfApp.Services;

/// <summary>
/// WpfAutoSuspendHelper.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WpfAutoSuspendHelper"/> class.
/// </remarks>
/// <param name="app">The application.</param>
/// <param name="driver">The driver.</param>
public sealed class WpfAutoSuspendHelper(Application app, ISuspensionDriver driver)
{
    /// <summary>
    /// Called on application exit to allow any cleanup.
    /// </summary>
    public void OnExit()
    {
        var d = driver;
    }

    /// <summary>
    /// Called on application startup to configure suspension.
    /// </summary>
    public void OnStartup()
    {
        RxApp.SuspensionHost.IsLaunchingNew = Observable.Return(Unit.Default);
        RxApp.SuspensionHost.IsResuming = Observable.Never<Unit>();
        RxApp.SuspensionHost.IsUnpausing = Observable.Never<Unit>();
        RxApp.SuspensionHost.ShouldPersistState = Observable.Never<IDisposable>();

        RxApp.SuspensionHost.SetupDefaultSuspendResume(driver);
    }
}
