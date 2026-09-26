// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using Android.OS;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>A local, in-process service that records lesson absences. It never leaves the app's own process, so
/// binding to it always succeeds without a system permission prompt — a small, safe target for
/// <see cref="ContextExtensions.ServiceBound{TBinder}(Context, Intent)"/>.</summary>
[Service(Exported = false)]
[System.Diagnostics.DebuggerDisplay("AttendanceTrackerService")]
public sealed class AttendanceTrackerService : Service
{
    /// <summary>The binder handed to a bound client, exposing this service directly. Created lazily in
    /// <see cref="OnBind"/>, rather than in the constructor, so <see langword="this"/> never escapes before
    /// construction finishes.</summary>
    private AttendanceBinder? _binder;

    /// <inheritdoc/>
    public override IBinder OnBind(Intent? intent) => _binder ??= new AttendanceBinder(this);

    /// <summary>Records that a student was marked absent from a lesson.</summary>
    /// <param name="subject">The subject of the lesson the student missed.</param>
    public void RecordAbsence(string subject) =>
        TimetableLog.Info($"AttendanceTrackerService recorded an absence from {subject}.");

    /// <inheritdoc/>
    public override void OnDestroy()
    {
        _binder?.Dispose();
        _binder = null;
        base.OnDestroy();
    }

    /// <summary>Hands out the owning <see cref="AttendanceTrackerService"/> to a bound client.</summary>
    /// <param name="service">The service this binder belongs to.</param>
    [System.Diagnostics.DebuggerDisplay("AttendanceBinder")]
    public sealed class AttendanceBinder(AttendanceTrackerService service) : Binder
    {
        /// <summary>Gets the bound service instance.</summary>
        public AttendanceTrackerService Service { get; } = service;
    }
}
