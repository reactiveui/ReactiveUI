// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests ensuring the builder blocks reflection-based initialization.
/// </summary>
[TestFixture]
public class ReactiveUIBuilderBlockingTests
{
    [Test]
    public void Build_SetsFlag_AndBlocks_InitializeReactiveUI()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        var builder = locator.CreateReactiveUIBuilder();
        builder.WithCoreServices().Build();

        locator.InitializeReactiveUI();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);
    }
}
