// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWpf;

/// <summary>A course, with the students enrolled in it.</summary>
/// <param name="Name">The name of the course.</param>
/// <param name="Students">The students enrolled in the course.</param>
[System.Diagnostics.DebuggerDisplay("{Name} ({Students.Count} students)")]
public sealed record Course(string Name, IReadOnlyList<Student> Students);
