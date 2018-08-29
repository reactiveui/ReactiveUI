// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoLayoutViewModelViewHostLegacy"/> class.
        /// </summary>
        /// <param name="targetView">The target ns view.</param>
        public AutoLayoutViewModelViewHostLegacy(NSView targetView)
            : base(targetView)
        {
            AddAutoLayoutConstraintsToSubView = true;
        }
    }
}
