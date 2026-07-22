// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides a container for methods used to preserve specific Android UI components during linking.</summary>
/// <remarks>This class is intended for internal use to ensure that certain Android UI types and their members are
/// not removed by the linker. It is not intended to be used directly in application code.</remarks>
[Preserve(AllMembers = true)]
internal static class LinkerOverrides
{
    /// <summary>Preserves the following Android UI types and their members.</summary>
    [SupportedOSPlatform("android35.0")]
    [SuppressMessage(
        "Interoperability",
        "CA1422:Validate platform compatibility",
        Justification = "Linker preservation method references deprecated Android widgets solely to keep their members; it is never invoked.")]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used by linker.")]
    [SuppressMessage("Major Bug", "SST1189:Remove this self-assignment", Justification = "Deliberate self-assignment preserves both the getter and setter from the linker.")]
    private static void KeepMe()
    {
        using TextView txt = new(null);
        txt.Text = txt.Text;

        using ImageView iv = new(null);
        _ = iv.Drawable;

        using ProgressBar prog = new(null);
        prog.Progress = prog.Progress;

        using RadioButton cb = new(null);
        cb.Checked = cb.Checked;

        using NumberPicker np = new(null);
        np.Value = np.Value;

        using RatingBar rb = new(null);
        rb.Rating = rb.Rating;

        using CalendarView cv = new(null!);
        cv.Date = cv.Date;
        using TabHost th = new(null);
        th.CurrentTab = th.CurrentTab;
        using TimePicker tp = new(null);
        tp.Hour = tp.Hour;
        tp.Minute = tp.Minute;
    }
}
