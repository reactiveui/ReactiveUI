using System;
using Android.App;
using Android.Content;
using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// Platform Operations
    /// </summary>
    /// <seealso cref="ReactiveUI.IPlatformOperations"/>
    public class PlatformOperations : IPlatformOperations
    {
        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns></returns>
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