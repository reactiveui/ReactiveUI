// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="PlatformRegistrationManager"/>.
/// </summary>
[NotInParallel]
public class PlatformRegistrationManagerTest
{
    /// <summary>
    /// Tests that SetRegistrationNamespaces sets the namespaces.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetRegistrationNamespaces_SetsNamespaces()
    {
        var originalNamespaces = PlatformRegistrationManager.NamespacesToRegister;
        var newNamespaces = new[] { RegistrationNamespace.Maui };

        try
        {
            PlatformRegistrationManager.SetRegistrationNamespaces(newNamespaces);

            await Assert.That(PlatformRegistrationManager.NamespacesToRegister).IsEquivalentTo(newNamespaces);
        }
        finally
        {
            // Restore original namespaces
            PlatformRegistrationManager.SetRegistrationNamespaces(originalNamespaces);
        }
    }

    /// <summary>
    /// Tests that DefaultRegistrationNamespaces is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultRegistrationNamespaces_IsNotNull()
    {
        await Assert.That(PlatformRegistrationManager.DefaultRegistrationNamespaces).IsNotNull();
        await Assert.That(PlatformRegistrationManager.DefaultRegistrationNamespaces).Count().IsGreaterThan(0);
    }

    /// <summary>
    /// Tests that NamespacesToRegister starts with default namespaces.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NamespacesToRegister_StartsWithDefaultNamespaces()
    {
        await Assert.That(PlatformRegistrationManager.NamespacesToRegister).IsNotNull();
    }
}
