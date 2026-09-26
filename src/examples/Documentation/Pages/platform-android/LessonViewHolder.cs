// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using ReactiveUI.AndroidX;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Shows one lesson in the timetable's <see cref="LessonsRecyclerAdapter"/>.</summary>
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class LessonViewHolder : ReactiveRecyclerViewViewHolder<LessonViewModel>
{
    /// <summary>Every subscription this holder owns, torn down together when the holder is disposed.</summary>
    private readonly DisposableBag _subscriptions = new();

    /// <summary>Initializes a new instance of the <see cref="LessonViewHolder"/> class.</summary>
    /// <param name="itemView">The inflated row layout for one lesson.</param>
    public LessonViewHolder(View itemView)
        : base(itemView)
    {
        // Implicit strategy: any writable View-typed property (SubjectLabel, RoomLabel) is wired to the
        // like-named resource in the inflated row layout.
        this.WireUpControls();

        _subscriptions.Add(this.WhenAnyValue(holder => holder.ViewModel).Subscribe(lesson =>
        {
            SubjectLabel!.Text = lesson?.Subject;
            RoomLabel!.Text = lesson is null ? null : $"Room {lesson.Room}";
        }));

        _subscriptions.Add(Selected.Subscribe(static position => TimetableLog.Info($"Lesson row {position} tapped.")));
        _subscriptions.Add(SelectedWithViewModel.Subscribe(static lesson => TimetableLog.Info($"Selected lesson: {lesson?.Subject}.")));
        _subscriptions.Add(LongClicked.Subscribe(static position => TimetableLog.Info($"Lesson row {position} long-clicked.")));
        _subscriptions.Add(LongClickedWithViewModel.Subscribe(static lesson => TimetableLog.Info($"Long-clicked lesson: {lesson?.Subject}.")));
        _subscriptions.Add(Activated.Subscribe(static _ => TimetableLog.Info("Lesson row attached to the window.")));
        _subscriptions.Add(Deactivated.Subscribe(static _ => TimetableLog.Info("Lesson row detached from the window.")));
    }

    /// <summary>Gets or sets the label showing the lesson's subject.</summary>
    public TextView? SubjectLabel { get; set; }

    /// <summary>Gets or sets the label showing the lesson's room.</summary>
    public TextView? RoomLabel { get; set; }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscriptions.Dispose();
        }

        base.Dispose(disposing);
    }
}
