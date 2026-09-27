// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Navigation;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// The app's only window. It hosts a <see cref="RoutedViewHost"/> that slides between the course list and a
/// student's grade page as the router navigates, a notice board that shows the school office's notices through
/// <see cref="RoutedViewHostUnsafe"/> and <see cref="ViewModelViewHostUnsafe"/>, and a page-name banner and a
/// handful of status labels that show every other <see cref="TransitioningContentControl"/> transition and every
/// <c>WhenActivated</c> overload the ones above don't already cover.
/// </summary>
[DebuggerDisplay("MainWindow")]
public partial class MainWindow : ReactiveWindow<AppShell>
{
    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();

        // The designer surface constructs every view in a project to lay it out; skip wiring a router and
        // navigating in that environment.
        if (this.GetIsDesignMode())
        {
            return;
        }

        ViewModel = new AppShell();
        IReadOnlyList<Course> courses = GradeBookStore.CreateSeeded();
        CourseListViewModel courseList = new(ViewModel, courses);
        AboutButton.Click += (_, _) => ShowAboutPage(courses);

        Host.Transition = TransitioningContentControl.TransitionType.Slide;
        Host.Direction = TransitioningContentControl.TransitionDirection.Left;
        Host.Duration = TimeSpan.FromMilliseconds(250);
        Host.DefaultContent = "Pick a student to begin.";
        Host.TransitionStarted += static (_, _) => Console.WriteLine("Transition started.");
        Host.TransitionCompleted += static (_, _) => Console.WriteLine("Transition completed.");

        // Force a fixed layout contract instead of the default one RoutedViewHost builds from the window's
        // orientation, the way an app that never changes layout with orientation would.
        Host.ViewContractObservable = Signal.Emit<string?>("desktop");
        Console.WriteLine($"Host view contract observable set: {Host.GetValue(RoutedViewHost.ViewContractObservableProperty) is not null}");

        // PageNameBanner is built entirely in code, the way a control assembled outside XAML would be: its
        // direction and duration are set through the raw dependency properties, with SetValue and SetBinding,
        // rather than the Direction and Duration wrapper properties Host uses above.
        PageNameBanner.Transition = TransitioningContentControl.TransitionType.Bounce;
        PageNameBanner.SetValue(TransitioningContentControl.TransitionDirectionProperty, TransitioningContentControl.TransitionDirection.Down);
        _ = PageNameBanner.SetBinding(
            TransitioningContentControl.TransitionDurationProperty,
            new System.Windows.Data.Binding(nameof(AppShell.PageBannerDuration)) { Source = ViewModel });

        // NoticeBoardHost and LatestNoticeHost each pick a transition style and direction Host and SummaryHost
        // don't already use, so the project shows every combination the type supports.
        NoticeBoardHost.Transition = TransitioningContentControl.TransitionType.Drop;
        NoticeBoardHost.Direction = TransitioningContentControl.TransitionDirection.Right;
        LatestNoticeHost.Transition = TransitioningContentControl.TransitionType.Fade;

        // OfficeNoticeView is registered only with the service locator, so the notice board uses the Unsafe twins.
        NoticeBoard noticeBoard = new();
        NoticeBoardHost.Router = noticeBoard.Router;
        LatestNoticeHost.ViewModel = new OfficeNoticeViewModel(noticeBoard, "Reports are due on Friday.");

        // NoticeStatusText, CommandStatusText and PageSummaryText are plain labels, not IViewFor instances of
        // their own, so each ties its activation to this window through an explicit-view WhenActivated overload.
        _ = NoticeStatusText.WhenActivated(
            d => d(noticeBoard.Router.CurrentViewModel.Subscribe(page =>
                NoticeStatusText.Text = $"Notice board page: {page?.UrlPathSegment ?? "(none)"}")),
            this);

        _ = CommandStatusText.WhenActivated(
            d => d.Add(courseList.OpenStudent.IsExecuting.Subscribe(executing =>
                CommandStatusText.Text = executing ? "Opening student..." : "Idle")),
            this);

        _ = PageSummaryText.WhenActivated(
            () =>
            [
                ViewModel!.Router.CurrentViewModel.CombineLatest(
                    noticeBoard.Router.CurrentViewModel,
                    static (course, notice) => $"{course?.UrlPathSegment ?? "start"} / {notice?.UrlPathSegment ?? "none"}")
                    .Subscribe(text => PageSummaryText.Text = text)
            ],
            this);

        // The "d(...)" style registers one disposable at a time, rather than collecting them into a
        // MultipleDisposable first; RoutedViewHost's own constructor uses the same style internally.
        _ = this.WhenActivated(d =>
        {
            Host.Router = ViewModel!.Router;
            Host.ViewLocator = ViewLocator.GetCurrent();
            d(ViewModel.Router.Navigate.Execute(courseList).Subscribe());
            d(noticeBoard.Router.Navigate.Execute(new OfficeNoticeViewModel(noticeBoard, "Parent evening is on Tuesday."))
                .Subscribe());
            d(ViewModel.Router.CurrentViewModel.Subscribe(page => PageNameBanner.Content = page?.UrlPathSegment ?? "(none)"));
            Console.WriteLine($"View contract: {Host.ViewContract ?? "(none)"}");

            // BindingRoot is the same view model as ViewModel, exposed under the name every ReactiveUI view uses.
            Console.WriteLine($"BindingRoot's router has {BindingRoot?.Router.NavigationStack.Count} page(s) on screen.");
        });
    }

    /// <summary>Opens the "About" page in its own <see cref="NavigationWindow"/>, outside the router.</summary>
    /// <param name="courses">The courses to summarize.</param>
    private static void ShowAboutPage(IReadOnlyList<Course> courses)
    {
        int studentCount = courses.Sum(static course => course.Students.Count);
        CourseInfoPage page = new() { ViewModel = new CourseInfoPageViewModel(courses.Count, studentCount) };
        NavigationWindow aboutWindow = new()
        {
            Width = 320,
            Height = 200,
            Content = page,
        };
        aboutWindow.Show();
    }
}
