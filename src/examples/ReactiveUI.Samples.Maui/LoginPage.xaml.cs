// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Samples.Maui;

/// <summary>
/// A reactive login page demonstrating WhenActivated, Bind, BindCommand,
/// and DisplayAlert for user feedback in MAUI.
/// </summary>
public partial class LoginPage : ReactiveUI.Maui.ReactiveContentPage<LoginViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage"/> class.
    /// </summary>
    public LoginPage()
    {
        InitializeComponent();
        ViewModel = new LoginViewModel(RxSchedulers.MainThreadScheduler);

        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                .DisposeWith(d);

            this.Bind(ViewModel, vm => vm.Password, v => v.Password.Text)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Login, v => v.LoginButton)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(d);

            ViewModel.Login
                .SelectMany(success => Observable.FromAsync(() =>
                    DisplayAlert(
                        success ? "Login Successful" : "Login Failed",
                        success ? "Welcome!" : "Invalid credentials.",
                        "OK")))
                .Subscribe()
                .DisposeWith(d);
        });
    }
}
