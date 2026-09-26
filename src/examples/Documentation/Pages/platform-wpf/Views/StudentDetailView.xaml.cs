// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// Shows one student's grade. <c>BindWithValidation</c> binds the grade box two-way, the same way a plain
/// <c>Bind</c> would, but resolves the target control by walking the view's visual tree rather than needing the
/// generated field a source-generated <c>Bind</c> would use.
/// </summary>
[DebuggerDisplay("StudentDetailView")]
public partial class StudentDetailView : ReactiveUserControl<StudentDetailViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="StudentDetailView"/> class.</summary>
    public StudentDetailView()
    {
        InitializeComponent();

        _ = this.WhenActivated(d =>
        {
            _ = this.OneWayBind(ViewModel, vm => vm.Student.Name, v => v.NameText.Text)
                .DisposeWith(d);
            _ = this.BindWithValidation(ViewModel!, vm => vm.Grade, v => v.GradeBox.Text)
                .DisposeWith(d);
            _ = this.BindCommand(ViewModel, vm => vm.GoBack, v => v.BackButton)
                .DisposeWith(d);
        });
    }
}
