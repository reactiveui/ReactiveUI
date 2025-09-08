// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests.Platforms.Blazor;

[TestFixture]
public class ReactiveUIBuilderBlazorTests
{
    [Test]
    public void WithBlazor_Should_Register_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithBlazor().Build();

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.That(platformOperations, Is.Not.Null);

        var typeConverters = locator.GetServices<IBindingTypeConverter>();
        Assert.That(typeConverters, Is.Not.Empty);
    }

    [Test]
    public void WithCoreServices_AndBlazor_Should_Register_All_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithBlazor().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.That(platformOperations, Is.Not.Null);
    }
}
