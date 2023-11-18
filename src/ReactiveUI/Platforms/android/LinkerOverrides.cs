// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NET7_0_OR_GREATER
using System.Runtime.Versioning;
#endif
using Android.Widget;

namespace ReactiveUI;

[Preserve(AllMembers = true)]
internal class LinkerOverrides
{
#if NET7_0_OR_GREATER
    [ObsoletedOSPlatform("android30.0")]
    [SupportedOSPlatform("android23.0")]
#pragma warning disable CA1822 // Mark members as static
#else
    [Obsolete("This method was deprecated in API level 30.", false)]
#endif
#pragma warning disable IDE0051 // Remove unused private members
    private void KeepMe()
#pragma warning restore IDE0051 // Remove unused private members
#if NET7_0_OR_GREATER
#pragma warning restore CA1822 // Mark members as static
#endif
    {
        var txt = new TextView(null);
        txt.Text = txt.Text;

        var iv = new ImageView(null);
        var obj = iv.Drawable;

        var prog = new ProgressBar(null);
        prog.Progress = prog.Progress;

        var cb = new RadioButton(null);
        cb.Checked = cb.Checked;

        var np = new NumberPicker(null);
        np.Value = np.Value;

        var rb = new RatingBar(null);
        rb.Rating = rb.Rating;

        var cv = new CalendarView(null!);
        cv.Date = cv.Date;
        var th = new TabHost(null);
        th.CurrentTab = th.CurrentTab;
        var tp = new TimePicker(null);
#pragma warning disable CA1416 // Validate platform compatibility
        tp.Hour = tp.Hour;
        tp.Minute = tp.Minute;
#pragma warning restore CA1416 // Validate platform compatibility
    }
}
