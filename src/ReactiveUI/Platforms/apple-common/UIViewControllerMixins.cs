// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if UIKIT
using NSView = UIKit.UIView;
using NSViewController = UIKit.UIViewController;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    internal static class UIViewControllerMixins
    {
        internal static void ActivateSubviews(this NSViewController controller, bool activate)
        {
            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (controller.View is null)
            {
                throw new ArgumentException("The view on the controller is null.", nameof(controller));
            }

            controller.View.ActivateSubviews(activate);
        }

        private static void ActivateSubviews(this NSView masterView, bool activate)
        {
            if (masterView is null)
            {
                throw new ArgumentNullException(nameof(masterView));
            }

            foreach (var view in masterView.Subviews)
            {
                if (view is ICanForceManualActivation subview)
                {
                    subview.Activate(activate);
                }

                view.ActivateSubviews(activate);
            }
        }
    }
}
