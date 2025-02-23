// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using UIKit;

namespace ReactiveUI;

/// <summary>
/// UI Kit command binder platform registrations.
/// </summary>
/// <seealso cref="ReactiveUI.ICreatesCommandBinding" />
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public class UIKitCommandBinders : FlexibleCommandBinder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIKitCommandBinders"/> class.
    /// </summary>
    public UIKitCommandBinders()
    {
        Register(typeof(UIControl), 9, (cmd, t, cp) => ForTargetAction(cmd, t, cp, typeof(UIControl).GetRuntimeProperty("Enabled") ?? throw new InvalidOperationException("There is no Enabled property on the UIControl which is needed for binding.")));
        Register(typeof(UIBarButtonItem), 10, (cmd, t, cp) => ForEvent(cmd, t, cp, "Clicked", typeof(UIBarButtonItem).GetRuntimeProperty("Enabled") ?? throw new InvalidOperationException("There is no Enabled property on the UIBarButtonItem which is needed for binding.")));
    }

    /// <summary>
    /// Gets the UIKitCommandBinders instance.
    /// </summary>
    public static Lazy<UIKitCommandBinders> Instance { get; } = new();
}
