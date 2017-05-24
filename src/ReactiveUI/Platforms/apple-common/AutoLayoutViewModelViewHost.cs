using System;
#if UIKIT
using NSView = UIKit.UIView;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// Use this class instead of <see cref="ViewModelViewHost"/> when
    /// taking advantage of Auto Layout. This will automatically wire
    /// up edge constraints for you from the parent view (the target)
    /// to the child subview.
    /// </summary>
    [Obsolete("Use ViewModelViewHost instead. This class will be removed in a future release.")]
    public class AutoLayoutViewModelViewHostLegacy : ViewModelViewHostLegacy
    {
        public AutoLayoutViewModelViewHostLegacy(NSView targetView) : base(targetView)
        {
            AddAutoLayoutConstraintsToSubView = true;
        }
    }
}
