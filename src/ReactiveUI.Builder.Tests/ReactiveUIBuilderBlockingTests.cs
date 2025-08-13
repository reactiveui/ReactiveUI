// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests ensuring the builder blocks reflection-based initialization.
/// </summary>
public class ReactiveUIBuilderBlockingTests
{
    [Fact]
    public void Build_SetsFlag_AndBlocks_InitializeReactiveUI()
    {
        using var locator = new ModernDependencyResolver();

        RxApp.HasBeenBuiltUsingBuilder = false;

        var builder = locator.CreateBuilder();
        builder.WithCoreServices().Build();

        Assert.True(GetHasBeenBuiltUsingBuilder());

        locator.InitializeReactiveUI();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.NotNull(observableProperty);
    }

    private static bool GetHasBeenBuiltUsingBuilder()
    {
        return typeof(RxApp).GetField("HasBeenBuiltUsingBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static) is { } field && (bool)field.GetValue(null)!;
    }
}
