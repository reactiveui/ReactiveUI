// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// A reading challenge the library runs, such as a summer goal of ten books. It comes from a plug-in whose view,
/// <see cref="ReadingChallengeView"/>, is registered with the service locator only.
/// </summary>
/// <param name="hostScreen">The shell whose router shows the challenge.</param>
/// <param name="title">The challenge's title.</param>
[DebuggerDisplay("{Title}")]
public sealed class ReadingChallengeViewModel(IScreen hostScreen, string title) : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string UrlPathSegment => "reading-challenge";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = hostScreen;

    /// <summary>Gets the challenge's title.</summary>
    public string Title { get; } = title;
}
