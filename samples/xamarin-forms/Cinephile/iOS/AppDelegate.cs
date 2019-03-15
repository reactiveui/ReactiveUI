// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Akavache;
using Foundation;
using UIKit;

namespace Cinephile.iOS
{
    /// <summary>
    /// The application delegate for the application.
    /// </summary>
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        /// <inheritdoc/>
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);

            Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            BlobCache.ApplicationName = "Cinephile";
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            return base.FinishedLaunching(app, options);
        }

        /// <inheritdoc/>
        public override void WillTerminate(UIApplication uiApplication)
        {
            BlobCache.Shutdown();
            base.WillTerminate(uiApplication);
        }
    }
}
