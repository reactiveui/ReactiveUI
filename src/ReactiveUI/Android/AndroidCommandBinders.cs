using System;
using System.Reflection;
using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// Android Command Binders
    /// </summary>
    public class AndroidCommandBinders : FlexibleCommandBinder
    {
        /// <summary>
        /// The instance
        /// </summary>
        public static Lazy<AndroidCommandBinders> Instance = new Lazy<AndroidCommandBinders>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidCommandBinders"/> class.
        /// </summary>
        public AndroidCommandBinders()
        {
            Type view = typeof(View);
            Register(view, 9, (cmd, t, cp) => ForEvent(cmd, t, cp, "Click", view.GetRuntimeProperty("Enabled")));
        }
    }
}