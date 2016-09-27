using System;
using ReactiveUI;
using Xamarin.Forms;

namespace PlaygroundXamForms
{
	public class App : Application
	{

        public App()
        {
            var bootstrapper = RxApp.SuspensionHost.GetAppState<AppBootstrapper>();


            // The root page of your application
            MainPage = bootstrapper.CreateMainPage();
        }

	}
}

