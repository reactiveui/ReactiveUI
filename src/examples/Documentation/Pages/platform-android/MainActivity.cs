// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Android.Content;
using AndroidX.RecyclerView.Widget;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>The timetable's home screen: a list of lessons, and the entry point for every other screen this app
/// drives through on start.</summary>
[Activity(MainLauncher = true, Label = "@string/app_name")]
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class MainActivity : AndroidX.ReactiveAppCompatActivity<TimetableViewModel>
{
    /// <summary>Every disposable this activity owns, torn down together when it is destroyed.</summary>
    private readonly DisposableBag _subscriptions = new();

    /// <summary>Gets or sets the list of lessons; wired by naming convention to <c>lessonsRecyclerView</c>.</summary>
    public RecyclerView? LessonsRecyclerView { get; set; }

    /// <summary>Gets or sets the container the lesson-count badge is added to; wired to <c>badgeContainer</c>.</summary>
    public FrameLayout? BadgeContainer { get; set; }

    /// <summary>Gets or sets the container the detail and settings fragments are shown in; wired to <c>detailContainer</c>.</summary>
    public FrameLayout? DetailContainer { get; set; }

    /// <summary>Gets or sets the compound view featuring one lesson; wired to <c>featuredLessonCard</c>.</summary>
    public LessonCardView? FeaturedLessonCard { get; set; }

    /// <summary>Gets or sets the row two <see cref="LessonPeekHost"/> labels are added to; wired to <c>peeksRow</c>.</summary>
    public LinearLayout? PeeksRow { get; set; }

    /// <inheritdoc/>
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        // Implicit strategy: every writable View-typed property (LessonsRecyclerView, BadgeContainer,
        // DetailContainer) is wired to the like-named resource in activity_main.xml.
        this.WireUpControls();

        PropertyInfo[] wiredMembers = this.GetWireUpMembers(ControlFetcherMixins.ResolveStrategy.Implicit);
        TimetableLog.Info($"WireUpControls found {wiredMembers.Length} members to wire.");
        foreach (PropertyInfo member in wiredMembers)
        {
            TimetableLog.Info($"  {member.Name} -> resource '{member.GetResourceName()}'.");
        }

        ViewModel = new TimetableViewModel();

        // GetOrientation() returns the display's current rotation by name, such as "Rotation0" for the natural orientation.
        PlatformOperations platformOperations = new();
        string? orientation = platformOperations.GetOrientation();
        TimetableLog.Info($"Device orientation: {orientation}.");

        FeaturedLessonCard!.Show(ViewModel.Lessons[0]);

        LessonCountBadgeHost badgeHost = new(this, BadgeContainer!);
        badgeHost.SetLessonCount(ViewModel.Lessons.Count);
        BadgeContainer!.AddView(badgeHost.ToBadgeView());

        // The 3-arg constructor defaults attachToRoot to false, so this peek is added with an explicit AddView call.
        LessonPeekHost secondLessonPeek = new(this, PeeksRow!) { ViewModel = ViewModel.Lessons[1] };
        PeeksRow!.AddView(secondLessonPeek.View);

        // The 4-arg constructor with attachToRoot: true inflates directly into PeeksRow; no AddView call needed.
        _ = new LessonPeekHost(this, PeeksRow!, attachToRoot: true) { ViewModel = ViewModel.Lessons[2] };

        LessonsRecyclerView!.SetLayoutManager(new LinearLayoutManager(this));
        LessonsRecyclerView!.SetAdapter(new LessonsRecyclerAdapter(ViewModel.Lessons));

        _subscriptions.Add(Activated.Subscribe(static _ => TimetableLog.Info("MainActivity activated.")));
        _subscriptions.Add(Deactivated.Subscribe(static _ => TimetableLog.Info("MainActivity deactivated.")));
        _subscriptions.Add(ActivityResult.Subscribe(static result =>
            TimetableLog.Info($"MainActivity received an activity result: {result.Result}.")));

        await RunScenario();
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

    /// <summary>Drives the whole demo end to end: pick a lesson, report an absence, show its detail, confirm the
    /// absence, page the weekdays, then open notification settings.</summary>
    /// <returns>A task that completes once every screen in the scenario has run.</returns>
    private async Task RunScenario()
    {
        TimetableLog.Info("=== Scenario start ===");
        TimetableLog.Info($"Loaded {ViewModel!.Lessons.Count} lessons across {ViewModel.Weekdays.Count} weekdays.");

        LessonViewModel firstLesson = ViewModel.Lessons[0];
        TimetableLog.Info($"Selecting lesson: {firstLesson.Subject}.");

        Intent absenceIntent = new(this, typeof(AbsenceActivity));
        absenceIntent.PutExtra(AbsenceActivity.SubjectExtra, firstLesson.Subject);
        (Android.App.Result Result, Intent? Intent) absenceResult = await StartActivityForResultAsync(absenceIntent, 100);
        TimetableLog.Info($"Absence report (by intent) finished with {absenceResult.Result}: "
            + $"{absenceResult.Intent?.GetStringExtra(AbsenceActivity.SubjectExtra)}.");

        LessonViewModel secondLesson = ViewModel.Lessons[1];
        (Android.App.Result Result, Intent? Intent) secondAbsenceResult = await StartActivityForResultAsync(typeof(AbsenceActivity), 101);
        TimetableLog.Info($"Absence report (by type) finished with {secondAbsenceResult.Result}.");

        LessonDetailFragment detailFragment = new() { ViewModel = firstLesson };
        SupportFragmentManager!.BeginTransaction()!
            .Replace(DetailContainer!.Id, detailFragment)!
            .CommitNowAllowingStateLoss();
        TimetableLog.Info($"Showing detail fragment for {firstLesson.Subject}.");

        const string ConfirmationTag = "confirm-absence";
        new AbsenceConfirmationDialogFragment { ViewModel = secondLesson }.Show(SupportFragmentManager, ConfirmationTag);
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        (SupportFragmentManager!.FindFragmentByTag(ConfirmationTag) as AbsenceConfirmationDialogFragment)?.Dismiss();

        (Android.App.Result Result, Intent? Intent) weekdayResult = await StartActivityForResultAsync(typeof(WeekdayPagerActivity), 102);
        TimetableLog.Info($"Weekday pager finished with {weekdayResult.Result}, landed on "
            + $"{weekdayResult.Intent?.GetStringExtra(WeekdayPagerActivity.WeekdayExtra)}.");

        SettingsPreferenceFragment settingsFragment = new();
        SupportFragmentManager!.BeginTransaction()!
            .Replace(DetailContainer!.Id, settingsFragment)!
            .CommitNowAllowingStateLoss();
        TimetableLog.Info("Showing notification settings.");

        TimetableLog.Info("=== Scenario complete ===");
    }
}
