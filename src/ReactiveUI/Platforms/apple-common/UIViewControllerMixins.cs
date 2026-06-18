// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if UIKIT
using NSView = UIKit.UIView;
using NSViewController = UIKit.UIViewController;
#else
using AppKit;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Extension methods for activating and deactivating subviews on a view controller.</summary>
internal static class UIViewControllerMixins
{
    /// <summary>Provides subview activation extension members for a view.</summary>
    /// <param name="masterView">The view whose subviews to activate or deactivate.</param>
    extension(NSView masterView)
    {
        /// <summary>Recursively activates or deactivates all subviews of the given view.</summary>
        /// <param name="activate"><see langword="true"/> to activate subviews; <see langword="false"/> to deactivate.</param>
        private void ActivateSubviews(bool activate)
        {
            ArgumentExceptionHelper.ThrowIfNull(masterView);

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

    /// <summary>Provides subview activation extension members for a view controller.</summary>
    /// <param name="controller">The view controller whose subviews to activate or deactivate.</param>
    extension(NSViewController controller)
    {
        /// <summary>Recursively activates or deactivates all subviews of the given view controller's root view.</summary>
        /// <param name="activate"><see langword="true"/> to activate subviews; <see langword="false"/> to deactivate.</param>
        internal void ActivateSubviews(bool activate)
        {
            ArgumentExceptionHelper.ThrowIfNull(controller);

            if (controller.View is null)
            {
                throw new ArgumentException("The view on the controller is null.", nameof(controller));
            }

            controller.View.ActivateSubviews(activate);
        }
    }
}
