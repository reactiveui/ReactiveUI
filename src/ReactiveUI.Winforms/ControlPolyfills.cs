// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if !NET6_0_OR_GREATER
using ReactiveUI.Helpers;
#endif

namespace ReactiveUI.Winforms;

/// <summary>
/// Polyfills for <see cref="Control"/> members that are unavailable on the older target frameworks.
/// </summary>
internal static class ControlPolyfills
{
    /// <summary>
    /// Determines whether the control, or any of its ancestors, is hosted in a design-mode site.
    /// </summary>
    /// <param name="control">The control to inspect.</param>
    /// <returns>true if the control or one of its ancestors is in design mode; otherwise, false.</returns>
    public static bool GetIsAncestorSiteInDesignMode(this Control control)
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
