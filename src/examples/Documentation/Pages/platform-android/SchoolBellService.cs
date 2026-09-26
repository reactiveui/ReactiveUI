// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using Android.OS;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>A second, unrelated local service, bound only to show the weakly-typed (raw <see cref="IBinder"/>)
/// side of <see cref="ContextExtensions.ServiceBound(Context, Intent, Bind)"/>, as opposed to
/// <see cref="AttendanceTrackerService"/>'s strongly-typed binder.</summary>
[Service(Exported = false)]
[System.Diagnostics.DebuggerDisplay("SchoolBellService")]
public sealed class SchoolBellService : Service
{
    /// <inheritdoc/>
    public override IBinder OnBind(Intent? intent) => new Binder();
}
