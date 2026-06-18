// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI.Primitives;

namespace ReactiveUI.Samples.Wpf;

/// <summary>
/// A reactive login view demonstrating WhenActivated, Bind, BindCommand,
/// and manual event marshaling for WPF's PasswordBox.
/// </summary>
public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="LoginView"/> class.</summary>
    [SuppressMessage("Reliability", "S3366:Don't expose 'this' in constructors", Justification = "WhenActivated/binding setup requires 'this'; single-threaded sample.")]
    public LoginView()
    {
        InitializeComponent();
        ViewModel = new(RxSchedulers.MainThreadScheduler);

        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                .DisposeWith(d);

            // WPF PasswordBox doesn't support data binding, so marshal changes manually.
            Signal.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                    h => Password.PasswordChanged += h,
                    h => Password.PasswordChanged -= h)
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
