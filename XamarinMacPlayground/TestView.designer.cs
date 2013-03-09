
namespace XamarinMacPlayground
{
	
    // Should subclass MonoMac.AppKit.NSView
    [MonoMac.Foundation.Register("TestView")]
    public partial class TestView
    {
    }
	
    // Should subclass MonoMac.AppKit.NSViewController
    [MonoMac.Foundation.Register("TestViewController")]
    public partial class TestViewController
    {
    }
}

