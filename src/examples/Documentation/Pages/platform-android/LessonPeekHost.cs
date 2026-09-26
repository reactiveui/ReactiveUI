// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>A one-line peek at a lesson, shown alongside the badge in <see cref="MainActivity"/>. Shows the
/// <see cref="ReactiveViewHost{TViewModel}"/> constructors that neither bind by reflection nor take an explicit
/// bind callback: the caller reads <see cref="LayoutViewHost.View"/> itself once inflation finishes. The
/// parameterless constructor is not shown here — it exists only for Android's Java-side activation path (designer
/// tools and view recreation), not for code a real app writes.</summary>
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class LessonPeekHost : ReactiveViewHost<LessonViewModel>
{
    /// <summary>The label showing the lesson's subject.</summary>
    private readonly TextView _subjectLabel;

    /// <summary>Initializes a new instance of the <see cref="LessonPeekHost"/> class using the 3-argument
    /// constructor, which defaults <c>attachToRoot</c> to <see langword="false"/>: the inflated view is returned
    /// standalone, and the caller decides where and when to add it (see <see cref="MainActivity"/>, which adds it
    /// to <c>PeeksRow</c> itself).</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The view group the label's layout parameters are resolved against.</param>
    public LessonPeekHost(Context context, ViewGroup parent)
        : base(context, Resource.Layout.lesson_peek, parent)
    {
        _subjectLabel = View!.FindViewById<TextView>(Resource.Id.peekSubjectLabel)!;
        Hook();
    }

    /// <summary>Initializes a new instance of the <see cref="LessonPeekHost"/> class using the 4-argument
    /// constructor, stating <c>attachToRoot</c> explicitly: passing <see langword="true"/> inflates the label
    /// directly into <paramref name="parent"/>, so the caller never needs its own <c>AddView</c> call.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The view group the label is inflated into.</param>
    /// <param name="attachToRoot">Whether to attach the inflated label to <paramref name="parent"/> immediately.</param>
    public LessonPeekHost(Context context, ViewGroup parent, bool attachToRoot)
        : base(context, Resource.Layout.lesson_peek, parent, attachToRoot)
    {
        _subjectLabel = View!.FindViewById<TextView>(Resource.Id.peekSubjectLabel)!;
        Hook();
    }

    /// <summary>Subscribes the label to view-model changes, shared by both constructors.</summary>
    private void Hook() =>
        this.WhenAnyValue(host => host.ViewModel)
            .Subscribe(lesson => _subjectLabel.Text = lesson is null ? null : $"Next: {lesson.Subject}");
}
