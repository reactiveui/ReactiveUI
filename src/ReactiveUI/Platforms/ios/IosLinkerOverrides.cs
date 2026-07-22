// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Cocoa;
#else
namespace ReactiveUI.Cocoa;
#endif

/// <summary>Forces the linker to preserve iOS-only UIKit members not present on every Apple platform.</summary>
[Preserve(AllMembers = true)]
internal static class IosLinkerOverrides
{
    /// <summary>Forces the linker to preserve iOS-only UIKit members (UISlider, UIRefreshControl, UISwitch).</summary>
    internal static void KeepMe()
    {
        // UISlider
        using var slider = new UISlider { Value = default };
        _ = slider.Value;

        // UIRefreshControl
        using var rc = new UIRefreshControl();
        rc.ValueChanged += LinkerOverrides.Eh;
        rc.ValueChanged -= LinkerOverrides.Eh;

        // UISwitch
        using var sw = new UISwitch();
        sw.ValueChanged += LinkerOverrides.Eh;
        sw.On = true;
    }
}
