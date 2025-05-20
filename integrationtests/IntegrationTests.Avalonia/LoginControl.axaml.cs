// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using IntegrationTests.Shared;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace IntegrationTests.Avalonia;

/// <summary>
/// Interaction logic for LoginControl.xaml.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class LoginControl : ReactiveUserControl<LoginViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginControl"/> class.
    /// </summary>
    public LoginControl()
    {
        InitializeComponent();

        ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);

        this
            .WhenActivated(
                disposables =>
                {
                    this
                        .Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                        .DisposeWith(disposables);
                    this
                        .Bind(ViewModel, vm => vm.Password, v => v.Password.Text)
                        .DisposeWith(disposables);
                    this
                        .BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                        .DisposeWith(disposables);
                    this
                        .BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                        .DisposeWith(disposables);

                    ViewModel
                        .Login
                        .SelectMany(
                            result =>
                            {
                                if (result)
                                {
                                    return this.ShowMessage("Login Successful", "Welcome!");
                                }

                                return this.ShowMessage("Login Failed", "Ah, ah, ah, you didn't say the magic word!");
                            })
                        .Subscribe()
                        .DisposeWith(disposables);
                });
    }
}
