// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using IntegrationTests.Shared;
using ReactiveUI;
using UIKit;

#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable CA1010 // Collections should implement generic interface
#pragma warning disable SA1300 // Element should begin with upper-case letter

namespace IntegrationTests.iOS
{
    /// <summary>
    /// The main login view controller for the application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class LoginViewController : ReactiveViewController<LoginViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewController"/> class.
        /// </summary>
        /// <param name="handle">The handle to the controller instance.</param>
        public LoginViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <inheritdoc />
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.UserName, v => v.UsernameField.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Password, v => v.PasswordField.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Login, v => v.LoginButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.CancelButton)
                    .DisposeWith(disposables);

                ViewModel.Login
                         .SelectMany(result =>
                         {
                             UIAlertController alert;

                             if (result)
                             {
                                 alert = UIAlertController.Create("Login Successful", "Welcome!", UIAlertControllerStyle.Alert);
                             }
                             else
                             {
                                 alert = UIAlertController.Create("Login Failed", "Ah, ah, ah, you didn't say the magic word!", UIAlertControllerStyle.Alert);
                             }

                             alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                             PresentViewController(alert, true, null);

                             return Observable.Return(Unit.Default);
                         })
                         .Subscribe()
                         .DisposeWith(disposables);
            });
        }
    }
}
