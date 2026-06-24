// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.Converters;

/// <summary>Tests for <see cref="BindingFallbackConverterRegistry"/>.</summary>
public class BindingFallbackConverterRegistryTests
{
    /// <summary>An empty registry returns an empty converter list.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAllConvertersOnEmptyRegistryIsEmpty()
    {
        var registry = new BindingFallbackConverterRegistry();
        await Assert.That(registry.GetAllConverters()).IsEmpty();
    }

    /// <summary>A registered converter is returned by GetAllConverters.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAllConvertersReturnsRegisteredConverter()
    {
        var registry = new BindingFallbackConverterRegistry();
        var converter = new StubFallbackConverter();
        registry.Register(converter);

        await Assert.That(registry.GetAllConverters()).Contains(converter);
    }

    /// <summary>A fallback converter stub that never converts.</summary>
    private sealed class StubFallbackConverter : IBindingFallbackConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType) => 0;

        /// <inheritdoc/>
        public bool TryConvert(Type fromType, object from, Type toType, object? conversionHint, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out object? result)
        {
            result = null;
            return false;
        }
    }
}
