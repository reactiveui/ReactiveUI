// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.GitHub;

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>Registers the views of the GitHub feature, so the feature can be added to an app in one line.</summary>
public sealed class GitHubViewModule : IViewModule
{
    /// <inheritdoc/>
    public void RegisterViews(DefaultViewLocator locator) =>
        locator.CreateMappingBuilder()
            .Map<RepositorySearchViewModel, RepositorySearchView>();
}
