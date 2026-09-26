// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>One page of the weekday pager: a plain <see cref="ReactiveViewHost{TViewModel}"/> the pager adapter
/// creates per page, wired up explicitly (no reflection) so it stays AOT- and trimming-safe.</summary>
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class WeekdayViewHost : ReactiveViewHost<WeekdayViewModel>
{
    /// <summary>The label showing the weekday's name and lesson count.</summary>
    private TextView? _weekdayLabel;

    /// <summary>Initializes a new instance of the <see cref="WeekdayViewHost"/> class, inflating the page layout
    /// and wiring its label explicitly through the AOT-safe <c>bind</c> callback.</summary>
    /// <param name="context">The Android context.</param>
    /// <param name="parent">The pager's view group, used only to resolve layout parameters.</param>
    public WeekdayViewHost(Context context, ViewGroup parent)
        : base(
            context,
            Resource.Layout.weekday_page,
            parent,
            attachToRoot: false,
            bind: static (host, view) => ((WeekdayViewHost)host)._weekdayLabel = view.FindViewById<TextView>(Resource.Id.weekdayLabel))
    {
        this.WhenAnyValue(host => host.ViewModel).Subscribe(weekday =>
            _weekdayLabel!.Text = weekday is null ? null : $"{weekday.Name} ({weekday.LessonCount} lessons)");
    }
}
