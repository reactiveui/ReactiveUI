// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Primitives;

namespace ReactiveUI.Samples.Winforms;

/// <summary>A reactive login view demonstrating WhenActivated and reactive subscriptions for WinForms.</summary>
[DebuggerDisplay("LoginView")]
public sealed class LoginView : UserControl, IViewFor<LoginViewModel>
{
    /// <summary>The padding, in pixels, around the vertical layout panel.</summary>
    private const int LayoutPadding = 20;

    /// <summary>The width, in pixels, of each text box.</summary>
    private const int TextBoxWidth = 240;

    /// <summary>The width, in pixels, of each button.</summary>
    private const int ButtonWidth = 115;

    /// <summary>Initializes a new instance of the <see cref="LoginView"/> class.</summary>
    [SuppressMessage(
        "Correctness",
        "SST2403:Do not let 'this' escape from a constructor",
        Justification = "Single-threaded sample view; WhenActivated captures this for activation-scoped binding after construction.")]
    public LoginView()
    {
        var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new(LayoutPadding), WrapContents = false };

        layout.Controls.AddRange(UserNameBox, PasswordBox);

        var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
        buttons.Controls.AddRange(LoginButton, CancelButton);
        layout.Controls.Add(buttons);

        Controls.Add(layout);

        ViewModel = new(RxSchedulers.MainThreadScheduler);

        _ = this.WhenActivated(d =>
        {
            _ = this.Bind(ViewModel, vm => vm.UserName, v => v.UserNameBox.Text)
                .DisposeWith(d);

            _ = this.Bind(ViewModel, vm => vm.Password, v => v.PasswordBox.Text)
                .DisposeWith(d);

            _ = this.BindCommand(ViewModel, vm => vm.Login, v => v.LoginButton)
                .DisposeWith(d);

            _ = this.BindCommand(ViewModel, vm => vm.Cancel, v => v.CancelButton)
                .DisposeWith(d);

            _ = ViewModel.Login
                .Subscribe(static success => MessageBox.Show(
                    success ? "Welcome!" : "Invalid credentials.",
                    success ? "Login Successful" : "Login Failed"))
                .DisposeWith(d);
        });
    }

    /// <summary>Gets or sets the view model for this view.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public LoginViewModel? ViewModel { get; set; }

    /// <summary>Gets the text box bound to the view model's user name.</summary>
    internal TextBox UserNameBox { get; } = new() { PlaceholderText = "Username", Width = TextBoxWidth, Name = "Username" };

    /// <summary>Gets the text box bound to the view model's password.</summary>
    internal TextBox PasswordBox { get; } = new() { PlaceholderText = "Password", Width = TextBoxWidth, UseSystemPasswordChar = true, Name = "Password" };

    /// <summary>Gets the button bound to the view model's login command.</summary>
    internal Button LoginButton { get; } = new() { Text = "Login", Width = ButtonWidth, Name = "Login" };

    /// <summary>Gets the button bound to the view model's cancel command.</summary>
    internal Button CancelButton { get; } = new() { Text = "Cancel", Width = ButtonWidth, Name = "Cancel" };

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
            UserNameBox.Dispose();
            PasswordBox.Dispose();
            LoginButton.Dispose();
            CancelButton.Dispose();
        }

        base.Dispose(disposing);
    }
}
