// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>A class schedule, shown either a day or a week at a time.</summary>
/// <param name="weekLabel">The week the schedule covers, such as "Week of Sep 29".</param>
[System.Diagnostics.DebuggerDisplay("TimetableViewModel WeekLabel = {WeekLabel}")]
public sealed class TimetableViewModel(string weekLabel) : ReactiveObject
{
    /// <summary>Gets the week the schedule covers.</summary>
    public string WeekLabel => weekLabel;
}
