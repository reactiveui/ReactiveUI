using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace IntegrationTests.XamarinForms.iOS
{
    /// <summary>
    /// The class which hosts the main entry point to the application.
    /// </summary>
    public static class AppLaunch
    {
        /// <summary>
        /// The main entry point to the application.
        /// </summary>
        /// <param name="args">Arguments that are passed to the application from the command line.</param>
        internal static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
