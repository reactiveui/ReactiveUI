using System;
using Android.App;
using Android.Content;
using Android.Views;
using Java.Interop;

namespace ReactiveUI.Android
{
    public class PlatformOperations : IPlatformOperations
    {
        public static string GetOrientation()
        {
            var wm = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            if (wm == null) return null;

            var disp = wm.DefaultDisplay;
            if (disp == null) return null;

            return disp.Rotation.ToString();
        }

        public static DeviceOrientation GetOrientationEnum()
        {
            var wm = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            if (wm == null) return DeviceOrientation.None;

            var disp = wm.DefaultDisplay;
            if (disp == null) return DeviceOrientation.None;

            if (disp.Orientation == 0 || disp.Orientation == 2)
            {
                return DeviceOrientation.Portrait;
            }
            if (disp.Orientation == 1 || disp.Orientation == 3)
            {
                return DeviceOrientation.Landscape;
            }
            return DeviceOrientation.None;
        }

        DeviceOrientation IPlatformOperations.GetOrientationEnum()
        {
            return GetOrientationEnum();
        }

        string IPlatformOperations.GetOrientation()
        {
            return GetOrientation();
        }
    }
}

