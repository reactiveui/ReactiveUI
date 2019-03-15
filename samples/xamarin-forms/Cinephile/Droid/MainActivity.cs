// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using Akavache;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading.Forms.Platform;

namespace Cinephile.Droid
{
    /// <summary>
    /// The main application activity.
    /// </summary>
    [Activity(
        Label = "Cinephile.Droid",
        Icon = "@drawable/icon",
        Theme = "@style/MyTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <inheritdoc/>
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            BlobCache.ApplicationName = "Cinephile";
            CachedImageRenderer.Init(true);

            LoadApplication(new App());
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            BlobCache.LocalMachine.Flush().Wait();
            base.OnDestroy();
        }
    }
}
