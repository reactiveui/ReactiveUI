// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Checks RxApp dependency objects.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because it calls RxApp.EnsureInitialized()
/// and accesses Locator.Current, which interact with global static state. This state must not be
/// concurrently accessed by parallel tests.
/// </remarks>
[TestFixture]
[Apartment(ApartmentState.STA)]
[NonParallelizable]
public class RxAppDependencyObjectTests
{
    private RxAppSchedulersScope? _schedulersScope;

    [SetUp]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
    }

    [TearDown]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }
    /// <summary>
    /// Tests that Dependency Property notifiers should be found.
    /// </summary>
    [Test]
    public void DepPropNotifierShouldBeFound()
    {
        RxApp.EnsureInitialized();

        Assert.That(
            Locator.Current.GetServices<ICreatesObservableForProperty>()
                           .Any(static x => x is DependencyObjectObservableForProperty),
            Is.True);
    }
}
