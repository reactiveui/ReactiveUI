// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>A book the library desk can search, lend and take back.</summary>
/// <param name="Id">The book's catalogue number.</param>
/// <param name="Title">The book's title.</param>
/// <param name="Author">The book's author.</param>
[System.Diagnostics.DebuggerDisplay("{Title} ({Author})")]
public sealed record Book(int Id, string Title, string Author);
