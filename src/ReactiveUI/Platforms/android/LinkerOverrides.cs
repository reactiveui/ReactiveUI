// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ReactiveUI;

[Preserve(AllMembers = true)]
internal class LinkerOverrides
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Used by linker.")]
    private void KeepMe()
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

#pragma warning disable CA1422 // Validate platform compatibility
#pragma warning disable CS0618 // Type or member is obsolete
        var th = new TabHost(null);
        th.CurrentTab = th.CurrentTab;
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CA1422 // Validate platform compatibility

        var tp = new TimePicker(null);
        tp.Hour = tp.Hour;
        tp.Minute = tp.Minute;
    }
}
