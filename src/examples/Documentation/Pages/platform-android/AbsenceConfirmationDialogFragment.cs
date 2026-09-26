// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.OS;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Asks the user to confirm an absence, hosted as a modal <see cref="AndroidX.ReactiveDialogFragment{TViewModel}"/>.</summary>
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class AbsenceConfirmationDialogFragment : AndroidX.ReactiveDialogFragment<LessonViewModel>
{
    /// <summary>Every subscription this fragment owns, torn down together when it is disposed.</summary>
    private readonly DisposableBag _subscriptions = new();

    /// <summary>Initializes a new instance of the <see cref="AbsenceConfirmationDialogFragment"/> class.</summary>
    public AbsenceConfirmationDialogFragment()
    {
        _subscriptions.Add(Activated.Subscribe(static _ => TimetableLog.Info("AbsenceConfirmationDialogFragment activated.")));
        _subscriptions.Add(Deactivated.Subscribe(static _ => TimetableLog.Info("AbsenceConfirmationDialogFragment deactivated.")));
    }

    /// <inheritdoc/>
    public override Dialog OnCreateDialog(Bundle? savedInstanceState)
    {
        string subject = ViewModel?.Subject ?? "this lesson";

        return new global::AndroidX.AppCompat.App.AlertDialog.Builder(RequireContext()!)
            .SetTitle("Confirm absence")!
            .SetMessage($"Mark the student absent from {subject}?")!
            .SetPositiveButton("Confirm", (_, _) => TimetableLog.Info($"Absence confirmed for {subject}."))!
            .SetNegativeButton("Cancel", static (_, _) => TimetableLog.Info("Absence confirmation cancelled."))!
            .Create()!;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscriptions.Dispose();
        }

        base.Dispose(disposing);
    }
}
