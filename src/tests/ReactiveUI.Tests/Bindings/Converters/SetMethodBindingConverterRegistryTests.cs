// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>Tests for <see cref="SetMethodBindingConverterRegistry"/>.</summary>
public class SetMethodBindingConverterRegistryTests
{
    /// <summary>An empty registry returns null from TryGetConverter and an empty converter list.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task EmptyRegistryHasNoConverters()
    {
        var registry = new SetMethodBindingConverterRegistry();
        await Assert.That(registry.TryGetConverter(typeof(string), typeof(int))).IsNull();
        await Assert.That(registry.GetAllConverters()).IsEmpty();
    }

    /// <summary>A registered converter is returned by GetAllConverters.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAllConvertersReturnsRegisteredConverter()
    {
        var registry = new SetMethodBindingConverterRegistry();
        var converter = new StubSetMethodConverter();
        registry.Register(converter);

        await Assert.That(registry.GetAllConverters()).Contains(converter);
    }

    /// <summary>A set-method converter stub that supports nothing.</summary>
    private sealed class StubSetMethodConverter : ISetMethodBindingConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type? fromType, Type? toType) => 0;

        /// <inheritdoc/>
        public object? PerformSet(object? toTarget, object? newValue, object?[]? arguments) => toTarget;
    }
}
