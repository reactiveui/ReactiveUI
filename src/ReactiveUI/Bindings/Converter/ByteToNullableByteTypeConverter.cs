// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="byte"/> to <see cref="Nullable{Byte}"/>.
/// </summary>
public sealed class ByteToNullableByteTypeConverter : IBindingTypeConverter<byte, byte?>
{
    /// <inheritdoc/>
    public Type FromType => typeof(byte);

    /// <inheritdoc/>
    public Type ToType => typeof(byte?);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public bool TryConvert(byte from, object? conversionHint, out byte? result)
    {
        result = from;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is byte value)
        {
            result = (byte?)value;
            return true;
        }

        result = null;
        return false;
    }
}
