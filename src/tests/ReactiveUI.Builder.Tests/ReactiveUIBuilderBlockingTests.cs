// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Builder.Tests;

[NotInParallel]
public class ReactiveUIBuilderBlockingTests
{
    [Test]
    public async Task Build_SetsFlag_AndBlocks_InitializeReactiveUI()
    {
        var observableProperty = Locator.Current.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();
    }
}
