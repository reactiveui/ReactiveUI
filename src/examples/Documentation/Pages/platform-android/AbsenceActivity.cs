// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;
using Android.Views;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Records an absence for the lesson the student tapped. This activity predates AndroidX: it derives from
/// the platform's plain <see cref="ReactiveActivity{TViewModel}"/> rather than
/// <see cref="AndroidX.ReactiveAppCompatActivity{TViewModel}"/>, to keep that classic Activity/Fragment support
/// demonstrated somewhere real.</summary>
[Activity(Label = "Report absence")]
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class AbsenceActivity : ReactiveActivity<AbsenceViewModel>
{
    /// <summary>The intent extra key carrying the subject of the lesson to report.</summary>
    public static readonly string SubjectExtra = "subject";

    /// <summary>Every service-binding subscription this activity owns, disposed together when it is destroyed.</summary>
    private readonly DisposableBag _serviceSubscriptions = new();

    /// <summary>Gets or sets the label showing the absence status; wired by naming convention.</summary>
    public TextView? StatusLabel { get; set; }

    /// <summary>Gets or sets the compound view showing whether the absence has been reported yet; wired to <c>absenceStatusView</c>.</summary>
    public AbsenceStatusView? AbsenceStatusView { get; set; }

    /// <summary>Gets or sets a label this activity's layout carries no resource for, so it must stay excluded from
    /// wire-up, or <see cref="ControlFetcherMixins"/>'s explicit opt-out wire-up would throw looking for it.</summary>
    [IgnoreResource]
    public TextView? UnusedLabel { get; set; }

    /// <inheritdoc/>
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_absence);

        // ExplicitOptOut: every View-typed property is wired except those carrying [IgnoreResource].
        this.WireUpControls(ControlFetcherMixins.ResolveStrategy.ExplicitOptOut);

        // A control fetched directly, without a matching property, to show GetControl used on its own.
        View? extraLabel = this.GetControl("extraLabel");
        TimetableLog.Info(extraLabel is null ? "extraLabel was not found." : "extraLabel resolved directly with GetControl.");

        using (SuppressChangeNotifications())
        {
            string subject = Intent?.GetStringExtra(SubjectExtra) ?? "Unknown lesson";
            ViewModel = new AbsenceViewModel { Subject = subject };
        }

        _ = ThrownExceptions.Subscribe(static error => TimetableLog.Info($"AbsenceActivity reported an exception: {error.Message}"));

        StatusLabel!.Text = $"Reporting an absence from {ViewModel.Subject}.";
        AbsenceStatusView!.SetReported(false);

        // Bind.AutoCreate: this service is not already running, so the flag that starts it must be passed
        // explicitly. Without it the (Context, Intent) overloads' Bind.None never creates the service.
        Intent attendanceIntent = new(this, typeof(AttendanceTrackerService));
        _serviceSubscriptions.Add(this.ServiceBound<AttendanceTrackerService.AttendanceBinder>(attendanceIntent, Bind.AutoCreate)
            .Subscribe(
                binder =>
                {
                    if (binder is null)
                    {
                        return;
                    }

                    binder.Service.RecordAbsence(ViewModel!.Subject);
                    ViewModel.Reported = true;
                    StatusLabel!.Text = $"Reported: {ViewModel.Subject}.";
                    AbsenceStatusView!.SetReported(true);
                },
                static error => TimetableLog.Info($"Attendance service binding failed: {error.Message}")));

        // Give the AutoCreate bind above a moment to finish starting the service before binding to it again with
        // Bind.None below - Bind.None only connects to a service that is already running, it never creates one.
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        _serviceSubscriptions.Add(this.ServiceBound<AttendanceTrackerService.AttendanceBinder>(attendanceIntent)
            .Subscribe(static binder => TimetableLog.Info($"Generic (Context,Intent) ServiceBound connected: {binder is not null}.")));
        _serviceSubscriptions.Add(this.ServiceBound(attendanceIntent)
            .Subscribe(static binder => TimetableLog.Info($"Non-generic (Context,Intent) ServiceBound connected: {binder is not null}.")));

        // A second, unrelated service, read as a raw IBinder via the (Context, Intent, Bind) overload.
        Intent bellIntent = new(this, typeof(SchoolBellService));
        _serviceSubscriptions.Add(this.ServiceBound(bellIntent, Bind.AutoCreate)
            .Subscribe(static binder => TimetableLog.Info($"SchoolBellService bound via (Context,Intent,Bind): {binder is not null}.")));

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        Intent resultIntent = new();
        resultIntent.PutExtra(SubjectExtra, ViewModel!.Subject);
        SetResult(Android.App.Result.Ok, resultIntent);
        Finish();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _serviceSubscriptions.Dispose();
        }

        base.Dispose(disposing);
    }
}
