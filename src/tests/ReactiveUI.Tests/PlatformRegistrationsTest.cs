// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="PlatformRegistrations"/>.
/// </summary>
public class PlatformRegistrationsTest
{
    /// <summary>
    /// Tests that Register throws for null registerFunction.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_NullRegisterFunction_Throws()
    {
        var registrations = new PlatformRegistrations();

        await Assert.That(() => registrations.Register(null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that Register calls registerFunction for ComponentModelTypeConverter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_RegistersComponentModelTypeConverter()
    {
        var registrations = new PlatformRegistrations();
        var registered = new List<(Type ServiceType, object Instance)>();

        registrations.Register((factory, serviceType) =>
        {
            registered.Add((serviceType, factory()));
        });

        await Assert.That(registered).Count().IsGreaterThan(0);
        var typeConverterRegistration = registered.FirstOrDefault(x => x.ServiceType == typeof(IBindingTypeConverter));
        await Assert.That(typeConverterRegistration.Instance).IsNotNull();
        await Assert.That(typeConverterRegistration.Instance).IsTypeOf<ComponentModelTypeConverter>();
    }

    /// <summary>
    /// Tests that Register completes without throwing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Register_CompletesSuccessfully()
    {
        var registrations = new PlatformRegistrations();

        registrations.Register((factory, serviceType) => { });

        await Task.CompletedTask;
    }
}
