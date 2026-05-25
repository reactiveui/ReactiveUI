// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A view used for command binding tests.
/// </summary>
public class WinformCommandBindView : IViewFor<WinformCommandBindViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WinformCommandBindView"/> class.
    /// </summary>
    public WinformCommandBindView()
    {
        Command1 = new();
        Command2 = new();
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (WinformCommandBindViewModel?)value;
    }

    /// <inheritdoc/>
    public WinformCommandBindViewModel? ViewModel { get; set; }

    /// <summary>
    /// Gets or sets the button bound to the first command.
    /// </summary>
    public Button Command1 { get; protected set; }

    /// <summary>
    /// Gets or sets the control bound to the second command.
    /// </summary>
    public CustomClickableControl Command2 { get; protected set; }
}
