// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts nullable <see cref="long"/> values to <see cref="string"/>.
/// </summary>
public sealed class NullableLongToStringTypeConverter : BindingTypeConverter<long?, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(long? from, object? conversionHint, out string? result)
    {
        if (!from.HasValue)
        {
            result = null;
            return true;
        }

        switch (conversionHint)
        {
            case int width:
                {
                    result = from.Value.ToString($"D{width}");
                    return true;
                }

            case string format:
                {
                    result = from.Value.ToString(format);
                    return true;
                }

            default:
                {
                    result = from.Value.ToString();
                    return true;
                }
        }
    }
}
