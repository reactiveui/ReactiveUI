// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Commands;

/// <summary>One catalogue search in progress, reporting how many books it has matched so far.</summary>
/// <param name="MatchCount">A stream of the running match count, one value per book found.</param>
[System.Diagnostics.DebuggerDisplay("SearchSession")]
public sealed record SearchSession(IObservable<int> MatchCount);
