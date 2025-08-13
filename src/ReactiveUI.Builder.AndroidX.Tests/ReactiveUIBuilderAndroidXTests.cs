// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.AndroidX;

namespace ReactiveUI.Builder.AndroidX.Tests;

public class ReactiveUIBuilderAndroidXTests
{
    [Fact]
    public void WithAndroidX_Should_Register_Services()
    {
        using var locator = new ModernDependencyResolver();

        locator.CreateBuilder()
               .WithAndroidX()
               .Build();

        var commandBinder = locator.GetService<ICreatesCommandBinding>();
        Assert.NotNull(commandBinder);

        var observableForProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableForProperty);
    }

    [Fact]
    public void WithCoreServices_AndAndroidX_Should_Register_All_Services()
    {
        using var locator = new ModernDependencyResolver();

        locator.CreateBuilder()
               .WithCoreServices()
               .WithAndroidX()
               .Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        var commandBinder = locator.GetService<ICreatesCommandBinding>();
        Assert.NotNull(commandBinder);
    }
}
