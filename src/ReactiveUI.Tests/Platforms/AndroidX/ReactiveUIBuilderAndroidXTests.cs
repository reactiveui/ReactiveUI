// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.AndroidX;

namespace ReactiveUI.Tests.Platforms.AndroidX;

/// <summary>
/// Tests for AndroidX-specific ReactiveUIBuilder functionality.
/// These run on desktop test host and only verify DI registrations, not Android runtime behavior.
/// </summary>
[TestFixture]
public class ReactiveUIBuilderAndroidXTests
{
    /// <summary>
    /// Test that AndroidX services can be registered using the builder.
    /// </summary>
    [Test]
    public void WithAndroidX_Should_Register_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.WithAndroidX().Build();

        // Core/platform Android registrations ensure these services exist
        var commandBinder = locator.GetService<ICreatesCommandBinding>();
        Assert.That(commandBinder, Is.Not.Null);

        var observableForProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableForProperty, Is.Not.Null);
    }

    /// <summary>
    /// Test fluent chaining with AndroidX.
    /// </summary>
    [Test]
    public void WithCoreServices_AndAndroidX_Should_Register_All_Services()
    {
        using var locator = new ModernDependencyResolver();
        locator.CreateBuilder()
               .WithCoreServices()
               .WithAndroidX()
               .Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);

        var commandBinder = locator.GetService<ICreatesCommandBinding>();
        Assert.That(commandBinder, Is.Not.Null);
    }
}
