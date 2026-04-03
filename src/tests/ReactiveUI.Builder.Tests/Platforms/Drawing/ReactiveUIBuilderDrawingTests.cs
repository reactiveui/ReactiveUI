// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests.Platforms.Drawing;

public class ReactiveUIBuilderDrawingTests
{
    [Test]
    [TestExecutor<WithDrawingExecutor>]
    public async Task WithDrawing_Should_Register_Services()
    {
        var bindingConverters = Locator.Current.GetServices<IBindingTypeConverter>();
        await Assert.That(bindingConverters).IsNotNull();
    }

    [Test]
    public async Task WithDrawing_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        IReactiveUIBuilder? builder = null;

        var exception = await Assert.That(() => builder!.WithDrawing()).Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.ParamName).IsEqualTo("builder");
    }

    internal sealed class WithDrawingExecutor : BuilderTestExecutorBase
    {
        protected override void ConfigureBuilder() =>
            ((IReactiveUIBuilder)RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices())
                .WithDrawing()
                .BuildApp();
    }
}
