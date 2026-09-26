// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Shows the tapped lesson's details, hosted as an <see cref="AndroidX.ReactiveFragment{TViewModel}"/>.</summary>
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class LessonDetailFragment : AndroidX.ReactiveFragment<LessonViewModel>
{
    /// <summary>Gets or sets the label for the lesson's subject; opted in explicitly rather than by naming
    /// convention, so the layout's <c>detailSubject</c> id is resolved even though the property name differs.</summary>
    [WireUpResource("detailSubject")]
    public TextView? SubjectLabel { get; set; }

    /// <summary>Gets or sets the label for the lesson's room, opted in the same way.</summary>
    [WireUpResource("detailRoom")]
    public TextView? RoomLabel { get; set; }

    /// <inheritdoc/>
    public override View? OnCreateView(LayoutInflater? inflater, ViewGroup? container, Bundle? savedInstanceState)
    {
        View view = inflater!.Inflate(Resource.Layout.fragment_lesson_detail, container, false)
            ?? throw new InvalidOperationException("Inflating fragment_lesson_detail produced no view.");

        // ExplicitOptIn: only properties carrying [WireUpResource] are wired, ignoring any other View-typed
        // property this fragment might gain later.
        AndroidX.ControlFetcherMixins.WireUpControls(this, view, ControlFetcherMixins.ResolveStrategy.ExplicitOptIn);

        // A control the fragment fetches directly, rather than wiring to a property.
        View? icon = view.GetControl(GetType().Assembly, "detailIcon");
        TimetableLog.Info(icon is null ? "detailIcon was not found." : "detailIcon resolved directly with GetControl.");

        this.WhenAnyValue(fragment => fragment.ViewModel).Subscribe(lesson =>
        {
            SubjectLabel!.Text = lesson?.Subject;
            RoomLabel!.Text = lesson is null ? null : $"Room {lesson.Room}";
        });

        return view;
    }
}
