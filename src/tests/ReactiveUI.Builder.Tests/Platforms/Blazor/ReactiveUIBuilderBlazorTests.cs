// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests.Platforms.Blazor;

/// <summary>
/// Tests for registering Blazor platform services through the ReactiveUI builder.
/// </summary>
public class ReactiveUIBuilderBlazorTests
{
    /// <summary>
    /// Verifies that the Blazor builder registers platform operations and binding type converters.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithBlazorExecutor>]
    public async Task WithBlazor_Should_Register_Services()
    {
        var platformOperations = Locator.Current.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();

        var typeConverters = Locator.Current.GetServices<IBindingTypeConverter>();
        await Assert.That(typeConverters).IsNotEmpty();
    }

    /// <summary>
    /// Verifies that combining core and Blazor services registers both core and platform services.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithBlazorExecutor>]
    public async Task WithCoreServices_AndBlazor_Should_Register_All_Services()
    {
        var observableProperty = Locator.Current.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();

        var platformOperations = Locator.Current.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();
    }

    /// <summary>
    /// Executor that builds the app with Blazor platform services registered.
    /// </summary>
    internal sealed class WithBlazorExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            ((IReactiveUIBuilder)RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices())
            .WithBlazor()
            .BuildApp();
    }
}
