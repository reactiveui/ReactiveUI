
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using ReactiveUI;

namespace XamarinMacPlayground
{
    public partial class TestViewController : NSViewController, IViewFor<TestViewModel>
    {
        // Called when created from unmanaged code
        public TestViewController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
		
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public TestViewController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
		
        // Call to load from the XIB/NIB file
        public TestViewController() : base ("TestView", NSBundle.MainBundle)
        {
            Initialize();
        }
		
        // Shared initialization code
        void Initialize()
        {
        }
		
        //strongly typed view accessor
        public new TestView View {
            get { return (TestView)base.View; }
        }

        public TestViewModel ViewModel { get; set; }

        object IViewFor.ViewModel {
            get { return this.ViewModel; }
            set { this.ViewModel = (TestViewModel)value;}
        }
    }

    public class TestViewModel : ReactiveObject
    {
    }
}

