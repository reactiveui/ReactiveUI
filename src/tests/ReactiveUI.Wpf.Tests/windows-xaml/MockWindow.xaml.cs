// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Interaction logic for MockWindow.xaml.
/// </summary>
[ExcludeFromViewRegistration]
public partial class MockWindow
{
    public MockWindow()
    {
        InitializeComponent();
        ViewModel = new();

        // Hide window from user during tests
        ShowInTaskbar = false;
        WindowStyle = System.Windows.WindowStyle.None;
        Left = -10000;
        Top = -10000;
        Width = 1;
        Height = 1;
        Opacity = 0; // Make completely transparent
        ShowActivated = false; // Don't steal focus
    }
}
