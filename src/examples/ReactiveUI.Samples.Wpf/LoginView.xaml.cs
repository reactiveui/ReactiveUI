// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;

namespace ReactiveUI.Samples.Wpf;

/// <summary>
/// A reactive login view demonstrating WhenActivated, Bind, BindCommand,
/// and manual event marshaling for WPF's PasswordBox.
/// </summary>
public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginView"/> class.
    /// </summary>
    public LoginView()
    {
        InitializeComponent();
        ViewModel = new LoginViewModel(RxSchedulers.MainThreadScheduler);

        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                .DisposeWith(d);

            // WPF PasswordBox doesn't support data binding, so marshal changes manually.
            Observable.FromEventPattern(Password, nameof(PasswordBox.PasswordChanged))
                .Select(_ => Password.Password)
                .BindTo(this, v => v.ViewModel!.Password)
                .DisposeWith(d);

            ViewModel.Login
                .Subscribe(success => MessageBox.Show(
                    success ? "Welcome!" : "Invalid credentials.",
                    success ? "Login Successful" : "Login Failed"))
                .DisposeWith(d);
        });
    }
}
