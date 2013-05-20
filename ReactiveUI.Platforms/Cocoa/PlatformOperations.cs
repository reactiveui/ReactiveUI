using System;

namespace ReactiveUI.Cocoa
{
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

