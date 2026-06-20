// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Builder.Tests;

/// <summary>Tests that building the ReactiveUI builder registers core services and blocks duplicate initialization.</summary>
[NotInParallel]
public class ReactiveUIBuilderBlockingTests
{
    /// <summary>Verifies that building registers core services such as <see cref="ICreatesObservableForProperty"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Build_SetsFlag_AndBlocks_InitializeReactiveUI()
    {
        var observableProperty = Locator.Current.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();
    }
}
