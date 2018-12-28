// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;

#if UIKIT
using UIKit;
using NSView = UIKit.UIView;
using NSViewController = UIKit.UIViewController;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    internal static class UIViewControllerMixins
    {
        internal static void ActivateSubviews(this NSViewController @this, bool activate)
        {
            @this.View.ActivateSubviews(activate);
        }

        private static void ActivateSubviews(this NSView @this, bool activate)
        {
            foreach (var view in @this.Subviews)
            {
                var subview = view as ICanForceManualActivation;

                if (subview != null)
                {
                    subview.Activate(activate);
                }

                view.ActivateSubviews(activate);
            }
        }
    }
}
