// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>A student enrolled in a course, with the grade they currently have out of 100.</summary>
/// <param name="name">The student's name.</param>
/// <param name="grade">The student's starting grade out of 100.</param>
[System.Diagnostics.DebuggerDisplay("{Name}: {Grade}")]
public sealed class Student(string name, int grade) : ReactiveObject
{
    /// <summary>Gets the student's name.</summary>
    public string Name { get; } = name;

    /// <summary>Gets or sets the student's grade out of 100.</summary>
    public int Grade
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = grade;
}
