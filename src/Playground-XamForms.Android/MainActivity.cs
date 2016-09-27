using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;
using ReactiveUI;
using ReactiveUI.Legacy;


namespace PlaygroundXamForms.Android
{
    [Activity (Label = "Playground-XamForms.Android.Android", MainLauncher = true)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
            Xamarin.Forms.Forms.Init (this, bundle);

            // NB: This is the worst way ever to handle UserErrors and definitely *not*
            // best practice. Help your users out!
            // TODO
            UserError.RegisterHandler(ue => {
                var toast = Toast.MakeText(this, ue.ErrorMessage, ToastLength.Short);
                toast.Show();
                return Observable.Return(RecoveryOptionResult.CancelOperation);
            });

            LoadApplication(new App());
        }
    }
}

