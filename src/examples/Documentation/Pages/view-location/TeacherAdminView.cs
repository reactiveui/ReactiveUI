// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// The staff admin screen, mapped only while a teacher is signed in. Marked
/// <see cref="ExcludeFromViewRegistrationAttribute"/> so the source generator never finds it: only an explicit
/// mapping, added at sign-in and removed at sign-out, does.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("TeacherAdminView ViewModel = {ViewModel}")]
public sealed class TeacherAdminView : ReactiveObject, IViewFor<TeacherAdminViewModel>
{
    /// <summary>Gets or sets the teacher this admin screen was opened for.</summary>
    public TeacherAdminViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TeacherAdminViewModel?)value;
    }
}
