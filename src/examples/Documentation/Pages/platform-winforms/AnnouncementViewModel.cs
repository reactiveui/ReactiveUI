// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>A one-line notice for the library's front desk, such as a holiday closure.</summary>
/// <param name="Message">The notice to show.</param>
[DebuggerDisplay("AnnouncementViewModel {Message}")]
public sealed record AnnouncementViewModel(string Message);
