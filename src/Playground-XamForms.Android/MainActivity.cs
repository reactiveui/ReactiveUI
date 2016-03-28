using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;
using ReactiveUI;


namespace PlaygroundXamForms.Android
{
    [Activity (Label = "Playground-XamForms.Android.Android", MainLauncher = true)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            Xamarin.Forms.Forms.Init (this, bundle);

            var view = RxApp.SuspensionHost.GetAppState<AppBootstrapper>().CreateMainView();
            SetPage(view);
        }
    }
}

