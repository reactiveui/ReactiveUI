// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>Wraps <see cref="MembersPlaceholderViewController"/> in a navigation bar, its own <c>ViewModel</c> set once.</summary>
[System.Diagnostics.DebuggerDisplay("MembersNavigationController")]
public sealed class MembersNavigationController : ReactiveNavigationController<MembersViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="MembersNavigationController"/> class.</summary>
    /// <param name="viewModel">The members data the root page shows.</param>
    public MembersNavigationController(MembersViewModel viewModel)
        : base(new MembersPlaceholderViewController { ViewModel = viewModel }) =>
        ViewModel = viewModel;
}
