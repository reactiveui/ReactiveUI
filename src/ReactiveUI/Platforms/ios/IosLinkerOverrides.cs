// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Cocoa;
#else
namespace ReactiveUI.Cocoa;
#endif

/// <summary>Forces the linker to preserve iOS-only UIKit members not present on every Apple platform.</summary>
[Preserve(AllMembers = true)]
[SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "Locals force the linker to preserve these members.")]
internal class IosLinkerOverrides
{
    /// <summary>Forces the linker to preserve iOS-only UIKit members (UISlider, UIRefreshControl, UISwitch).</summary>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by linker.")]
    public void KeepMe()
    {
        // UISlider
        var slider = new UISlider { Value = default };
        _ = slider.Value;

        // UIRefreshControl
        var rc = new UIRefreshControl();
        rc.ValueChanged += LinkerOverrides.Eh;
        rc.ValueChanged -= LinkerOverrides.Eh;

        // UISwitch
        var sw = new UISwitch();
        sw.ValueChanged += LinkerOverrides.Eh;
        sw.On = true;
    }
}
