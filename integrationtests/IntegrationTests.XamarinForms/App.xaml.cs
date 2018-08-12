using ReactiveUI;
using Splat;
using System;
using Xamarin.Forms;

namespace IntegrationTests.XamarinForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = 
                new NavigationPage(
                    new MainPage()
                    {
                        ViewModel = new Shared.LoginViewModel(RxApp.MainThreadScheduler)
                    });
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
