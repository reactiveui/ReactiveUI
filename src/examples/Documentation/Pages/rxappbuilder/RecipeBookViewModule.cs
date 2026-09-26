// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>Maps the pantry feature's view, so the feature can be added to the app in one line.</summary>
[System.Diagnostics.DebuggerDisplay("RecipeBookViewModule")]
public sealed class RecipeBookViewModule : IViewModule
{
    /// <inheritdoc/>
    public void RegisterViews(DefaultViewLocator locator) =>
        locator.CreateMappingBuilder()
            .Map<PantryViewModel, PantryView>();
}
