// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using Android.Content;
using Android.Views;
using AndroidX.ViewPager.Widget;

namespace ReactiveUI.Documentation.PlatformAndroid;

/// <summary>Pages through the timetable's weekdays. This activity only needs fragment support, not the full
/// AppCompat theming <see cref="AndroidX.ReactiveAppCompatActivity{TViewModel}"/> brings, so it derives from the
/// lighter <see cref="AndroidX.ReactiveFragmentActivity{TViewModel}"/> instead.</summary>
[Activity(Label = "Weekdays")]
[System.Diagnostics.DebuggerDisplay("{ViewModel}")]
public sealed class WeekdayPagerActivity : AndroidX.ReactiveFragmentActivity<WeekdayPagerViewModel>
{
    /// <summary>The intent extra key carrying the weekday the pager landed on when it finished.</summary>
    public static readonly string WeekdayExtra = "weekday";

    /// <summary>Every disposable this activity owns, torn down together when it is destroyed.</summary>
    private readonly DisposableBag _subscriptions = new();

    /// <inheritdoc/>
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_weekday_pager);

        ViewModel = new WeekdayPagerViewModel();
        _subscriptions.Add(Activated.Subscribe(static _ => TimetableLog.Info("WeekdayPagerActivity activated.")));
        _subscriptions.Add(Deactivated.Subscribe(static _ => TimetableLog.Info("WeekdayPagerActivity deactivated.")));
        _subscriptions.Add(ActivityResult.Subscribe(static result =>
            TimetableLog.Info($"WeekdayPagerActivity received an activity result: {result.Result}.")));

        ViewPager pager = FindViewById<ViewPager>(Resource.Id.weekdayPager)!;
        ObservableCollection<WeekdayViewModel> weekdays = BuildWeekdays();

        // The recommended constructor: pages an observable collection directly.
        AndroidX.ReactivePagerAdapter<WeekdayViewModel, ObservableCollection<WeekdayViewModel>> adapter = new(
            weekdays,
            CreatePage,
            static (weekday, _) => TimetableLog.Info($"Weekday page created for {weekday.Name}."));

        // The lower-level constructor pages any change-set stream, not just a collection - useful when the pages
        // come from a query or a filter rather than a plain list. Built here only to show it accepts the same
        // change-set ToReactiveChangeSet() produces, then disposed since the collection-based adapter above is
        // the one actually shown.
        using (AndroidX.ReactivePagerAdapter<WeekdayViewModel> fromChangeSet = new(weekdays.ToReactiveChangeSet(), CreatePage))
        {
            TimetableLog.Info($"Change-set-backed pager adapter also reports {fromChangeSet.Count} pages.");
        }

        pager.Adapter = adapter;
        pager.PageSelected += (_, e) =>
        {
            WeekdayViewModel weekday = weekdays[e.Position];
            ViewModel!.CurrentWeekday = weekday.Name;
            TimetableLog.Info($"Weekday pager moved to {weekday.Name}.");
        };

        TimetableLog.Info($"Weekday pager adapter reports {adapter.Count} pages.");

        await ReportFirstLessonAbsent(weekdays);

        pager.CurrentItem = 1;
        await Task.Delay(TimeSpan.FromMilliseconds(300));

        Intent resultIntent = new();
        resultIntent.PutExtra(WeekdayExtra, ViewModel!.CurrentWeekday);
        SetResult(Android.App.Result.Ok, resultIntent);
        Finish();
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

    /// <summary>Builds one summary per weekday that has at least one lesson.</summary>
    /// <returns>The weekday summaries to page through.</returns>
    private static ObservableCollection<WeekdayViewModel> BuildWeekdays()
    {
        TimetableViewModel timetable = new();
        return timetable.Weekdays;
    }

    /// <summary>Creates one pager page for a weekday, as an instance method so the lambda passed to the adapter needs no capture.</summary>
    /// <param name="weekday">The weekday the page shows; unused directly since <see cref="WeekdayViewHost"/> binds
    /// to it once the pager assigns it.</param>
    /// <param name="parent">The pager, used to resolve layout parameters for the inflated page.</param>
    /// <returns>The inflated page view.</returns>
    private View CreatePage(WeekdayViewModel weekday, ViewGroup parent)
    {
        _ = weekday;
        WeekdayViewHost host = new(this, parent);
        View view = host.ToView()!;

        WeekdayViewHost? typedHost = view.GetViewHost<WeekdayViewHost>();
        ILayoutViewHost? untypedHost = view.GetViewHost();
        TimetableLog.Info($"Weekday page tagged: typed={typedHost is not null}, untyped={untypedHost is not null}.");

        return view;
    }

    /// <summary>Reports the first lesson on the current weekday absent, using both the type and the intent overload
    /// of <see cref="AndroidX.ReactiveFragmentActivity{TViewModel}"/>'s <c>StartActivityForResultAsync</c>.</summary>
    /// <param name="weekdays">The weekday summaries currently paged.</param>
    /// <returns>A task that completes once both reports have finished.</returns>
    private async Task ReportFirstLessonAbsent(ObservableCollection<WeekdayViewModel> weekdays)
    {
        if (weekdays.Count == 0)
        {
            return;
        }

        // The Type overload takes no extras, so it suits a generic report that AbsenceActivity defaults itself.
        (Android.App.Result Result, Intent? Intent) firstResult = await StartActivityForResultAsync(typeof(AbsenceActivity), 300);
        TimetableLog.Info($"Generic absence report (by type) finished with {firstResult.Result}.");

        Intent byIntent = new(this, typeof(AbsenceActivity));
        byIntent.PutExtra(AbsenceActivity.SubjectExtra, weekdays[^1].Name);
        (Android.App.Result Result, Intent? Intent) secondResult = await StartActivityForResultAsync(byIntent, 301);
        TimetableLog.Info($"Absence report (by intent) finished with {secondResult.Result}.");
    }
}
