// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ReactiveUI.Primitives;

namespace ReactiveUI.Samples.Wpf;

/// <summary>
/// A reactive login view demonstrating WhenActivated, Bind, BindCommand,
/// and manual event marshaling for WPF's PasswordBox.
/// </summary>
[DebuggerDisplay("LoginView")]
public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="LoginView"/> class.</summary>
    [SuppressMessage(
        "Correctness",
        "SST2403:Do not let 'this' escape from a constructor",
        Justification = "Single-threaded sample view; WhenActivated captures this for activation-scoped binding after construction.")]
    public LoginView()
    {
        InitializeComponent();
        ViewModel = new(RxSchedulers.MainThreadScheduler);

        _ = this.WhenActivated(d =>
        {
            _ = this.Bind(ViewModel, static vm => vm.UserName, static v => v.Username.Text)
                .DisposeWith(d);

            _ = this.BindCommand(ViewModel, static vm => vm.Login, static v => v.Login)
                .DisposeWith(d);

            _ = this.BindCommand(ViewModel, static vm => vm.Cancel, static v => v.Cancel)
                .DisposeWith(d);

            // WPF PasswordBox doesn't support data binding, so marshal changes manually.
            _ = Signal.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                    h => Password.PasswordChanged += h,
                    h => Password.PasswordChanged -= h)
                .Select(_ => Password.Password)
                .BindTo(this, static v => v.ViewModel!.Password)
                .DisposeWith(d);

            _ = ViewModel.Login
                .Subscribe(static success => MessageBox.Show(
                    success ? "Welcome!" : "Invalid credentials.",
                    success ? "Login Successful" : "Login Failed"))
                .DisposeWith(d);
        });
    }
}
