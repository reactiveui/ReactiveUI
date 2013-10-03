using System;
using Android.Views;

namespace ReactiveUI.Android.Android
{
    public class AndroidCommandBinders : FlexibleCommandBinder
    {
        public static Lazy<AndroidCommandBinders> Instance = new Lazy<AndroidCommandBinders>();

        public AndroidCommandBinders()
        {
            Register(typeof(View), 9, (cmd, t, cp)=> ForEvent(cmd, t, cp, "Click", "Enabled"));
        }
    }
}