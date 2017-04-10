namespace ReactiveUI
{
    /// <summary>
    /// Returns the current orientation of the device on iOS.
    /// </summary>
    public class PlatformOperations : IPlatformOperations
    {
        public string GetOrientation()
        {
#if UNIFIED && UIKIT
            return UIKit.UIDevice.CurrentDevice.Orientation.ToString();
#elif UIKIT
            return MonoTouch.UIKit.UIDevice.CurrentDevice.Orientation.ToString();
#else
            return null;
#endif
        }
    }
}
