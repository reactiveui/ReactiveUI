// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.GitHub;

/// <summary>A GitHub repository as the search API returns it.</summary>
/// <param name="FullName">The owner and name, such as <c>reactiveui/ReactiveUI</c>.</param>
/// <param name="Description">The one-line description.</param>
/// <param name="Stars">The number of stars.</param>
[System.Diagnostics.DebuggerDisplay("{FullName} ({Stars})")]
public sealed record Repository(string FullName, string Description, int Stars);
