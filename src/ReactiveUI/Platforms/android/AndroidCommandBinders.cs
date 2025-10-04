// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
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
[Preserve(AllMembers = true)]
public class AndroidCommandBinders : FlexibleCommandBinder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidCommandBinders"/> class.
    /// </summary>
    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Marked as Preserve")]
    public AndroidCommandBinders()
    {
        var view = typeof(View);
        Register(view, 9, (cmd, t, cp) => ForEvent(cmd, t, cp, "Click", view.GetRuntimeProperty("Enabled") ?? throw new InvalidOperationException("Could not find property 'Enabled' on type View, which is needed for binding")));
    }
}
