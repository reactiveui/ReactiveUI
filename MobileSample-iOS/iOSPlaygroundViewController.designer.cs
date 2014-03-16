// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace iOSPlayground
{
	[Register ("iOSPlaygroundViewController")]
	partial class iOSPlaygroundViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel TheGuid { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TheGuid != null) {
				TheGuid.Dispose ();
				TheGuid = null;
			}
		}
	}
}
