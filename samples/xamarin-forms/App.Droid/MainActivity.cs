// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using Android.App;
using Android.Widget;
using Android.OS;
using Serilog;

namespace App.Droid
{
    [Activity(Label = "App.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var log = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .WriteTo.AndroidLog()
                .CreateLogger();

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

