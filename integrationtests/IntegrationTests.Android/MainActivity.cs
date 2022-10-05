// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using IntegrationTests.Shared;
using ReactiveUI;

namespace IntegrationTests.Android
{
    /// <summary>
    /// The main activity for the application.
    /// </summary>
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    [ExcludeFromCodeCoverage]
    public class MainActivity : ReactiveActivity<LoginViewModel>
    {
        /// <summary>
        /// Gets or sets the user name edit text.
        /// </summary>
        public EditText Username { get; set; }

        /// <summary>
        /// Gets or sets the password edit text.
        /// </summary>
        public EditText Password { get; set; }

        /// <summary>
        /// Gets or sets the login button.
        /// </summary>
        public Button Login { get; set; }

        /// <summary>
        /// Gets or sets the cancel butotn.
        /// </summary>
        public Button Cancel { get; set; }

        /// <inheritdoc />
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        /// <inheritdoc />
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return item.ItemId == Resource.Id.action_settings || base.OnOptionsItemSelected(item);
        }

        /// <inheritdoc />
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);
            SetActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

            // WireUpControls looks through your layout file, finds all controls
            // with an id defined, and binds them to the controls defined in this class.
            // This is basically the same functionality as in
            // https://jakewharton.github.io/butterknife/
            this.WireUpControls();

            ViewModel = new LoginViewModel(RxApp.MainThreadScheduler);
            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.UserName, v => v.Username.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Password, v => v.Password.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.Login, v => v.Login)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.Cancel, v => v.Cancel)
                    .DisposeWith(disposables);

                ViewModel
                    .Login
                    .SelectMany(
                        result =>
                        {
                            if (result)
                            {
                                new AlertDialog.Builder(this)
                                    .SetTitle("Login Successful")
                                    .SetMessage("Welcome!")
                                    .Show();
                            }
                            else
                            {
                                new AlertDialog.Builder(this)
                                    .SetTitle("Login Failed")
                                    .SetMessage("Ah, ah, ah, you didn't say the magic word!")
                                    .Show();
                            }

                            return Observable.Return(Unit.Default);
                        })
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }
    }
}
