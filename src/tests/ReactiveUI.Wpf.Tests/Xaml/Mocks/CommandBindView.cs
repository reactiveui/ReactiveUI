// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Mocks;
using Image = System.Windows.Controls.Image;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>Mock command binding view.</summary>
public class CommandBindView : IViewFor<CommandBindViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="CommandBindView"/> class.</summary>
    public CommandBindView()
    {
        Command1 = new();
        Command2 = new Image();
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (CommandBindViewModel?)value;
    }

    /// <inheritdoc/>
    public CommandBindViewModel? ViewModel { get; set; }

    /// <summary>Gets the command1.</summary>
    public CustomClickButton Command1 { get; protected set; }

    /// <summary>Gets the command2.</summary>
    public Image Command2 { get; protected set; }
}
