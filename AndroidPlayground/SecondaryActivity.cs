
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ActionbarSherlock.App;

namespace AndroidPlayground
{
    [Activity (Label = "SecondaryActivity")]
    public class SecondaryActivity : SherlockActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Secondary);
        }
    }
}

