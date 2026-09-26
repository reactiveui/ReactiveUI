// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>Shows a student's name and grade. The <see cref="ViewModelViewHost"/> in <see cref="CourseListView"/> hosts it.</summary>
[DebuggerDisplay("StudentSummaryView")]
public partial class StudentSummaryView : ReactiveUserControl<StudentSummaryViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="StudentSummaryView"/> class.</summary>
    public StudentSummaryView()
    {
        InitializeComponent();

        // This overload takes a function that returns the bindings, rather than an action that registers them
        // one at a time; either style is equivalent, and a view with a handful of bindings often reads well as
        // a list built with collection-expression syntax.
        _ = this.WhenActivated(CreateBindings);
    }

    /// <summary>Builds the bindings for this view's two labels.</summary>
    /// <returns>The subscriptions to dispose when the view deactivates.</returns>
    private IEnumerable<IDisposable> CreateBindings() =>
    [
        this.OneWayBind(ViewModel, vm => vm.Student.Name, v => v.NameText.Text),
        this.OneWayBind(ViewModel, vm => vm.Student.Grade, v => v.GradeText.Text, static grade => $"Grade: {grade}")
    ];
}
