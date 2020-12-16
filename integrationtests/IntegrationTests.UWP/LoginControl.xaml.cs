// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using IntegrationTests.Shared;
using ReactiveUI;
using Windows.UI.Popups;

namespace IntegrationTests.UWP
{
    /// <summary>
    /// A control for logging the user in.
    /// </summary>
    public partial class LoginControl : LoginControlBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginControl"/> class.
        /// </summary>
        public LoginControl()
        {
            InitializeComponent();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler)
            {
                UserName = string.Empty,
                Password = string.Empty
            };

            this
               .WhenActivated(
                   disposables =>
                   {
                       this
                           .Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                           .DisposeWith(disposables);
                       this
                           .Bind(ViewModel, vm => vm.Password, v => v.Password.Password)
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
                                   var dialog = result ?
                                       new MessageDialog("Login Successful", "Welcome!") :
                                       new MessageDialog("Login Failed", "Ah, ah, ah, you didn't say the magic word!");

                                   return dialog.ShowAsync().ToObservable();
                               })
                           .Subscribe()
                           .DisposeWith(disposables);
                   });
        }
    }
}
