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
        resolver.InitializeReactiveUI();

        // Register for both the interface and the concrete type
        resolver.Register(static () => new RoutableFooView(), typeof(IViewFor<IRoutableFooViewModel>));
        resolver.Register(static () => new RoutableFooView(), typeof(IViewFor<RoutableFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView<IRoutableViewModel>(vm);

            await Assert.That(result).IsTypeOf<RoutableFooView>();
        }
    }

    /// <summary>
    /// Tests that make sure this instance [can override name resolution function].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CanOverrideNameResolutionFunc()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(static () => new RoutableFooCustomView(), typeof(IViewFor<IRoutableFooViewModel>));
        resolver.Register(static () => new RoutableFooCustomView(), typeof(IViewFor<RoutableFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator
            {
                ViewModelToViewFunc = static x => x.Replace("ViewModel", "CustomView")
            };
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView<IRoutableViewModel>(vm);
            await Assert.That(result).IsTypeOf<RoutableFooCustomView>();
        }
    }
}
