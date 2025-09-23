// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Contains unit tests for the <see cref="DefaultViewLocator"/> class, verifying view resolution behavior in WPF scenarios.
/// </summary>
public partial class DefaultViewLocatorTests
{
    /// <summary>
    /// Tests that whether this instance [can resolve view from view model with IRoutableViewModel].
    /// </summary>
    [Test]
    public void CanResolveViewFromViewModelWithIRoutableViewModelType()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(static () => new RoutableFooView(), typeof(IViewFor<IRoutableFooViewModel>));

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator();
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView<IRoutableViewModel>(vm);
            Assert.That(result, Is.TypeOf<RoutableFooView>());
        }
    }

    /// <summary>
    /// Tests that make sure this instance [can override name resolution function].
    /// </summary>
    [Test]
    public void CanOverrideNameResolutionFunc()
    {
        var resolver = new ModernDependencyResolver();

        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
        resolver.Register(static () => new RoutableFooCustomView());

        using (resolver.WithResolver())
        {
            var fixture = new DefaultViewLocator
            {
                ViewModelToViewFunc = static x => x.Replace("ViewModel", "CustomView")
            };
            var vm = new RoutableFooViewModel();

            var result = fixture.ResolveView<IRoutableViewModel>(vm);
            Assert.That(result, Is.TypeOf<RoutableFooCustomView>());
        }
    }
}
