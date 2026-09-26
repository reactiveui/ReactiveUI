// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>
/// Converts a spice level, from 1 to 5, to a word. The level is a <see cref="byte"/> rather than an <see
/// cref="int"/> so that no typed converter shadows this fallback converter in the examples on this page.
/// </summary>
[System.Diagnostics.DebuggerDisplay("SpiceLevelFallbackConverter")]
public sealed class SpiceLevelFallbackConverter : IBindingFallbackConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType) =>
        fromType == typeof(byte) && toType == typeof(string) ? 1 : 0;

    /// <inheritdoc/>
    public bool TryConvert(Type fromType, object from, Type toType, object? conversionHint, out object? result)
    {
        result = (byte)from switch
        {
            <= 1 => "Mild",
            <= 3 => "Medium",
            _ => "Hot",
        };
        return true;
    }
}
