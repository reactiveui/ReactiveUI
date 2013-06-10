using System;

namespace ReactiveUI.Cocoa
{
    /// <summary>
    /// Returns the current orientation of the device on iOS.
    /// </summary>
    public class PlatformOperations : IPlatformOperations
    {
        public string GetOrientation()
        {
#if UIKIT
            return MonoTouch.UIKit.UIDevice.CurrentDevice.Orientation.ToString();
#else
            return null;
#endif
        }
    }
}

