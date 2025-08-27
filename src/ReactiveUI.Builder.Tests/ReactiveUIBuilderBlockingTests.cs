// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests ensuring the builder blocks reflection-based initialization.
/// </summary>
public class ReactiveUIBuilderBlockingTests : AppBuilderTestBase
{
    [Fact]
    public async Task Build_SetsFlag_AndBlocks_InitializeReactiveUI() =>
        await RunAppBuilderTestAsync(() =>
        {
            using var locator = new ModernDependencyResolver();

            var builder = locator.CreateReactiveUIBuilder();
            builder.Build();

            locator.InitializeReactiveUI();

            var observableProperty = locator.GetService<ICreatesObservableForProperty>();
            Assert.NotNull(observableProperty);
        });
}
