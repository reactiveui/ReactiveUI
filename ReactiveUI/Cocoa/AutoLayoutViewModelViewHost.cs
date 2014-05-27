#if UIKIT
using NSView = MonoTouch.UIKit.UIView;
#else
using MonoMac.AppKit;
#endif

namespace ReactiveUI.Cocoa
{
    /// <summary>
    ///     Use this class instead of <see cref="ViewModelViewHost"/> when
    ///     taking advantage of Auto Layout. This will automatically wire
    ///     up edge constraints for you from the parent view (the target)
    ///     to the child subview.
    /// </summary>
    public class AutoLayoutViewModelViewHost : ViewModelViewHost
    {
        public AutoLayoutViewModelViewHost(NSView targetView) : base(targetView)
        {
            AddAutoLayoutConstraintsToSubView = true;
        }
    }
}
