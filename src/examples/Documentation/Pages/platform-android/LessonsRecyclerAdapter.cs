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
    /// <inheritdoc/>
    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        ArgumentNullException.ThrowIfNull(parent);

        LayoutInflater inflater = LayoutInflater.From(parent.Context)
            ?? throw new InvalidOperationException("No LayoutInflater is available for this parent.");
        View itemView = inflater.Inflate(Resource.Layout.lesson_item, parent, false)
            ?? throw new InvalidOperationException("Inflating lesson_item produced no view.");

        return new LessonViewHolder(itemView);
    }
}
