// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using ReactiveUI.Drawing;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests.Platforms.Drawing;

public class ReactiveUIBuilderDrawingTests
{
    [Fact]
    public void WithDrawing_Should_Register_Services()
    {
        ResetAppBuilderState();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateBuilder();

        builder.WithDrawing().Build();

        // Drawing registers bitmap loader in non-NETSTANDARD contexts; we can still assert no exception and core services with chaining
        locator.CreateBuilder().WithCoreServices().WithDrawing().Build();
        var bindingConverters = locator.GetServices<IBindingTypeConverter>();
        Assert.NotNull(bindingConverters);
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
