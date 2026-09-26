// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// A plain WPF <c>Page</c> shown outside the router, from the main window's "About" button.
/// <see cref="ReactivePage{TViewModel}"/> gives a <c>Page</c> the same <c>ViewModel</c>/<c>BindingRoot</c> pair as
/// <see cref="ReactiveUserControl{TViewModel}"/> and <see cref="ReactiveWindow{TViewModel}"/>, for the WPF apps that
/// navigate with <c>Frame</c>/<c>NavigationWindow</c> pages instead of (or alongside) a <see cref="RoutingState"/>.
/// </summary>
[DebuggerDisplay("CourseInfoPage")]
public partial class CourseInfoPage : ReactivePage<CourseInfoPageViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="CourseInfoPage"/> class.</summary>
    public CourseInfoPage()
    {
        InitializeComponent();

        _ = this.WhenActivated(d =>
        {
            _ = this.OneWayBind(ViewModel, vm => vm.CourseCount, v => v.CourseCountText.Text, static count => $"Courses: {count}")
                .DisposeWith(d);
            _ = this.OneWayBind(ViewModel, vm => vm.StudentCount, v => v.StudentCountText.Text, static count => $"Students: {count}")
                .DisposeWith(d);

            // BindingRoot is the same view model as ViewModel, exposed under the name every ReactiveUI view uses so
            // XAML resource lookups and view-agnostic code can read it without knowing the concrete view model type.
            Console.WriteLine($"About page shows {BindingRoot?.CourseCount} courses.");
        });
    }
}
