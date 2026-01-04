// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;

namespace ReactiveUI.Tests.Core;

/// <summary>
/// Tests for the <see cref="ViewLocator"/> static class.
/// </summary>
[NotInParallel]
public class ViewLocatorTest
{
    /// <summary>
    /// Tests that ViewLocator.Current throws when no locator is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Current_ThrowsViewLocatorNotFoundException_WhenNoLocatorRegistered()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        // Don't initialize ReactiveUI - this will leave no IViewLocator registered
        using (resolver.WithResolver())
        {
            var ex = await ThrowsAsync<ViewLocatorNotFoundException>(async () =>
            {
                _ = ViewLocator.Current;
                await Task.CompletedTask;
            });

            await That(ex).IsNotNull();
            await That(ex!.Message).Contains("Could not find a default ViewLocator");
        }
    }

    /// <summary>
    /// Tests that ViewLocator.Current returns the registered locator when available.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Current_ReturnsRegisteredLocator_WhenLocatorIsRegistered()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        RxAppBuilder.CreateReactiveUIBuilder(resolver)
            .WithCoreServices()
            .BuildApp();

        using (resolver.WithResolver())
        {
            var locator = ViewLocator.Current;

            await That(locator).IsNotNull();
            await That(locator).IsTypeOf<DefaultViewLocator>();
        }
    }

    /// <summary>
    /// Tests that ViewLocator.Current returns custom locator when registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Current_ReturnsCustomLocator_WhenCustomLocatorIsRegistered()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        var customLocator = new CustomViewLocator();
        resolver.Register(() => customLocator, typeof(IViewLocator));

        using (resolver.WithResolver())
        {
            var locator = ViewLocator.Current;

            await That(locator).IsNotNull();
            await That(locator).IsSameReferenceAs(customLocator);
        }
    }

    /// <summary>
    /// Custom view locator for testing.
    /// </summary>
    private class CustomViewLocator : IViewLocator
    {
        /// <inheritdoc/>
        public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
            where TViewModel : class
        {
            return null;
        }

        /// <inheritdoc/>
        [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
        [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
        public IViewFor<object>? ResolveView(object? instance, string? contract = null)
        {
            return null;
        }
    }
}
