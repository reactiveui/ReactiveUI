// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows.Navigation;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// The app's only window. It hosts a <see cref="RoutedViewHost"/> that slides between the course list and a
/// student's grade page as the router navigates, and a notice board that shows the school office's notices through
/// <see cref="RoutedViewHostUnsafe"/> and <see cref="ViewModelViewHostUnsafe"/>.
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
        AboutButton.Click += (_, _) => ShowAboutPage(courses);

        Host.Transition = TransitioningContentControl.TransitionType.Slide;
        Host.Direction = TransitioningContentControl.TransitionDirection.Left;
        Host.Duration = TimeSpan.FromMilliseconds(250);
        Host.DefaultContent = "Pick a student to begin.";
        Host.TransitionStarted += static (_, _) => Console.WriteLine("Transition started.");
        Host.TransitionCompleted += static (_, _) => Console.WriteLine("Transition completed.");

        // OfficeNoticeView is registered only with the service locator, so the notice board uses the Unsafe twins.
        NoticeBoard noticeBoard = new();
        NoticeBoardHost.Router = noticeBoard.Router;
        LatestNoticeHost.ViewModel = new OfficeNoticeViewModel(noticeBoard, "Reports are due on Friday.");

        // The "d(...)" style registers one disposable at a time, rather than collecting them into a
        // MultipleDisposable first; RoutedViewHost's own constructor uses the same style internally.
        _ = this.WhenActivated(d =>
        {
            Host.Router = ViewModel!.Router;
            Host.ViewLocator = ViewLocator.GetCurrent();
            d(ViewModel.Router.Navigate.Execute(new CourseListViewModel(ViewModel, courses))
                .Subscribe());
            d(noticeBoard.Router.Navigate.Execute(new OfficeNoticeViewModel(noticeBoard, "Parent evening is on Tuesday."))
                .Subscribe());
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
