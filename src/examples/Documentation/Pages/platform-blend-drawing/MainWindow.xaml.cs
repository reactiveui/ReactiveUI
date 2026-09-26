// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformBlendDrawing;

/// <summary>The dashboard's only window. Its status panel and alert label are driven entirely by Blend behaviors.</summary>
[DebuggerDisplay("MainWindow")]
public partial class MainWindow : System.Windows.Window
{
    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new WeatherStateViewModel();
        DataContext = ViewModel;
    }

    /// <summary>Gets the view model backing the window, so <see cref="SmokeTest"/> can drive it.</summary>
    public WeatherStateViewModel ViewModel { get; }
}
