// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Guidelines;

/// <summary>
/// Subscribes to a student's grade inside <c>WhenActivated</c> and ties the subscription to the activation with
/// <c>DisposeWith</c>, so deactivating this object stops the announcements.
/// </summary>
[System.Diagnostics.DebuggerDisplay("PreferGradeAnnouncer")]
public sealed class PreferGradeAnnouncer : ReactiveObject, IActivatableViewModel, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="PreferGradeAnnouncer"/> class.</summary>
    /// <param name="student">The student to announce grades for.</param>
    public PreferGradeAnnouncer(Student student) =>
        this.WhenActivated(disposables =>
            student.WhenAnyValue(x => x.Grade)
                .Subscribe(grade => Console.WriteLine($"Prefer: {student.Name}'s grade is now {grade}"))
                .DisposeWith(disposables));

    /// <inheritdoc/>
    public ViewModelActivator Activator { get; } = new();

    /// <inheritdoc/>
    public void Dispose() => Activator.Dispose();
}
