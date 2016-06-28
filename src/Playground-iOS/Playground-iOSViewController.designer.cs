// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace PlaygroundiOS
{
	[Register ("Playground_iOSViewController")]
	partial class Playground_iOSViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel savedGuid { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (savedGuid != null) {
				savedGuid.Dispose ();
				savedGuid = null;
			}
		}
	}
}
