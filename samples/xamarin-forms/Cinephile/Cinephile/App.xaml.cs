// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Net.Http;
using FFImageLoading;
using FFImageLoading.Config;
using Fusillade;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Cinephile
{
    /// <summary>
    /// The main application instance.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();

            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = new HttpClient(new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Background))
            });

            new AppBootstrapper();
            MainPage = AppBootstrapper.CreateMainPage();

            // I hate to do this, but honestly dont know a better way to styke the navbar
            ((NavigationPage)Current.MainPage).Style = Current.Resources["DefaultNavigationPageStyle"] as Style;
        }

        /// <inheritdoc/>
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        /// <inheritdoc/>
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <inheritdoc/>
        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
