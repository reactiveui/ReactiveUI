// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Shows a revision note for a lesson, hosted as an <see cref="AndroidX.ReactiveFragment{TViewModel}"/>.
/// Wires its own label with the two-argument, implicit-strategy overload of <c>WireUpControls</c>, rather than
/// stating a strategy explicitly the way <see cref="LessonDetailFragment"/> does.</summary>
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class LessonNotesFragment : AndroidX.ReactiveFragment<LessonViewModel>
{
    /// <summary>Gets or sets the label showing the revision note; wired by naming convention to <c>notesLabel</c>.</summary>
    public TextView? NotesLabel { get; set; }

    /// <inheritdoc/>
    public override View? OnCreateView(LayoutInflater? inflater, ViewGroup? container, Bundle? savedInstanceState)
    {
        View view = inflater!.Inflate(Resource.Layout.fragment_lesson_notes, container, false)
            ?? throw new InvalidOperationException("Inflating fragment_lesson_notes produced no view.");

        // The convenience overload: no resolve strategy to state, since Implicit is what most fragments want.
        AndroidX.ControlFetcherMixins.WireUpControls(this, view);

        NotesLabel!.Text = ViewModel is null ? "No notes yet." : $"Revise {ViewModel.Subject} before the next lesson.";
        return view;
    }
}
