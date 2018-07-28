// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReactiveUI
{
    /// <summary>
    /// Returns the current orientation of the device on iOS.
    /// </summary>
    public class PlatformOperations : IPlatformOperations
    {
        public string GetOrientation()
        {
#if UIKIT
            return UIKit.UIDevice.CurrentDevice.Orientation.ToString();
#else
            return null;
#endif
        }
    }
}
