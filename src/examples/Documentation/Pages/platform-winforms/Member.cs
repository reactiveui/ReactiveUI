// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>A library member who can borrow books.</summary>
/// <param name="Id">The member's card number.</param>
/// <param name="Name">The member's name.</param>
[DebuggerDisplay("Member {Name} ({Id})")]
public sealed record Member(string Id, string Name);
