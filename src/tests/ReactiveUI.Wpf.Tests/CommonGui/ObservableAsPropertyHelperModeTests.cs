// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.TestGuiMocks.common_gui;
using ReactiveUI.TestGuiMocks.common_gui.Mocks;
using ReactiveUI.TestGuiMocks.CommonGuiMocks;
using ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;

namespace ReactiveUI.Tests;

/// <summary>
/// OAPH mode tests.
/// </summary>
public class ObservableAsPropertyHelperModeTests
{
    /// <summary>
    /// Tests that ToProperty should only subscribe only once.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToPropertyShouldSubscribeOnlyOnce()
    {
        using (ProductionMode.Set())
        {
            var f = new RaceConditionFixture();

            // This line is important because it triggers connect to
            // be called recursively thus cause the subscription
            // to be called twice. Not sure if this is a reactive UI
            // or RX bug.
            f.PropertyChanged += (e, s) => _ = f.A;

            using (Assert.Multiple())
            {
                // Trigger subscription to the underlying observable.
                await Assert.That(f.A).IsTrue();

                await Assert.That(f.Count).IsEqualTo(1);
            }
        }
    }

    /// <summary>
    /// Tests to make sure that ToProperty overload with the nameof only subscribes once.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_NameOf_ShouldSubscribeOnlyOnce()
    {
        using (ProductionMode.Set())
        {
            var f = new RaceConditionNameOfFixture();

            // This line is important because it triggers connect to
            // be called recursively thus cause the subscription
            // to be called twice. Not sure if this is a reactive UI
            // or RX bug.
            f.PropertyChanged += (e, s) => _ = f.A;

            using (Assert.Multiple())
            {
                // Trigger subscription to the underlying observable.
                await Assert.That(f.A).IsTrue();

                await Assert.That(f.Count).IsEqualTo(1);
            }
        }
    }
}
