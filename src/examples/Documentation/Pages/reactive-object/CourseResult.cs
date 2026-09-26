// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveObjects;

/// <summary>A finalized grade for one student in one course. Every property is set once at creation, except a moderator's note added later.</summary>
[System.Diagnostics.DebuggerDisplay("{StudentName}: {Course}, Grade = {Grade}")]
public sealed record CourseResult : ReactiveRecord
{
    /// <summary>The note a moderator has attached, if any.</summary>
    private string? _moderatorNotes;

    /// <summary>Gets the name of the student the result belongs to.</summary>
    public string StudentName { get; init; } = string.Empty;

    /// <summary>Gets the course the result belongs to.</summary>
    public string Course { get; init; } = string.Empty;

    /// <summary>Gets the recorded grade.</summary>
    public int Grade { get; init; }

    /// <summary>Gets the note a moderator has attached, if any.</summary>
    public string? ModeratorNotes => _moderatorNotes;

    /// <summary>Attaches a moderator's note while the result is still open for review.</summary>
    /// <param name="note">The note to attach.</param>
    /// <remarks><see cref="ModeratorNotes"/> has no setter, so this raises the notifications by hand, the same way <see cref="Student.AddGrade"/> does.</remarks>
    public void AddModeratorNote(string note)
    {
        this.RaisePropertyChanging(nameof(ModeratorNotes));
        _moderatorNotes = note;
        this.RaisePropertyChanged(nameof(ModeratorNotes));
    }
}
