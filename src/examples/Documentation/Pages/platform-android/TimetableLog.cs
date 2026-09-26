// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Util;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Writes one line to both <c>Console.WriteLine</c> and <c>Log</c> under the <c>RxDocs</c> tag, so the
/// scenario this app drives on start can be read back from <c>adb logcat</c>.</summary>
public static class TimetableLog
{
    /// <summary>The logcat tag every line in this app's scenario is written under.</summary>
    private const string Tag = "RxDocs";

    /// <summary>Writes one line describing what the running scenario just did.</summary>
    /// <param name="message">The line to write.</param>
    public static void Info(string message)
    {
        Console.WriteLine(message);
        Log.Info(Tag, message);
    }
}
