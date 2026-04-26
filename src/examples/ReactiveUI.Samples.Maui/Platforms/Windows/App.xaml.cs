// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Samples.Maui.WinUI;

/// <summary>
/// Windows application entry point.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App() => InitializeComponent();

    /// <inheritdoc/>
    protected override MauiApp CreateMauiApp() => ReactiveUI.Samples.Maui.MauiProgram.CreateMauiApp();
}
