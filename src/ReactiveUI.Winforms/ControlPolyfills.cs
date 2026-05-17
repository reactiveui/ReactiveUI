// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Winforms;

internal static class ControlPolyfills
{
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
