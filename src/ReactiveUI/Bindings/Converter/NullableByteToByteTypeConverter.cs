// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Nullable{Byte}"/> to <see cref="byte"/>.
/// </summary>
/// <remarks>
/// When the nullable value is null, the conversion fails and returns false.
/// </remarks>
public sealed class NullableByteToByteTypeConverter : IBindingTypeConverter<byte?, byte>
{
    /// <inheritdoc/>
    public Type FromType => typeof(byte?);

    /// <inheritdoc/>
    public Type ToType => typeof(byte);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public bool TryConvert(byte? from, object? conversionHint, out byte result)
    {
        if (from is null)
        {
            result = 0;
            return false;
        }

        result = from.Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        switch (from)
        {
            case null:
                {
                    result = null;
                    return TryConvert(null, conversionHint, out _);
                }

            case byte value when TryConvert(value, conversionHint, out var typedResult):
                {
                    result = typedResult;
                    return true;
                }

            default:
                {
                    result = null;
                    return false;
                }
        }
    }
}
