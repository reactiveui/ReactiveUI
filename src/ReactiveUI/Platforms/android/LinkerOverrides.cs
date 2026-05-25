// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace ReactiveUI;

/// <summary>
/// Provides a container for methods used to preserve specific Android UI components during linking.
/// </summary>
/// <remarks>This class is intended for internal use to ensure that certain Android UI types and their members are
/// not removed by the linker. It is not intended to be used directly in application code.</remarks>
[Preserve(AllMembers = true)]
internal class LinkerOverrides
{
    /// <summary>
    /// Preserves the following Android UI types and their members.
    /// </summary>
    [ObsoletedOSPlatform("android30.0")]
    [SupportedOSPlatform("android23.0")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used by linker.")]
    [SuppressMessage("Major Bug", "S1656:Variables should not be self-assigned", Justification = "Used by linker.")]
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by linker.")]
    [SuppressMessage(
        "Style",
        "IDE0051:Private member is unused",
        Justification = "Linker preservation member.")]
    private void KeepMe()
    {
        TextView txt = new(null);
        txt.Text = txt.Text;

        ImageView iv = new(null);
        _ = iv.Drawable;

        ProgressBar prog = new(null);
        prog.Progress = prog.Progress;

        RadioButton cb = new(null);
        cb.Checked = cb.Checked;

        NumberPicker np = new(null);
        np.Value = np.Value;

        RatingBar rb = new(null);
        rb.Rating = rb.Rating;

        CalendarView cv = new(null!);
        cv.Date = cv.Date;
        TabHost th = new(null);
        th.CurrentTab = th.CurrentTab;
        TimePicker tp = new(null);
        tp.Hour = tp.Hour;
        tp.Minute = tp.Minute;
    }
}
