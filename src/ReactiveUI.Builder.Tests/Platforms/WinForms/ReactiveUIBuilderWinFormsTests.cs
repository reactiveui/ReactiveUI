// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using ReactiveUI.Winforms;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests.Platforms.WinForms;

public class ReactiveUIBuilderWinFormsTests
{
    [Fact]
    public void WithWinForms_Should_Register_WinForms_Services()
    {
        ResetAppBuilderState();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.WithWinForms().Build();

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.NotNull(platformOperations);

        var activationFetcher = locator.GetService<IActivationForViewFetcher>();
        Assert.NotNull(activationFetcher);
    }

    [Fact]
    public void WithCoreServices_AndWinForms_Should_Register_All_Services()
    {
        ResetAppBuilderState();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.WithCoreServices().WithWinForms().Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);

        var platformOperations = locator.GetService<IPlatformOperations>();
        Assert.NotNull(platformOperations);
    }

    private static void ResetAppBuilderState()
    {
        // Reset the static state of the AppBuilder.HasBeenBuilt property
        // This is necessary to ensure that tests can run independently
        var prop = typeof(AppBuilder).GetProperty("HasBeenBuilt", BindingFlags.Static | BindingFlags.Public);

        // Get the non-public setter method
        var setter = prop?.GetSetMethod(true); // 'true' includes non-public methods

        // Invoke the setter to set the value to false
        setter?.Invoke(null, new object[] { false });
    }
}
