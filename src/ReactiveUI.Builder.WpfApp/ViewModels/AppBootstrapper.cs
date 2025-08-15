// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>
/// AppBootstrapper.
/// </summary>
/// <seealso cref="ReactiveUI.ReactiveObject" />
/// <seealso cref="ReactiveUI.IScreen" />
public class AppBootstrapper : ReactiveObject, IScreen
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
    /// </summary>
    public AppBootstrapper()
    {
        Router = new RoutingState();

        // Navigate to Lobby on start
        Router.Navigate.Execute(new LobbyViewModel(this)).Subscribe();
    }

    /// <summary>
    /// Gets the Router associated with this Screen.
    /// </summary>
    public RoutingState Router { get; }
}
