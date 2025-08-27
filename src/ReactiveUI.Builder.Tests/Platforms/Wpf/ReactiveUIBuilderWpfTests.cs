// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Builder.Tests.Platforms.Wpf;

public class ReactiveUIBuilderWpfTests : AppBuilderTestBase
{
    [Fact]
    public async Task WithWpf_Should_Register_Wpf_Services() =>
        await RunAppBuilderTestAsync(() =>
        {
            using var locator = new ModernDependencyResolver();
            var builder = locator.CreateReactiveUIBuilder();

            builder.WithWpf().Build();

            var platformOperations = locator.GetService<IPlatformOperations>();
            Assert.NotNull(platformOperations);

            var activationFetcher = locator.GetService<IActivationForViewFetcher>();
            Assert.NotNull(activationFetcher);
        });

    [Fact]
    public async Task WithCoreServices_AndWpf_Should_Register_All_Services() =>
        await RunAppBuilderTestAsync(() =>
        {
            using var locator = new ModernDependencyResolver();
            var builder = locator.CreateReactiveUIBuilder();

            builder.WithWpf().Build();

            var observableProperty = locator.GetService<ICreatesObservableForProperty>();
            Assert.NotNull(observableProperty);

            var platformOperations = locator.GetService<IPlatformOperations>();
            Assert.NotNull(platformOperations);
        });
}
