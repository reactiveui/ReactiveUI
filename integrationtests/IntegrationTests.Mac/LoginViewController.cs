// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AppKit;
using Foundation;
using IntegrationTests.Shared;
using ReactiveUI;

namespace IntegrationTests.Mac
{
    /// <summary>
    /// A controller responsible for logging in the user.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class LoginViewController : ReactiveViewController<LoginViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewController"/> class.
        /// </summary>
        /// <param name="handle">The handle for the control.</param>
        public LoginViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Gets or sets the represented object.
        /// </summary>
        public override NSObject RepresentedObject
        {
            get => base.RepresentedObject;
            set
            {
                base.RepresentedObject = value;

                // Update the view, if already loaded.
            }
        }

        /// <inheritdoc />
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);

            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.UserName, v => v.UsernameField.StringValue, username => username ?? string.Empty, username => username)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Password, v => v.PasswordField.StringValue, password => password ?? string.Empty, password => password)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Login, v => v.LoginButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.CancelButton)
                    .DisposeWith(disposables);

                ViewModel.Login
                         .SelectMany(result =>
                         {
                             var alert = new NSAlert();

                             if (result)
                             {
                                 alert.AlertStyle = NSAlertStyle.Informational;
                                 alert.MessageText = "Login Successful";
                                 alert.InformativeText = "Welcome!";
                             }
                             else
                             {
                                 alert.AlertStyle = NSAlertStyle.Critical;
                                 alert.MessageText = "Login Failed";
                                 alert.InformativeText = "Ah, ah, ah, you didn't say the magic word!";
                             }

                             alert.RunModal();

                             return Observable.Return(Unit.Default);
                         })
                         .Subscribe()
                         .DisposeWith(disposables);
            });
        }
    }
}
