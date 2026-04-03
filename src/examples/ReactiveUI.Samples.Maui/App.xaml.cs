// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Samples.Maui;

/// <summary>
/// MAUI application shell that creates the initial navigation window.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App() => InitializeComponent();

    /// <inheritdoc/>
    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new AppShell());
}
