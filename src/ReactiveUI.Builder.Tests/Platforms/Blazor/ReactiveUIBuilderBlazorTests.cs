// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;

namespace ReactiveUI.Builder.Tests.Platforms.Blazor;

public class ReactiveUIBuilderBlazorTests
{
    [Fact]
    public void WithBlazor_Should_Register_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.WithBlazor().Build();

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.NotNull(platformOperations);

        var typeConverters = locator.GetServices<IBindingTypeConverter>();
        Assert.NotEmpty(typeConverters);
    }

    [Fact]
    public void WithCoreServices_AndBlazor_Should_Register_All_Services()
    {
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.WithCoreServices().WithBlazor().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.NotNull(platformOperations);
    }
}
