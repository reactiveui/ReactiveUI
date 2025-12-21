// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests.Platforms.WinForms;

[TestFixture]
public class ReactiveUIBuilderWinFormsTests
{
    [Test]
    public void WithWinForms_Should_Register_WinForms_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithWinForms().Build();

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.That(platformOperations, Is.Not.Null);

        var activationFetcher = locator.GetService<IActivationForViewFetcher>();
        Assert.That(activationFetcher, Is.Not.Null);
    }

    [Test]
    public void WithCoreServices_AndWinForms_Should_Register_All_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithWinForms().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.That(platformOperations, Is.Not.Null);
    }
}
