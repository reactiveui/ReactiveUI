// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder.Maui.Tests;

[TestFixture]
public class ReactiveUIBuilderMauiTests
{
    [Test]
    public void WithMaui_Should_Register_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
               .WithMaui()
               .Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);

        var typeConverters = locator.GetServices<IBindingTypeConverter>();
        Assert.That(typeConverters, Is.Not.Null);
    }
}
