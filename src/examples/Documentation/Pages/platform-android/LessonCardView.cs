// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>A compound view highlighting the timetable's featured lesson, inflated into the top of
/// <see cref="MainActivity"/>'s layout. Wires its own children with the plain <see cref="View"/> overload of
/// <c>WireUpControls</c>, the way a hand-rolled compound view would.</summary>
[RequiresUnreferencedCode("Wires its child labels by reflecting over this type's properties.")]
[RequiresDynamicCode("Wires its child labels by reflecting over this type's properties.")]
[System.Diagnostics.DebuggerDisplay("{CardSubjectLabel}")]
public sealed class LessonCardView : LinearLayout
{
    /// <summary>Initializes a new instance of the <see cref="LessonCardView"/> class from an XML layout tag.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="attrs">The XML attributes supplied by the layout inflater.</param>
    public LessonCardView(Context context, IAttributeSet? attrs)
        : base(context, attrs)
    {
        Orientation = Orientation.Vertical;
        LayoutInflater.From(context)!.Inflate(Resource.Layout.lesson_card, this, true);

        // Implicit strategy: CardSubjectLabel and CardRoomLabel are wired to the like-named ids merged into this view.
        this.WireUpControls();
    }

    /// <summary>Initializes a new instance of the <see cref="LessonCardView"/> class from a JNI handle; required by
    /// the Android runtime's activation path, not called directly by app code.</summary>
    /// <param name="handle">The JNI handle supplied by the Android runtime.</param>
    /// <param name="ownership">The ownership of <paramref name="handle"/>.</param>
    public LessonCardView(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    /// <summary>Gets or sets the label showing the featured lesson's subject.</summary>
    public TextView? CardSubjectLabel { get; set; }

    /// <summary>Gets or sets the label showing the featured lesson's room.</summary>
    public TextView? CardRoomLabel { get; set; }

    /// <summary>Shows the given lesson's details on the card.</summary>
    /// <param name="lesson">The lesson to feature.</param>
    public void Show(LessonViewModel lesson)
    {
        CardSubjectLabel!.Text = lesson.Subject;
        CardRoomLabel!.Text = $"Room {lesson.Room}";
    }
}
