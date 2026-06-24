// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Wpf.Mocks;

/// <summary>A mock view used by WPF command binding tests.</summary>
public class CommandBindingView : ReactiveObject, IViewFor<CommandBindingViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="CommandBindingView"/> class.</summary>
    public CommandBindingView()
    {
        Command1 = new();
        Command2 = new();
        Command3 = new();
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (CommandBindingViewModel?)value;
    }

    /// <inheritdoc/>
    public CommandBindingViewModel? ViewModel
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the first command control.</summary>
    public CustomClickButton Command1 { get; protected set; }

    /// <summary>Gets the second command control.</summary>
    public Image Command2 { get; protected set; }

    /// <summary>Gets the third command control.</summary>
    public Image Command3 { get; protected set; }
}
