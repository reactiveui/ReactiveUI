// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Checks RxApp dependency objects.
/// </summary>
public class RxAppDependencyObjectTests : AppBuilderTestBase
{
    /// <summary>
    /// Tests that Dependency Property notifiers should be found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task DepPropNotifierShouldBeFound() =>
        await RunAppBuilderTestAsync(() =>
        {
            RxApp.EnsureInitialized();

            Assert.True(Locator.Current.GetServices<ICreatesObservableForProperty>()
                               .Any(x => x is DependencyObjectObservableForProperty));
        });
}
