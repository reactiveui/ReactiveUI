// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using IntegrationTests.Shared;
using ReactiveUI;

namespace IntegrationTests.WPF
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml.
    /// </summary>
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
                           .BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                           .DisposeWith(disposables);
                       this
                           .BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                           .DisposeWith(disposables);

                       // we marshal changes to password manually because WPF's
                       // PasswordBox.Password property doesn't support change notifications
                       Password
                          .Events()
                          .PasswordChanged
                          .Select(_ => Password.Password)
                          .Subscribe(x => ViewModel.Password = x)
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
}
