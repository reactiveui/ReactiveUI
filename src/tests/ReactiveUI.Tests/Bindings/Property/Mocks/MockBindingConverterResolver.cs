// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Bindings.Property.Mocks;

/// <summary>Test mock for <see cref="IBindingConverterResolver"/>.</summary>
/// <remarks>
/// This mock uses simple dictionary-based lookups for testing binding converter resolution
/// without requiring the full Splat/RxConverters infrastructure.
/// </remarks>
internal sealed class MockBindingConverterResolver : IBindingConverterResolver
{
    /// <summary>The registered type converters keyed by source and target type.</summary>
    private readonly Dictionary<(Type From, Type To), object?> _converters = [];

    /// <summary>The registered set-method converters keyed by source and target type.</summary>
    private readonly Dictionary<(Type? From, Type? To), Func<object?, object?, object?[]?, object?>?>
        _setMethodConverters = [];

    /// <summary>Registers a converter for testing.</summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <param name="converter">The converter instance to return.</param>
    public void RegisterConverter(Type fromType, Type toType, object converter)
    {
        ArgumentNullException.ThrowIfNull(fromType);
        ArgumentNullException.ThrowIfNull(toType);
        ArgumentNullException.ThrowIfNull(converter);

        _converters[(fromType, toType)] = converter;
    }

    /// <summary>Registers a set-method converter for testing.</summary>
    /// <param name="fromType">The source type (may be null).</param>
    /// <param name="toType">The target type (may be null).</param>
    /// <param name="converter">The converter function to return.</param>
    public void RegisterSetMethodConverter(
        Type? fromType,
        Type? toType,
        Func<object?, object?, object?[]?, object?> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        _setMethodConverters[(fromType, toType)] = converter;
    }

    /// <inheritdoc/>
    public object? GetBindingConverter(Type fromType, Type toType)
    {
        ArgumentNullException.ThrowIfNull(fromType);
        ArgumentNullException.ThrowIfNull(toType);

        return _converters.TryGetValue((fromType, toType), out var converter) ? converter : null;
    }

    /// <inheritdoc/>
    public Func<object?, object?, object?[]?, object?>? GetSetMethodConverter(Type? fromType, Type? toType) =>
        _setMethodConverters.TryGetValue((fromType, toType), out var converter) ? converter : null;
}
