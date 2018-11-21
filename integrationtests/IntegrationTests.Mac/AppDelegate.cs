using AppKit;
using Foundation;

namespace IntegrationTests.Mac
{
    /// <summary>
    /// The main application delegate.
    /// </summary>
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        /// <inheritdoc />
        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
        }

        /// <inheritdoc />
        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        /// <inheritdoc />
        [Export("applicationShouldTerminateAfterLastWindowClosed:")]
        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}
