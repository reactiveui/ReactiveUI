// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Xaml;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Wpf;

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
        // Get the resolver set up by the executor scope
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);

        // Register for both the interface and the concrete type
        resolver.Register(static () => new RoutableFooView(), typeof(IViewFor<IRoutableFooViewModel>));
        resolver.Register(static () => new RoutableFooView(), typeof(IViewFor<RoutableFooViewModel>));

        var fixture = new DefaultViewLocator();
        var vm = new RoutableFooViewModel();

        var result = fixture.ResolveView(vm);

        await Assert.That(result).IsTypeOf<RoutableFooView>();
    }

    /// <summary>
    /// Tests that make sure this instance [can resolve custom view with Map].
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<Executor<RoutableFooCustomView, RoutableFooViewModel>>]
    public async Task CanResolveCustomViewWithMap()
    {
        // Get the resolver set up by the executor scope
        var resolver = AppLocator.Current as IDependencyResolver;
        ArgumentNullException.ThrowIfNull(resolver);

        var fixture = new DefaultViewLocator();

        // Use Map to register custom view
        fixture.Map<RoutableFooViewModel, RoutableFooCustomView>(static () => new RoutableFooCustomView());

        var vm = new RoutableFooViewModel();

        var result = fixture.ResolveView(vm);
        await Assert.That(result).IsTypeOf<RoutableFooCustomView>();
    }

    public class Executor<TView, TViewModel> : STAThreadExecutor
        where TView : class, IViewFor<TViewModel>, new()
        where TViewModel : class, IReactiveObject
    {
        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            RxAppBuilder.ResetForTesting();

            // Replace with a new locator that tests can modify
            // Include WPF platform services to ensure view locator, activation, etc. work
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithWpf()
                .RegisterView<TView, TViewModel>()
                .WithCoreServices()
                .BuildApp();
        }

        /// <inheritdoc />
        protected override void CleanUp()
        {
            RxAppBuilder.ResetForTesting();
            base.CleanUp();
        }
    }
}
