// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf.Mocks.TransitionMock;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
[ExcludeFromViewRegistration]
public partial class TcMockWindow
{
    /// <summary>
    /// The off-screen position used to hide the window during tests.
    /// </summary>
    private const int OffScreenPosition = -10000;

    /// <summary>
    /// Initializes a new instance of the <see cref="TcMockWindow"/> class.
    /// </summary>
    public TcMockWindow()
    {
        InitializeComponent();

        // Hide window from user during tests
        ShowInTaskbar = false;
        WindowStyle = System.Windows.WindowStyle.None;
        Left = OffScreenPosition;
        Top = OffScreenPosition;
        Width = 1;
        Height = 1;
        Opacity = 0; // Make completely transparent
        ShowActivated = false; // Don't steal focus
    }
}
