// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Reads the timetable through <c>this</c>, the pattern this guideline recommends: the pipeline is written against
/// this view model's own property, not against <see cref="Timetable"/> directly.
/// </summary>
[System.Diagnostics.DebuggerDisplay("NextClass = {NextClass}")]
public sealed class PreferLessonReminderViewModel : ReactiveObject, IDisposable
{
    /// <summary>Backs <see cref="NextClass"/>.</summary>
    private readonly ObservableAsPropertyHelper<string> _nextClass;

    /// <summary>Initializes a new instance of the <see cref="PreferLessonReminderViewModel"/> class.</summary>
    /// <param name="timetable">The school's timetable, a dependency this view model does not own.</param>
    public PreferLessonReminderViewModel(Timetable timetable)
    {
        Timetable = timetable;
        _nextClass = this.WhenAny(x => x.Timetable.NextClass, static change => change.Value)
            .ToProperty(this, nameof(NextClass));
    }

    /// <summary>Gets the timetable this view model reads.</summary>
    public Timetable Timetable { get; }

    /// <summary>Gets the next class on the timetable.</summary>
    public string NextClass => _nextClass.Value;

    /// <inheritdoc/>
    public void Dispose() => _nextClass.Dispose();
}
