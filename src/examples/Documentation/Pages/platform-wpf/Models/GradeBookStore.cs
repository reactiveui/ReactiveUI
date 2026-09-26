// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>An in-memory grade book: a handful of courses, each with a few students.</summary>
public static class GradeBookStore
{
    /// <summary>Creates the courses the app starts with.</summary>
    /// <returns>The seeded courses.</returns>
    public static IReadOnlyList<Course> CreateSeeded() =>
    [
        new Course(
            "Calculus I",
            [
                new Student("Ada Lovelace", 92),
                new Student("Alan Turing", 88)
            ]),
        new Course(
            "Data Structures",
            [
                new Student("Grace Hopper", 95),
                new Student("Margaret Hamilton", 90)
            ])
    ];
}
