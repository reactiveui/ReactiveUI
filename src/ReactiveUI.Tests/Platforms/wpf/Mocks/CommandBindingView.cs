// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;
using ReactiveUI.Tests.Xaml;

namespace ReactiveUI.Tests.Wpf;

public class CommandBindingView : ReactiveObject, IViewFor<CommandBindingViewModel>
{
    private CommandBindingViewModel? _viewModel;

    public CommandBindingView()
    {
        Command1 = new CustomClickButton();
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

    public CustomClickButton Command1 { get; protected set; }

    public Image Command2 { get; protected set; }

    public Image Command3 { get; protected set; }
}
