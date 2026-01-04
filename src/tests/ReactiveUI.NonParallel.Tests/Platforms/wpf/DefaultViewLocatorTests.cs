// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Core;

namespace ReactiveUI.Tests;

/// <summary>
/// Contains unit tests for the <see cref="DefaultViewLocator"/> class, verifying view resolution behavior in WPF scenarios.
/// </summary>
[NotInParallel]
public partial class DefaultViewLocatorTests
{
    /// <summary>
    /// Tests that whether this instance [can resolve view from view model with IRoutableViewModel].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveViewFromViewModelWithIRoutableViewModelType()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        RxAppBuilder.CreateReactiveUIBuilder(resolver)
            .WithCoreServices()
            .BuildApp();

        // Register for both the interface and the concrete type
        resolver.Register(static () => new RoutableFooView(), typeof(IViewFor<IRoutableFooViewModel>));
        resolver.Register(static () => new RoutableFooView(), typeof(IViewFor<RoutableFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView(vm);

            await Assert.That(result).IsTypeOf<RoutableFooView>();
        }
    }

    /// <summary>
    /// Tests that make sure this instance [can resolve custom view with Map].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanResolveCustomViewWithMap()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        RxAppBuilder.CreateReactiveUIBuilder(resolver)
            .WithCoreServices()
            .BuildApp();

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();

            // Use Map to register custom view
            fixture.Map<RoutableFooViewModel, RoutableFooCustomView>(static () => new RoutableFooCustomView());

            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView(vm);
            await Assert.That(result).IsTypeOf<RoutableFooCustomView>();
        }
    }
}
