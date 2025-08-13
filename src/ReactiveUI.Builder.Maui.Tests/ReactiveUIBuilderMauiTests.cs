// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Builder.Maui.Tests;

public class ReactiveUIBuilderMauiTests
{
    [Fact]
    public void WithMaui_Should_Register_Services()
    {
        using var locator = new ModernDependencyResolver();

        locator.CreateBuilder()
               .WithMaui()
               .Build();

        var typeConverters = locator.GetServices<IBindingTypeConverter>();
        Assert.NotNull(typeConverters);
    }

    [Fact]
    public void WithCoreServices_AndMaui_Should_Register_All_Services()
    {
        using var locator = new ModernDependencyResolver();

        locator.CreateBuilder()
               .WithCoreServices()
               .WithMaui()
               .Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        var typeConverters = locator.GetServices<IBindingTypeConverter>();
        Assert.NotNull(typeConverters);
    }
}
