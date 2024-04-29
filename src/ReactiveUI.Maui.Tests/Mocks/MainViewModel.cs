// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests.Mocks;

/// <summary>
/// The main view model.
/// </summary>
public class MainViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel() => HostScreen = Locator.Current.GetService<IScreen>() ?? throw new InvalidOperationException("There is no valid screen");

    /// <inheritdoc/>
    public string? UrlPathSegment => "Main view";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }
}
