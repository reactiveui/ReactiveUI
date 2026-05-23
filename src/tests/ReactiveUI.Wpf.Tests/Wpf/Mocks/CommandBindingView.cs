// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Wpf.Mocks;

/// <summary>
/// A mock view used by WPF command binding tests.
/// </summary>
public class CommandBindingView : ReactiveUI.ReactiveObject, IViewFor<CommandBindingViewModel>
{
    /// <summary>
    /// Backing field for the <see cref="ViewModel"/> property.
    /// </summary>
    private CommandBindingViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBindingView"/> class.
    /// </summary>
    public CommandBindingView()
    {
        Command1 = new();
        Command2 = new Image();
        Command3 = new Image();
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
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }

    /// <summary>
    /// Gets or sets the first command control.
    /// </summary>
    public CustomClickButton Command1 { get; protected set; }

    /// <summary>
    /// Gets or sets the second command control.
    /// </summary>
    public Image Command2 { get; protected set; }

    /// <summary>
    /// Gets or sets the third command control.
    /// </summary>
    public Image Command3 { get; protected set; }
}
