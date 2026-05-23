// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
    public int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        switch (from)
        {
            case null:
                {
                    result = null;
                    return false;
                }

            case string s:
                {
                    result = s;
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
