// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>The member card shown in the shell's <see cref="ViewModelControlHost"/> once a loan picks a member.</summary>
/// <param name="Member">The member the card describes.</param>
/// <param name="LoanCount">How many books the member currently has on loan, including this one.</param>
[DebuggerDisplay("MemberCardViewModel {Member.Name} LoanCount = {LoanCount}")]
public sealed record MemberCardViewModel(Member Member, int LoanCount);
