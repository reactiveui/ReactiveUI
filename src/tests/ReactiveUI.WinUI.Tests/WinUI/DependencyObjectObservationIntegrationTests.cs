// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Exercises generated Binding observation against WinUI dependency properties through ReactiveUI.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class DependencyObjectObservationIntegrationTests
{
    /// <summary>The number of values an observation delivers for its initial value and one change.</summary>
    private const int InitialAndChangeCount = 2;

    /// <summary>Verifies that a dependency property delivers its initial value and later changes.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValue_DependencyProperty_DeliversInitialAndChange()
    {
        var view = new DependencyObjectFixture();
        var values = view.WhenAnyValue(static x => x.TestString).Collect();

        await Assert.That(values).Count().IsEqualTo(1);
        await Assert.That(values[0]).IsNull();

        view.TestString = "Updated";

        await Assert.That(values).Count().IsEqualTo(InitialAndChangeCount);
        await Assert.That(values[1]).IsEqualTo("Updated");
    }

    /// <summary>Verifies that generated observation finds a dependency property inherited by a derived control.</summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WhenAnyValue_InheritedDependencyProperty_DeliversChange()
    {
        var view = new DerivedDependencyObjectFixture();
        var values = view.WhenAnyValue(static x => x.TestString).Collect();

        view.TestString = "Inherited";

        await Assert.That(values).Count().IsEqualTo(InitialAndChangeCount);
        await Assert.That(values[0]).IsNull();
        await Assert.That(values[1]).IsEqualTo("Inherited");
    }
}
