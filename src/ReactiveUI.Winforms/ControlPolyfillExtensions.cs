// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Winforms;
#else
namespace ReactiveUI.Winforms;
#endif

/// <summary>Polyfills for <see cref="Control"/> members that are unavailable on the older target frameworks.</summary>
internal static class ControlPolyfillExtensions
{
    /// <summary>Provides polyfill extension methods for <see cref="Control"/>.</summary>
    /// <param name="control">The control.</param>
    extension(Control control)
    {
        /// <summary>Determines whether the control, or any of its ancestors, is hosted in a design-mode site.</summary>
        /// <returns>true if the control or one of its ancestors is in design mode; otherwise, false.</returns>
        public bool GetIsAncestorSiteInDesignMode()
        {
#if NET6_0_OR_GREATER
            return control.IsAncestorSiteInDesignMode;
#else
            ArgumentExceptionHelper.ThrowIfNull(control);

            if (control.Site is { DesignMode: true })
            {
                return true;
            }
            else if (control.Parent is null)
            {
                return false;
            }
            else
            {
                return control.Parent.GetIsAncestorSiteInDesignMode();
            }
#endif
        }
    }
}
