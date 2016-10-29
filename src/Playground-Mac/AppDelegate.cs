using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using ReactiveUI;

namespace XamarinMacPlayground
{
    public partial class AppDelegate : NSApplicationDelegate
    {
        MainWindowController mainWindowController;
		
        public AppDelegate ()
        {
        }

        public override void FinishedLaunching (NSObject notification)
        {
            RxApp.MutableResolver.Register(() => new TestViewController(), typeof(IViewFor<TestViewModel>));

            mainWindowController = new MainWindowController ();
            mainWindowController.Window.MakeKeyAndOrderFront (this);
        }
    }
}