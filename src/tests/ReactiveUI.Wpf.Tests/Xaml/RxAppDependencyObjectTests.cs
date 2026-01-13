// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Wpf;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Checks RxApp dependency objects.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because it calls RxAppBuilder.EnsureInitialized()
/// and accesses Locator.Current, which interact with global static state. This state must not be
/// concurrently accessed by parallel tests.
/// </remarks>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class RxAppDependencyObjectTests
{
    /// <summary>
    /// Tests that Dependency Property notifiers should be found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DepPropNotifierShouldBeFound()
    {
        RxAppBuilder.EnsureInitialized();

        await Assert.That(AppLocator.Current.GetServices<ICreatesObservableForProperty>()
                           .Any(static x => x is DependencyObjectObservableForProperty)).IsTrue();
    }
}
