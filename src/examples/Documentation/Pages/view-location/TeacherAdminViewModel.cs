// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>The staff-only screen a teacher sees after signing in.</summary>
/// <param name="teacherName">The signed-in teacher's name.</param>
[System.Diagnostics.DebuggerDisplay("TeacherAdminViewModel TeacherName = {TeacherName}")]
public sealed class TeacherAdminViewModel(string teacherName) : ReactiveObject
{
    /// <summary>Gets the signed-in teacher's name.</summary>
    public string TeacherName => teacherName;
}
