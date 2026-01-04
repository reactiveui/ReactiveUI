// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// UIKit command binder platform registrations.
/// </summary>
/// <remarks>
/// <para>
/// This binder registers UIKit-specific command bindings using AOT-friendly event subscription
/// where possible (explicit add/remove handler delegates) and avoids string/reflection-based
/// event hookup.
/// </para>
/// <para>
/// Enabled-state synchronization uses a cached <see cref="PropertyInfo"/> for the platform
/// <c>Enabled</c> property and is performed via the shared infrastructure in
/// <see cref="FlexibleCommandBinder"/>.
/// </para>
/// </remarks>
[Preserve(AllMembers = true)]
public sealed class UIKitCommandBinders : FlexibleCommandBinder
{
    /// <summary>
    /// Cached <c>Enabled</c> property for <see cref="UIControl"/> (used by <see cref="UIControlEnabledProperty"/>).
    /// </summary>
    private static readonly PropertyInfo UIControlEnabledProperty =
        typeof(UIControl).GetRuntimeProperty(nameof(UIControl.Enabled))
        ?? throw new InvalidOperationException("There is no Enabled property on UIControl which is required for binding.");

    /// <summary>
    /// Cached <c>Enabled</c> property for <see cref="UIBarButtonItem"/>.
    /// </summary>
    private static readonly PropertyInfo UIBarButtonItemEnabledProperty =
        typeof(UIBarButtonItem).GetRuntimeProperty(nameof(UIBarButtonItem.Enabled))
        ?? throw new InvalidOperationException("There is no Enabled property on UIBarButtonItem which is required for binding.");

    /// <summary>
    /// Initializes a new instance of the <see cref="UIKitCommandBinders"/> class.
    /// </summary>
    public UIKitCommandBinders()
    {
        // UIControl uses UIKit target-action rather than .NET events.
        Register(
            typeof(UIControl),
            affinity: 9,
            static (cmd, t, cp) => ForTargetAction(cmd, t, cp, UIControlEnabledProperty));

        // UIBarButtonItem exposes a normal .NET event ("Clicked"). Use explicit add/remove to avoid reflection.
        Register(
            typeof(UIBarButtonItem),
            affinity: 10,
            static (cmd, t, cp) =>
            {
                if (t is not UIBarButtonItem item)
                {
                    return Disposable.Empty;
                }

                return ForEvent(
                    command: cmd,
                    target: item,
                    commandParameter: cp,
                    addHandler: h => item.Clicked += h,
                    removeHandler: h => item.Clicked -= h,
                    enabledProperty: UIBarButtonItemEnabledProperty);
            });
    }

    /// <summary>
    /// Gets the shared <see cref="UIKitCommandBinders"/> instance.
    /// </summary>
    public static Lazy<UIKitCommandBinders> Instance { get; } = new();
}
