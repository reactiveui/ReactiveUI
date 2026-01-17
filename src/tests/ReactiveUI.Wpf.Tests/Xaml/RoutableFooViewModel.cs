// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// A routable view model.
/// </summary>
public class RoutableFooViewModel : ReactiveUI.ReactiveObject, IRoutableFooViewModel
{
    /// <inheritdoc/>
    public IScreen HostScreen { get; set; } = new TestScreen();

    /// <inheritdoc/>
    public string? UrlPathSegment { get; set; }
}
