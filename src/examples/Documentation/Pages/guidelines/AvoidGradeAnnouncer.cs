// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Subscribes to a student's grade in its constructor and never keeps the subscription, so nothing can ever stop it
/// announcing grades, even once this object should no longer care.
/// </summary>
public sealed class AvoidGradeAnnouncer : IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="AvoidGradeAnnouncer"/> class.</summary>
    /// <param name="student">The student to announce grades for.</param>
    public AvoidGradeAnnouncer(Student student) =>
        student.WhenAnyValue(x => x.Grade).Subscribe(grade => Console.WriteLine($"Avoid: {student.Name}'s grade is now {grade}"));

    /// <summary>Does nothing: the subscription above was never kept, so there is nothing left to dispose.</summary>
    public void Dispose()
    {
    }
}
