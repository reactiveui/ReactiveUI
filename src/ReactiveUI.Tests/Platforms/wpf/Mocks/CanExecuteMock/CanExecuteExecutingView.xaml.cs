// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Interaction logic for CommandBindingView.xaml.
/// </summary>
public partial class CanExecuteExecutingView
{
    public CanExecuteExecutingView()
    {
        InitializeComponent();
        ViewModel = new();
        this.WhenActivated(d =>
        {
            this.BindCommand(ViewModel, vm => vm.Command3, v => v.Execute);
            this.OneWayBind(ViewModel, vm => vm.Result, v => v.Result.Text);
        });
    }
}
