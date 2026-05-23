// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;

namespace ReactiveUI.Builder.Tests.Platforms.Drawing;

/// <summary>
/// Tests for registering drawing platform services through the ReactiveUI builder.
/// </summary>
public class ReactiveUIBuilderDrawingTests
{
    /// <summary>
    /// Verifies that the drawing builder registers binding type converters.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithDrawingExecutor>]
    public async Task WithDrawing_Should_Register_Services()
    {
        var bindingConverters = Locator.Current.GetServices<IBindingTypeConverter>();
        await Assert.That(bindingConverters).IsNotNull();
    }

    /// <summary>
    /// Verifies that calling WithDrawing on a null builder throws <see cref="ArgumentNullException"/>.
    /// </summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task WithDrawing_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        IReactiveUIBuilder? builder = null;

        var exception = await Assert.That(() => builder!.WithDrawing()).Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// Executor that builds the app with drawing platform services registered.
    /// </summary>
    internal sealed class WithDrawingExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            ((IReactiveUIBuilder)RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices())
            .WithDrawing()
            .BuildApp();
    }
}
