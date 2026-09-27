// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using ReactiveUI.AndroidX;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Binds the timetable's <see cref="TimetableViewModel.Lessons"/> collection to a <see cref="RecyclerView"/>.</summary>
/// <param name="lessons">The lessons collection to observe; every add, remove or replace updates the list on screen.</param>
[System.Diagnostics.DebuggerDisplay("ItemCount = {ItemCount}")]
public sealed class LessonsRecyclerAdapter(ObservableCollection<LessonViewModel> lessons) : ReactiveRecyclerViewAdapter<LessonViewModel, ObservableCollection<LessonViewModel>>(lessons)
{
    /// <summary>The view type for a lesson held in the science lab, room 7.</summary>
    private const int LabViewType = 1;

    /// <summary>The view type for every other lesson.</summary>
    private const int StandardViewType = 0;

    /// <summary>Picks a view type for a lesson: the science lab (room 7) gets its own type, so
    /// <see cref="OnCreateViewHolder"/> can tell a lab lesson apart from any other.</summary>
    /// <param name="position">The position of the lesson in the list.</param>
    /// <param name="viewModel">The lesson at that position, or <see langword="null"/> if the position is out of range.</param>
    /// <returns>An ID identifying the view type to use for the lesson.</returns>
    public override int GetItemViewType(int position, LessonViewModel? viewModel) =>
        viewModel?.Room == "7" ? LabViewType : StandardViewType;

    /// <inheritdoc/>
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        ArgumentNullException.ThrowIfNull(parent);

        LayoutInflater inflater = LayoutInflater.From(parent.Context)
            ?? throw new InvalidOperationException("No LayoutInflater is available for this parent.");
        View itemView = inflater.Inflate(Resource.Layout.lesson_item, parent, false)
            ?? throw new InvalidOperationException("Inflating lesson_item produced no view.");

        if (viewType == LabViewType)
        {
            TimetableLog.Info("Lesson row created for the science lab (room 7).");
        }

        return new LessonViewHolder(itemView);
    }
}
