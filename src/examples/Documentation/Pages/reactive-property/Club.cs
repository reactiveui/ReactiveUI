// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ReactiveProperty;

/// <summary>A school club a student can join.</summary>
/// <param name="Name">The club's name.</param>
/// <param name="MinimumAge">The youngest age the club accepts.</param>
[System.Diagnostics.DebuggerDisplay("{Name}, MinimumAge = {MinimumAge}")]
public sealed record Club(string Name, int MinimumAge);
