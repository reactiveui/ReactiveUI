// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>The page that shows one student's grade and lets the user edit it.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class StudentDetailViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="StudentDetailViewModel"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="student">The student the page shows.</param>
    public StudentDetailViewModel(IScreen hostScreen, Student student)
    {
        HostScreen = hostScreen;
        Student = student;
        Grade = student.Grade;
        GoBack = ReactiveCommand.CreateFromObservable(() =>
        {
            Student.Grade = Grade;
            return HostScreen.Router.NavigateBack.Execute();
        });
    }

    /// <inheritdoc/>
    public string UrlPathSegment => $"students/{Student.Name}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the student the page shows.</summary>
    public Student Student { get; }

    /// <summary>Gets or sets the grade shown in the grade box. Saved to the student only when the page navigates back.</summary>
    public int Grade
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets the command that saves the grade and navigates back to the course list.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> GoBack { get; }
}
