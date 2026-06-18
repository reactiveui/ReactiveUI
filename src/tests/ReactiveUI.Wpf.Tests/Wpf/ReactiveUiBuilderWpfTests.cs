// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for the WPF ReactiveUI builder extensions.</summary>
public class ReactiveUiBuilderWpfTests
{
    /// <summary>Verifies that <c>WithWpf</c> registers the WPF platform services.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_Should_Register_Wpf_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithWpf().Build();

        var platformOperations = locator.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();

        var activationFetcher = locator.GetService<IActivationForViewFetcher>();
        await Assert.That(activationFetcher).IsNotNull();

        var propertyBinder = locator.GetService<IPropertyBinderImplementation>();
        await Assert.That(propertyBinder).IsNotNull();
    }

    /// <summary>Verifies that combining core services with <c>WithWpf</c> registers all expected services.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithCoreServices_AndWpf_Should_Register_All_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithWpf().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();

        var platformOperations = locator.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();
    }
}
