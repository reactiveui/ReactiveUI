// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Forms;
using IntegrationTests.Shared;
using ReactiveUI;

namespace IntegrationTests.WinForms
{
    /// <summary>
    /// A control for logging in a user.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class LoginControl : UserControl, IViewFor<LoginViewModel>
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
                       .SelectMany(result =>
                       {
                           var dialogResult = Task.Run(() => result ?
                               MessageBox.Show("Welcome!", "Login Successful") :
                               MessageBox.Show("Ah, ah, ah, you didn't say the magic word!", "Login Failed"));

                           return dialogResult.ToObservable();
                       })
                       .Subscribe()
                       .DisposeWith(disposables);
                   });
        }

        /// <summary>
        /// Gets or sets the login view model.
        /// </summary>
        public LoginViewModel ViewModel { get; set; }

        /// <inheritdoc />
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (LoginViewModel)value;
        }
    }
}
