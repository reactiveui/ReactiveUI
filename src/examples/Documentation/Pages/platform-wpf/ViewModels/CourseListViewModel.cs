// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>The page that lists every student across every course. Opening one navigates to its grade page.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class CourseListViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Backs <see cref="Summary"/>.</summary>
    private readonly ObservableAsPropertyHelper<StudentSummaryViewModel?> _summary;

    /// <summary>Initializes a new instance of the <see cref="CourseListViewModel"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="courses">The courses to list.</param>
    public CourseListViewModel(IScreen hostScreen, IReadOnlyList<Course> courses)
    {
        HostScreen = hostScreen;
        Courses = courses;
        Students = [.. courses.SelectMany(static course => course.Students)];

        _summary = this.WhenAnyValue(x => x.SelectedStudent)
            .Select(static student => student is null ? null : new StudentSummaryViewModel(student))
            .ToProperty(this, nameof(Summary));

        OpenStudent = ReactiveCommand.CreateFromObservable<Student, IRoutableViewModel>(
            student => HostScreen.Router.Navigate.Execute(new StudentDetailViewModel(HostScreen, student)));
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "courses";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the courses the page lists.</summary>
    public IReadOnlyList<Course> Courses { get; }

    /// <summary>Gets every student across every course, for the list box.</summary>
    public IReadOnlyList<Student> Students { get; }

    /// <summary>Gets or sets the student currently highlighted in the list.</summary>
    public Student? SelectedStudent
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the preview of <see cref="SelectedStudent"/> shown in the <see cref="ViewModelViewHost"/>.</summary>
    public StudentSummaryViewModel? Summary => _summary.Value;

    /// <summary>Gets the command that opens a student's grade page.</summary>
    public ReactiveCommand<Student, IRoutableViewModel> OpenStudent { get; }
}
