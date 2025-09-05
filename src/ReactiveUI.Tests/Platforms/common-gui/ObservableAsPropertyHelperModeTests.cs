// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ReactiveUI.Tests;

/// <summary>
/// OAPH mode tests.
/// </summary>
[TestFixture]
public class ObservableAsPropertyHelperModeTests
{
    /// <summary>
    /// Tests that ToProperty should only subscribe only once.
    /// </summary>
    [Test]
    public void ToPropertyShouldSubscribeOnlyOnce()
    {
        using (ProductionMode.Set())
        {
            var f = new RaceConditionFixture();

            // This line is important because it triggers connect to
            // be called recursively thus cause the subscription
            // to be called twice. Not sure if this is a reactive UI
            // or RX bug.
            f.PropertyChanged += (e, s) => Debug.WriteLine(f.A);

            // Trigger subscription to the underlying observable.
            Assert.That(f.A, Is.EqualTo(true));

            Assert.That(f.Count, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// Tests to make sure that ToProperty overload with the nameof only subscribes once.
    /// </summary>
    [Test]
    public void ToProperty_NameOf_ShouldSubscribeOnlyOnce()
    {
        using (ProductionMode.Set())
        {
            var f = new RaceConditionNameOfFixture();

            // This line is important because it triggers connect to
            // be called recursively thus cause the subscription
            // to be called twice. Not sure if this is a reactive UI
            // or RX bug.
            f.PropertyChanged += (e, s) => Debug.WriteLine(f.A);

            // Trigger subscription to the underlying observable.
            Assert.That(f.A, Is.EqualTo(true));

            Assert.That(f.Count, Is.EqualTo(1));
        }
    }
}
