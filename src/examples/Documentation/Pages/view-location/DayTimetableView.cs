// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// Shows one day of the timetable. Marked <see cref="ExcludeFromViewRegistrationAttribute"/> so an explicit mapping,
/// not the source generator, decides when this view is shown.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("DayTimetableView ViewModel = {ViewModel}")]
public sealed class DayTimetableView : ReactiveObject, IViewFor<TimetableViewModel>
{
    /// <summary>Gets or sets the timetable this view shows a day of.</summary>
    public TimetableViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TimetableViewModel?)value;
    }
}
