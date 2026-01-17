// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#nullable enable

using System.Reflection;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// UI Kit command binder platform registrations.
/// </summary>
[Preserve(AllMembers = true)]
public sealed class UIKitCommandBinders : FlexibleCommandBinder
{
    /// <summary>
    /// The reflected property name used to control enabled state across UIKit types.
    /// </summary>
    private const string EnabledPropertyName = "Enabled";

    /// <summary>
    /// Cached <see cref="PropertyInfo"/> for <see cref="UIControl.Enabled"/>.
    /// </summary>
    private static readonly PropertyInfo UIControlEnabledProperty =
        typeof(UIControl).GetRuntimeProperty(EnabledPropertyName) ??
        throw new InvalidOperationException("There is no Enabled property on UIControl which is needed for binding.");

    /// <summary>
    /// Cached <see cref="PropertyInfo"/> for <see cref="UIControl.Enabled"/>.
    /// </summary>
    private static readonly PropertyInfo UIRefreshControlEnabledProperty =
        typeof(UIRefreshControl).GetRuntimeProperty(EnabledPropertyName) ??
        throw new InvalidOperationException("There is no Enabled property on UIRefreshControl which is needed for binding.");

    /// <summary>
    /// Cached <see cref="PropertyInfo"/> for <see cref="UIBarButtonItem.Enabled"/>.
    /// </summary>
    private static readonly PropertyInfo UIBarButtonItemEnabledProperty =
        typeof(UIBarButtonItem).GetRuntimeProperty(EnabledPropertyName) ??
        throw new InvalidOperationException("There is no Enabled property on UIBarButtonItem which is needed for binding.");

    /// <summary>
    /// Initializes a new instance of the <see cref="UIKitCommandBinders"/> class.
    /// </summary>
    public UIKitCommandBinders()
    {
        // UIControl: prefer the AOT-safe target-action helper (no string event name).
        Register(typeof(UIControl), 9, static (cmd, t, cp) => ForTargetAction(cmd, t, cp, UIControlEnabledProperty));

        // UIRefreshControl: ValueChanged is a .NET event; use the AOT-safe ForEvent overload via add/remove delegates.
        Register(typeof(UIRefreshControl), 10, (cmd, t, cp) =>
            ForEvent(
                cmd,
                (UIRefreshControl)t!,
                cp,
                addHandler: h => ((UIRefreshControl)t!).ValueChanged += h,   // see note below
                removeHandler: h => ((UIRefreshControl)t!).ValueChanged -= h,
                UIRefreshControlEnabledProperty));

        // UIBarButtonItem: Clicked is a .NET event; use the AOT-safe ForEvent overload via add/remove delegates.
        Register(typeof(UIBarButtonItem), 10, (cmd, t, cp) =>
            ForEvent(
                cmd,
                (UIBarButtonItem)t!,
                cp,
                addHandler: h => ((UIBarButtonItem)t!).Clicked += h,
                removeHandler: h => ((UIBarButtonItem)t!).Clicked -= h,
                UIBarButtonItemEnabledProperty));
    }

    /// <summary>
    /// Gets a lazily-initialized singleton instance of <see cref="UIKitCommandBinders"/>.
    /// </summary>
    public static Lazy<UIKitCommandBinders> Instance { get; } = new();
}
