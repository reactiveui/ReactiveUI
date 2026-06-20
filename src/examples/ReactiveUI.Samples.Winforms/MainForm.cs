// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Samples.Winforms;

/// <summary>Main application form that hosts the login view.</summary>
public sealed class MainForm : Form
{
    /// <summary>The width of the main window client area, in pixels.</summary>
    private const int WindowWidth = 300;

    /// <summary>The height of the main window client area, in pixels.</summary>
    private const int WindowHeight = 200;

    /// <summary>Initializes a new instance of the <see cref="MainForm"/> class.</summary>
    public MainForm()
    {
        Text = "ReactiveUI WinForms Login Sample";
        ClientSize = new(WindowWidth, WindowHeight);
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(new LoginView { Dock = DockStyle.Fill });
    }
}
