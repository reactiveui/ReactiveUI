// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>A one-line peek at the first lesson. Its label is wired by name through
/// <see cref="ReactiveViewHostUnsafe{TViewModel}"/>'s reflection-based auto-wireup, so the constructor needs no
/// <c>FindViewById</c> call. <see cref="LessonPeekHost"/> shows the same peek on the reflection-free base.</summary>
[RequiresUnreferencedCode("Wires PeekSubjectLabel by reflecting over this type's properties.")]
[RequiresDynamicCode("Wires PeekSubjectLabel by reflecting over this type's properties.")]
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class FirstLessonPeekHost : ReactiveViewHostUnsafe<LessonViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="FirstLessonPeekHost"/> class, inflating the peek layout
    /// and wiring <see cref="PeekSubjectLabel"/> to the like-named resource by reflection.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The view group the label's layout parameters are resolved against.</param>
    public FirstLessonPeekHost(Context context, ViewGroup parent)
        : base(
            context,
            Resource.Layout.lesson_peek,
            parent,
            attachToRoot: false,
            performAutoWireup: true,
            resolveStrategy: ControlFetcherMixins.ResolveStrategy.Implicit) =>
        this.WhenAnyValue(host => host.ViewModel)
            .Subscribe(lesson => PeekSubjectLabel!.Text = lesson is null ? null : $"First: {lesson.Subject}");

    /// <summary>Gets or sets the label the lesson's subject is written to; wired automatically from the constructor.</summary>
    public TextView? PeekSubjectLabel { get; set; }
}
