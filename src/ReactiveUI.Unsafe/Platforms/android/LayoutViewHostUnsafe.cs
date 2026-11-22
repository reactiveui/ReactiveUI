// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Views;
#if REACTIVE_SHIM
using static ReactiveUI.Reactive.ControlFetcherMixins;
#else
using static ReactiveUI.ControlFetcherMixins;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>A <see cref="LayoutViewHost"/> that can wire its child controls to its properties by reflection.</summary>
/// <remarks>
/// <para>
/// Auto-wireup finds each property of the derived host by reflection and looks up the matching control id in the
/// app's generated Android resource type. The trimmer cannot see either lookup, so this type is not safe to trim or
/// to compile ahead of time.
/// </para>
/// <para>
/// Use <see cref="LayoutViewHost"/> and wire the controls in its <c>bind</c> callback when you can.
/// </para>
/// </remarks>
[RequiresUnreferencedCode(AndroidWireupMessages.UnsafeWireup)]
[RequiresDynamicCode(AndroidWireupMessages.UnsafeWireup)]
[System.Diagnostics.DebuggerDisplay("{View}")]
public class LayoutViewHostUnsafe : LayoutViewHost
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayoutViewHostUnsafe"/> class by inflating a layout resource and
    /// optionally wiring child controls to properties by reflection.
    /// </summary>
    /// <param name="context">The Android context.</param>
    /// <param name="layoutId">The layout resource identifier.</param>
    /// <param name="parent">The parent view group.</param>
    /// <param name="attachToRoot">Whether to attach the inflated view to the parent.</param>
    /// <param name="performAutoWireup">If <see langword="true"/>, wires child controls to properties by reflection.</param>
    /// <param name="resolveStrategy">Which properties the auto-wireup considers.</param>
    protected LayoutViewHostUnsafe(
        Context context,
        int layoutId,
        ViewGroup parent,
        bool attachToRoot,
        bool performAutoWireup,
        ResolveStrategy resolveStrategy)
        : base(context, layoutId, parent, attachToRoot)
    {
        if (!performAutoWireup)
        {
            return;
        }

        this.WireUpControls(resolveStrategy);
    }
}
