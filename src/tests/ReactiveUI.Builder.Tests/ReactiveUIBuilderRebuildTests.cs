// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests;

/// <summary>Tests that a second build in the same process leaves the first build's global state alone.</summary>
[NotInParallel]
[TestExecutor<ResetOnlyExecutor>]
public class ReactiveUIBuilderRebuildTests
{
    /// <summary>Verifies that a second builder does not replace the global converter service or message bus.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Second_build_leaves_global_state_from_first_build()
    {
        var originalBus = MessageBus.Current;
        try
        {
            var firstBuilder = RxAppBuilder.CreateReactiveUIBuilder();
            _ = firstBuilder.WithCoreServices();
            _ = firstBuilder.BuildApp();
            var firstService = BindingConverters.Current;
            var firstBus = MessageBus.Current;

            var resolver = new ModernDependencyResolver();
            var secondBuilder = new ReactiveUIBuilder(resolver, resolver);
            _ = secondBuilder.WithCoreServices();
            _ = secondBuilder
                .WithMessageBus(new MessageBus())
                .BuildApp();

            using (Assert.Multiple())
            {
                await Assert.That(BindingConverters.Current).IsSameReferenceAs(firstService);
                await Assert.That(MessageBus.Current).IsSameReferenceAs(firstBus);
            }
        }
        finally
        {
            MessageBus.Current = originalBus;
        }
    }

    /// <summary>Executor that only resets state, leaving builder configuration to each test.</summary>
    internal sealed class ResetOnlyExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            // Tests in this class configure and build the builders themselves.
        }
    }
}
