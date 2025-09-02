// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// UI Kit command binder platform registrations.
/// </summary>
/// <seealso cref="ICreatesCommandBinding" />
[Preserve(AllMembers = true)]
public class UIKitCommandBinders : FlexibleCommandBinder
{
    private const string Enabled = nameof(Enabled);

    /// <summary>
    /// Initializes a new instance of the <see cref="UIKitCommandBinders"/> class.
    /// </summary>
    [SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Marked as Preserve")]
    public UIKitCommandBinders()
    {
        Register(typeof(UIControl), 9, (cmd, t, cp) => ForTargetAction(cmd, t, cp, typeof(UIControl).GetRuntimeProperty(Enabled) ?? throw new InvalidOperationException("There is no Enabled property on the UIControl which is needed for binding.")));
        Register(typeof(UIRefreshControl), 10, (cmd, t, cp) => ForEvent(cmd, t, cp, "ValueChanged", typeof(UIRefreshControl).GetRuntimeProperty(Enabled) ?? throw new InvalidOperationException("There is no Enabled property on the UIRefreshControl which is needed for binding.")));
        Register(typeof(UIBarButtonItem), 10, (cmd, t, cp) => ForEvent(cmd, t, cp, "Clicked", typeof(UIBarButtonItem).GetRuntimeProperty(Enabled) ?? throw new InvalidOperationException("There is no Enabled property on the UIBarButtonItem which is needed for binding.")));
    }

    /// <summary>
    /// Gets the UIKitCommandBinders instance.
    /// </summary>
    public static Lazy<UIKitCommandBinders> Instance { get; } = new();
}
