// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Asks whether to add a note to an absence report. This activity predates AndroidX, like
/// <see cref="AbsenceActivity"/>: it derives from the plain <see cref="ReactiveActivity{TViewModel}"/>, so
/// <see cref="AbsenceActivity"/> can start it with that base's own <c>ActivityResult</c> and
/// <c>StartActivityForResultAsync</c> members rather than the AndroidX ones <see cref="MainActivity"/> already
/// uses.</summary>
[Activity(Label = "Add a note")]
public sealed class AbsenceNotePromptActivity : ReactiveActivity<AbsenceViewModel>
{
    /// <inheritdoc/>
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_absence_note_prompt);

        string subject = Intent?.GetStringExtra(AbsenceActivity.SubjectExtra) ?? "this lesson";
        TextView promptLabel = FindViewById<TextView>(Resource.Id.notePromptLabel)!;
        promptLabel.Text = $"Add a note about {subject}?";

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Intent resultIntent = new();
        resultIntent.PutExtra(AbsenceActivity.SubjectExtra, subject);
        SetResult(Android.App.Result.Ok, resultIntent);
        Finish();
    }
}
