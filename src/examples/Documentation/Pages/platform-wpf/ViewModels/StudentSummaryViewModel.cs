// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>
/// A short preview of a student. This is not routable: the course list shows it in a <see cref="ViewModelViewHost"/>
/// next to the list, rather than navigating to a whole page for it.
/// </summary>
/// <param name="student">The student to preview.</param>
[System.Diagnostics.DebuggerDisplay("{Student.Name}")]
public sealed class StudentSummaryViewModel(Student student) : ReactiveObject
{
    /// <summary>Gets the student being previewed.</summary>
    public Student Student { get; } = student;
}
