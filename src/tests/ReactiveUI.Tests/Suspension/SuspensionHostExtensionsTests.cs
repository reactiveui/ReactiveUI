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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAppStateReturns()
    {
        var fixture = new SuspensionHost
        {
            AppState = new DummyAppState()
        };

        var result = fixture.GetAppState<DummyAppState>();

        await Assert.That(result).IsSameReferenceAs(fixture.AppState);
    }

    /// <summary>
    /// Verifies that a null <see cref="SuspensionHost"/> throws <see cref="ArgumentNullException"/> when calling SetupDefaultSuspendResume.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullSuspensionHostThrowsException()
    {
        await Assert.That(static () => ((SuspensionHost)null!).SetupDefaultSuspendResume()).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that a null AppState does not throw when calling SetupDefaultSuspendResume.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.SetupDefaultSuspendResume()).ThrowsNothing();
    }

    /// <summary>
    /// Verifies that observing AppState does not throw.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateDoesNotThrowException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.ObserveAppState<DummyAppState>().Subscribe()).ThrowsNothing();
    }

    /// <summary>
    /// Verifies that observing AppState does not throw <see cref="InvalidCastException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveAppStateDoesNotThrowInvalidCastException()
    {
        var fixture = new SuspensionHost();

        await Assert.That(() => fixture.ObserveAppState<DummyAppState>().Subscribe()).ThrowsNothing();
    }
}
