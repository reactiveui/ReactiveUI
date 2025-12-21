// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.AOTTests;

/// <summary>
/// Test routable view model for AOT testing.
/// </summary>
internal class TestRoutableViewModel : ReactiveObject, IRoutableViewModel
{
    /// <inheritdoc/>
    public string? UrlPathSegment { get; } = "test";

    /// <inheritdoc/>
    public IScreen HostScreen { get; } = null!;

    public ViewModelActivator Activator { get; } = new();
}
