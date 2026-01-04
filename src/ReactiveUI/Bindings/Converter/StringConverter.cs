// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to <see cref="string"/> (identity converter).
/// </summary>
/// <remarks>
/// This converter provides a fast path for string-to-string bindings without
/// requiring reflection or TypeDescriptor.
/// </remarks>
public sealed class StringConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public Type FromType => typeof(string);

    /// <inheritdoc/>
    public Type ToType => typeof(string);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 10;

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        if (from is null)
        {
            result = null;
            return false;
        }

        if (from is string s)
        {
            result = s;
            return true;
        }

        result = null;
        return false;
    }
}
