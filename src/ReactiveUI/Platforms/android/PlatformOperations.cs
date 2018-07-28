// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Android.App;
using Android.Content;
using Android.Views;

namespace ReactiveUI
{
    public class PlatformOperations : IPlatformOperations
    {
        public string GetOrientation()
        {
            var wm = Application.Context.GetSystemService(Context.WindowService) as IWindowManager;
            if (wm == null) return null;

            var disp = wm.DefaultDisplay;
            if (disp == null) return null;

            return disp.Rotation.ToString();
        }
    }
}

