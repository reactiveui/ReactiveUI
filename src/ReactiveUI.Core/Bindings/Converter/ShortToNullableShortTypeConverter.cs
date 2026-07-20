// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>Converts <see cref="short"/> to <see cref="short"/>?.</summary>
public sealed class ShortToNullableShortTypeConverter : IBindingTypeConverter<short, short?>
{
    /// <inheritdoc/>
    public Type FromType => typeof(short);

    /// <inheritdoc/>
    public Type ToType => typeof(short?);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public bool TryConvert(short from, object? conversionHint, out short? result)
    {
        result = from;
        return true;
    }

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is short value)
        {
            result = (short?)value;
            return true;
        }

        result = null;
        return false;
    }
}
