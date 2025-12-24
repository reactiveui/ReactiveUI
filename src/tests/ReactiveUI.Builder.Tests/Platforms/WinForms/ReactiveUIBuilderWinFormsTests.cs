// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Winforms;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests.Platforms.WinForms;

public class ReactiveUIBuilderWinFormsTests
{
    [Test]
    public async Task WithWinForms_Should_Register_WinForms_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithWinForms().Build();

        var platformOperations = locator.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();

        var activationFetcher = locator.GetService<IActivationForViewFetcher>();
        await Assert.That(activationFetcher).IsNotNull();
    }

    [Test]
    public async Task WithCoreServices_AndWinForms_Should_Register_All_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithWinForms().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();

        var platformOperations = locator.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();
    }
}
