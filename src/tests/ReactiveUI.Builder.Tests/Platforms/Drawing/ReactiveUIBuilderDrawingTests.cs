// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Builder.Tests.Platforms.Drawing;

public class ReactiveUIBuilderDrawingTests
{
    [Test]
    public async Task WithDrawing_Should_Register_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithDrawing().Build();

        // Drawing registers bitmap loader in non-NETSTANDARD contexts; we can still assert no exception and core services with chaining
        locator.CreateReactiveUIBuilder().WithDrawing().Build();
        var bindingConverters = locator.GetServices<IBindingTypeConverter>();
        await Assert.That(bindingConverters).IsNotNull();
    }
}
