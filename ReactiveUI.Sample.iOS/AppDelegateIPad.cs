using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveUI;
using ReactiveUI.iOS;

namespace ReactiveUI.Sample.iOS
{
	// The name AppDelegateIPad is referenced in the MainWindowIPad.xib file.
	public partial class AppDelegateIPad : UIApplicationDelegateRx
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// If you have defined a view, add it here:
			// window.AddSubview (navigationController.View);
			
			window.MakeKeyAndVisible ();
			
			return true;
		}
	}
}

