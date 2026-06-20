// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Primitives;

namespace ReactiveUI.Samples.Winforms;

/// <summary>A reactive login view demonstrating WhenActivated and reactive subscriptions for WinForms.</summary>
public sealed class LoginView : UserControl, IViewFor<LoginViewModel>
{
    /// <summary>The text box bound to the view model's user name.</summary>
    private readonly TextBox _username = new() { PlaceholderText = "Username", Width = 240, Name = "Username" };

    /// <summary>The text box bound to the view model's password.</summary>
    private readonly TextBox _password = new()
    {
        PlaceholderText = "Password",
        Width = 240,
        UseSystemPasswordChar = true,
        Name = "Password"
    };

    /// <summary>The button bound to the view model's login command.</summary>
    private readonly Button _login = new() { Text = "Login", Width = 115, Name = "Login" };

    /// <summary>The button bound to the view model's cancel command.</summary>
    private readonly Button _cancel = new() { Text = "Cancel", Width = 115, Name = "Cancel" };

    /// <summary>Initializes a new instance of the <see cref="LoginView"/> class.</summary>
    public LoginView()
    {
        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new(20),
            WrapContents = false
        };

        layout.Controls.AddRange(_username, _password);

        var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
        buttons.Controls.AddRange(_login, _cancel);
        layout.Controls.Add(buttons);

        Controls.Add(layout);

        ViewModel = new(RxSchedulers.MainThreadScheduler);

        this.WhenActivated(d =>
        {
            this.Bind(ViewModel, vm => vm.UserName, v => v._username.Text)
                .DisposeWith(d);

            this.Bind(ViewModel, vm => vm.Password, v => v._password.Text)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Login, v => v._login)
                .DisposeWith(d);

            this.BindCommand(ViewModel, vm => vm.Cancel, v => v._cancel)
                .DisposeWith(d);

            ViewModel.Login
                .Subscribe(success => MessageBox.Show(
                    success ? "Welcome!" : "Invalid credentials.",
                    success ? "Login Successful" : "Login Failed"))
                .DisposeWith(d);
        });
    }

    /// <summary>Gets or sets the view model for this view.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LoginViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as LoginViewModel;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _username.Dispose();
            _password.Dispose();
            _login.Dispose();
            _cancel.Dispose();
        }

        base.Dispose(disposing);
    }
}
