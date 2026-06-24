// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Wpf.Mocks.CanExecuteMock;

/// <summary>Interaction logic for CommandBindingView.xaml.</summary>
[ExcludeFromViewRegistration]
public partial class CanExecuteExecutingView
{
    /// <summary>Initializes a new instance of the <see cref="CanExecuteExecutingView"/> class.</summary>
    public CanExecuteExecutingView()
    {
        InitializeComponent();
        ViewModel = new();
        _ = this.WhenActivated(d =>
        {
            d(this.BindCommand(ViewModel, vm => vm.Command3, v => v.Execute));
            d(this.OneWayBind(ViewModel, vm => vm.Result, v => v.Result.Text));
        });
    }
}
