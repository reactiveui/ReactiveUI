// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>A small, non-reactive badge showing how many lessons are on the timetable. Plain
/// <see cref="LayoutViewHost"/> is enough here: the badge has no view model, just a count to render once. Its
/// label is wired by name through the legacy reflection-based constructor, rather than an explicit bind callback,
/// to show that path too.</summary>
[RequiresUnreferencedCode("Wires BadgeText by reflecting over this type's properties.")]
[RequiresDynamicCode("Wires BadgeText by reflecting over this type's properties.")]
[System.Diagnostics.DebuggerDisplay("{BadgeText}")]
public sealed class LessonCountBadgeHost : LayoutViewHost
{
    /// <summary>Initializes a new instance of the <see cref="LessonCountBadgeHost"/> class, inflating the badge
    /// layout and wiring <see cref="BadgeText"/> to the like-named resource by reflection.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The parent view group the badge is placed into.</param>
    public LessonCountBadgeHost(Context context, ViewGroup parent)
        : base(
            context,
            Resource.Layout.lesson_count_badge,
            parent,
            attachToRoot: false,
            performAutoWireup: true,
            resolveStrategy: ControlFetcherMixins.ResolveStrategy.Implicit)
    {
    }

    /// <summary>Gets or sets the label the badge text is written to; wired automatically from the constructor.</summary>
    public TextView? BadgeText { get; set; }

    /// <summary>Sets the badge's text to the current lesson count.</summary>
    /// <param name="lessonCount">The number of lessons to show.</param>
    public void SetLessonCount(int lessonCount) =>
        BadgeText!.Text = $"{lessonCount} lessons this week";

    /// <summary>Returns the inflated badge <see cref="View"/>, ready to add to a layout.</summary>
    /// <returns>The badge's root view.</returns>
    public View ToBadgeView() => ToView()!;
}
