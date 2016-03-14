using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using ReactiveUI;
using System.Diagnostics;

namespace XamarinMacPlayground
{
    public partial class MainWindow : MonoMac.AppKit.NSWindow
    {
		#region Constructors
		
        // Called when created from unmanaged code
        public MainWindow (IntPtr handle) : base (handle)
        {
            Initialize ();
        }
		
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public MainWindow (NSCoder coder) : base (coder)
        {
            Initialize ();

        }
		
        // Shared initialization code
        void Initialize ()
        {
            Console.WriteLine ("Foo");

            this.WhenAny (x => x.Frame, x => x.Value).Subscribe (x => {
                Console.WriteLine("Changed!");
            });
        }
		
		#endregion
    }
}

