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

        _ = this.WhenActivated(d =>
        {
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
