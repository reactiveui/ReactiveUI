// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI;
using Xamarin.Forms;

namespace IntegrationTests.XamarinForms
{
    /// <summary>
    /// The class which represents the main application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(
                    new MainPage()
                    {
                        ViewModel = new Shared.LoginViewModel(RxApp.MainThreadScheduler)
                    });
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        /// <inheritdoc />
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <inheritdoc />
        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
