// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>A student's enrollment in one course: whether the course still accepts students, and whether its roster is loading.</summary>
[System.Diagnostics.DebuggerDisplay("Course = {Course}, IsEnabled = {IsEnabled}")]
public sealed class Enrollment : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="Enrollment"/> class.</summary>
    /// <param name="course">The course this enrollment is for.</param>
    public Enrollment(string course) => Course = course;

    /// <summary>Gets the course this enrollment is for.</summary>
    public string Course { get; }

    /// <summary>Gets or sets a value indicating whether the course still accepts students.</summary>
    public bool IsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    /// <summary>Gets or sets a value indicating whether the course's roster is loading.</summary>
    public bool IsLoading
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
