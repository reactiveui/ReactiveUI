// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// The generic twin of <see cref="AnalyticsViewModelViewHost"/>, typed to one view model so no cast is needed to
/// read <c>ViewModel</c> back. It overrides the generic <c>ResolveViewForViewModel</c> the same way: run the base
/// resolution, then note the view it chose.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model the host shows.</typeparam>
[System.Diagnostics.DebuggerDisplay("AnalyticsViewModelViewHost")]
public sealed class AnalyticsViewModelViewHost<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> : ViewModelViewHost<TViewModel>
    where TViewModel : class
{
    /// <summary>Gets the resolved view names recorded so far, in order.</summary>
    public List<string> ResolvedViews { get; } = [];

    /// <inheritdoc/>
    protected override void ResolveViewForViewModel(TViewModel? viewModel, string? contract)
    {
        base.ResolveViewForViewModel(viewModel, contract);
        ResolvedViews.Add(Content?.GetType().Name ?? "(nothing)");
    }
}
