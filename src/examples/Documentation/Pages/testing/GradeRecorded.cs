// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Testing;

/// <summary>A message broadcast on the message bus each time a grade is recorded for a student.</summary>
/// <param name="StudentName">The name of the student the grade belongs to.</param>
/// <param name="Grade">The grade that was recorded.</param>
[System.Diagnostics.DebuggerDisplay("StudentName = {StudentName}, Grade = {Grade}")]
public sealed record GradeRecorded(string StudentName, int Grade);
