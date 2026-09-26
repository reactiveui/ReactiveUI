// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Reads the timetable through the dependency itself instead of through <c>this</c>. It behaves the same as
/// <see cref="PreferLessonReminderViewModel"/> here, but ties the pipeline to <see cref="Timetable"/>'s lifetime
/// instead of to this view model's, which matters once <see cref="Timetable"/> is a long-lived singleton.
/// </summary>
[System.Diagnostics.DebuggerDisplay("NextClass = {NextClass}")]
public sealed class AvoidLessonReminderViewModel : ReactiveObject, IDisposable
{
    /// <summary>Backs <see cref="NextClass"/>.</summary>
    private readonly ObservableAsPropertyHelper<string> _nextClass;

    /// <summary>Initializes a new instance of the <see cref="AvoidLessonReminderViewModel"/> class.</summary>
    /// <param name="timetable">The school's timetable, a dependency this view model does not own.</param>
    public AvoidLessonReminderViewModel(Timetable timetable) =>
        _nextClass = timetable.WhenAny(x => x.NextClass, static change => change.Value)
            .ToProperty(this, nameof(NextClass));

    /// <summary>Gets the next class on the timetable.</summary>
    public string NextClass => _nextClass.Value;

    /// <inheritdoc/>
    public void Dispose() => _nextClass.Dispose();
}
