// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using Android.Views;

namespace ReactiveUI;

/// <summary>
/// Android implementation that provides binding to an ICommand in the ViewModel to a Control
/// in the View.
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
public class AndroidCommandBinders : FlexibleCommandBinder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidCommandBinders"/> class.
    /// </summary>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
#endif
    public AndroidCommandBinders() // TODO: Create Test
    {
        var view = typeof(View);
        Register(view, 9, (cmd, t, cp) => ForEvent(cmd, t, cp, "Click", view.GetRuntimeProperty("Enabled") ?? throw new InvalidOperationException("Could not find property 'Enabled' on type View, which is needed for binding")));
    }

    /// <summary>
    /// Gets the static instance of <see cref="AndroidCommandBinders"/>.
    /// </summary>
    public static Lazy<AndroidCommandBinders> Instance { get; } = new(); // TODO: Create Test
}
