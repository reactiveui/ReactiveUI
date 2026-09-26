// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>Lists every student and previews the one currently selected next to the list.</summary>
[DebuggerDisplay("CourseListView")]
public partial class CourseListView : ReactiveUserControl<CourseListViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="CourseListView"/> class.</summary>
    public CourseListView()
    {
        InitializeComponent();

        // Unlike Host in MainWindow, SummaryHost shows nothing until a student is selected, so it needs its own
        // placeholder content. It gets its own transition too, distinct from Host's, since a preview panel
        // changing next to the list reads better as a small upward move than a full-width slide.
        SummaryHost.DefaultContent = "Select a student to preview their grade.";
        SummaryHost.Transition = TransitioningContentControl.TransitionType.Move;
        SummaryHost.Direction = TransitioningContentControl.TransitionDirection.Up;

        _ = this.WhenActivated(d =>
        {
            SummaryHost.ViewLocator = ViewLocator.GetCurrent();
            _ = this.OneWayBind(ViewModel, vm => vm.Students, v => v.StudentList.ItemsSource)
                .DisposeWith(d);
            _ = this.Bind(ViewModel, vm => vm.SelectedStudent, v => v.StudentList.SelectedItem)
                .DisposeWith(d);
            _ = this.OneWayBind(ViewModel, vm => vm.Summary, v => v.SummaryHost.ViewModel)
                .DisposeWith(d);
            _ = this.BindCommand(
                    ViewModel,
                    vm => vm.OpenStudent,
                    v => v.OpenButton,
                    this.WhenAnyValue(v => v.ViewModel!.SelectedStudent).WhereNotNull())
                .DisposeWith(d);
        });
    }
}
