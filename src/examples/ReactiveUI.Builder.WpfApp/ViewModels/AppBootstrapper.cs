// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.WpfApp.Services;
using Splat;

namespace ReactiveUI.Builder.WpfApp.ViewModels;

/// <summary>The root screen that owns the router and navigates to the terminal on start-up.</summary>
/// <seealso cref="ReactiveObject" />
/// <seealso cref="IScreen" />
public sealed class AppBootstrapper : ReactiveObject, IScreen
{
    /// <summary>Initializes a new instance of the <see cref="AppBootstrapper"/> class.</summary>
    public AppBootstrapper()
    {
        Router = new();

        var processor = Locator.Current.GetService<IPaymentProcessor>()!;
        _ = Router.Navigate.Execute(new TerminalViewModel(this, processor)).Subscribe();
    }

    /// <summary>Gets the router that hosts the navigation stack.</summary>
    public RoutingState Router { get; }
}
