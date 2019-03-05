// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http;
using FFImageLoading;
using FFImageLoading.Config;
using Fusillade;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Cinephile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = new HttpClient(new RateLimitedHttpMessageHandler(new HttpClientHandler(), Priority.Background))
            });

            var bootstrapper = new AppBootstrapper();
            MainPage = bootstrapper.CreateMainPage();

            // I hate to do this, but honestly dont know a better way to styke the navbar
            (Current.MainPage as NavigationPage).Style = Current.Resources["DefaultNavigationPageStyle"] as Style;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
