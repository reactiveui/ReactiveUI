using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;
// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Suspension;
public class SuspensionHostExtensionsTests
{
    /// <summary>
    /// Verifies that GetAppState correctly retrieves the current app state.
    /// </summary>
    [Test]
    public void GetAppStateReturns()
    {
        var fixture = new SuspensionHost
        {
            AppState = new DummyAppState()
        };

        var result = fixture.GetAppState<DummyAppState>();

        Assert.That(result, Is.SameAs(fixture.AppState));
    }

    /// <summary>
    /// Verifies that a null <see cref="SuspensionHost"/> throws <see cref="ArgumentNullException"/> when calling SetupDefaultSuspendResume.
    /// </summary>
    [Test]
    public void NullSuspensionHostThrowsException()
    {
        Assert.That(
            static () => ((SuspensionHost)null!).SetupDefaultSuspendResume(),
            Throws.TypeOf<ArgumentNullException>());
    }

    /// <summary>
    /// Verifies that a null AppState does not throw when calling SetupDefaultSuspendResume.
    /// </summary>
    [Test]
    public void NullAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        Assert.That(() => fixture.SetupDefaultSuspendResume(), Throws.Nothing);
    }

    /// <summary>
    /// Verifies that observing AppState does not throw.
    /// </summary>
    [Test]
    public void ObserveAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        Assert.That(() => fixture.ObserveAppState<DummyAppState>().Subscribe(), Throws.Nothing);
    }

    /// <summary>
    /// Verifies that observing AppState does not throw <see cref="InvalidCastException"/>.
    /// </summary>
    [Test]
    public void ObserveAppStateDoesNotThrowInvalidCastException()
    {
        var fixture = new SuspensionHost();

        Assert.That(() => fixture.ObserveAppState<DummyAppState>().Subscribe(), Throws.Nothing);
    }
}