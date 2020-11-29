// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Android.App;
using Android.Content;
using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// Returns the current orientation of the device on Android.
    /// </summary>
    public class PlatformOperations : IPlatformOperations
    {
        /// <inheritdoc/>
        public string? GetOrientation()
        {
            if (!(Application.Context.GetSystemService(Context.WindowService) is IWindowManager wm))
            {
                return null;
            }

            var disp = wm.DefaultDisplay;
            if (disp is null)
            {
                return null;
            }

            return disp.Rotation.ToString();
        }
    }
}
