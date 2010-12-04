using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReactiveXaml;
using ReactiveXaml.iOS;

namespace ReactiveXaml.Sample.iOS
{
	// The name AppDelegateIPhone is referenced in the MainWindowIPhone.xib file.
	public partial class AppDelegateIPhone : UIApplicationDelegateRx
	{
		public AppDelegateIPhone()
		{
			
		}
		
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.MakeKeyAndVisible();
			var sbDelegate = new UITextFieldDelegateRx();
			this.searchBox.Delegate = sbDelegate;
			
			sbDelegate.OnEditingStarted.Subscribe(_ => {
				throw new Exception("Die!");
			});
			return true;
		}
	}
}

