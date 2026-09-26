// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using AndroidResult = Android.App.Result;

namespace ReactiveUI.Device.Tests;

/// <summary>An activity that returns a result to its caller as soon as it is created.</summary>
[Activity(Exported = false)]
public class ResultSourceActivity : Activity
{
    /// <summary>The intent extra key carrying the answer.</summary>
    internal const string AnswerKey = "answer";

    /// <summary>The answer the activity returns.</summary>
    internal const int Answer = 42;

    /// <inheritdoc/>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var data = new Intent();
        _ = data.PutExtra(AnswerKey, Answer);
        SetResult(AndroidResult.Ok, data);
        Finish();
    }
}
