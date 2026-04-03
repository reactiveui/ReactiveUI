// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;
using ReactiveUI.Builder.Tests.Executors;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests.Platforms.Blazor;

public class ReactiveUIBuilderBlazorTests
{
    [Test]
    [TestExecutor<WithBlazorExecutor>]
    public async Task WithBlazor_Should_Register_Services()
    {
        var platformOperations = Locator.Current.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();

        var typeConverters = Locator.Current.GetServices<IBindingTypeConverter>();
        await Assert.That(typeConverters).IsNotEmpty();
    }

    [Test]
    [TestExecutor<WithBlazorExecutor>]
    public async Task WithCoreServices_AndBlazor_Should_Register_All_Services()
    {
        var observableProperty = Locator.Current.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();

        var platformOperations = Locator.Current.GetService<IPlatformOperations>();
        await Assert.That(platformOperations).IsNotNull();
    }

    internal sealed class WithBlazorExecutor : BuilderTestExecutorBase
    {
        protected override void ConfigureBuilder() =>
            ((IReactiveUIBuilder)RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices())
                .WithBlazor()
                .BuildApp();
    }
}
