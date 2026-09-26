// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>A student's profile screen.</summary>
/// <param name="studentName">The student the profile belongs to.</param>
[System.Diagnostics.DebuggerDisplay("SchoolProfileViewModel StudentName = {StudentName}")]
public sealed class SchoolProfileViewModel(string studentName) : ReactiveObject
{
    /// <summary>Gets the student the profile belongs to.</summary>
    public string StudentName => studentName;
}
