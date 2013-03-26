using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ActionbarSherlock.App;

namespace AndroidPlayground
{
    [Activity (Label = "AndroidPlayground", MainLauncher = true)]
    public class Activity1 : SherlockActivity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);
            
            button.Click += (o,e) =>
            {

            };

            ActionBar.Title = "Bar";
        }
    }
}


