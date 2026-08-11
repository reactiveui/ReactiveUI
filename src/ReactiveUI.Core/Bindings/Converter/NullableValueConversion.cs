// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Shared conversion shape for the <see cref="string"/>-to-nullable-value binding converters, so the
/// empty-string-is-null rule lives in one place rather than once per converted value type.
/// </summary>
internal static class NullableValueConversion
{
    /// <summary>The framework <c>TryParse(string?, out T)</c> shape.</summary>
    /// <typeparam name="T">The value type being parsed.</typeparam>
    /// <param name="input">The text to parse; never <see langword="null"/> or empty when invoked here.</param>
    /// <param name="value">The parsed value when parsing succeeds.</param>
    /// <returns><see langword="true"/> when <paramref name="input"/> was parsed.</returns>
    internal delegate bool TryParse<T>(string? input, out T value)
        where T : struct;

    /// <summary>Converts text to a nullable value, treating a null or empty input as <see langword="null"/>.</summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="from">The text to convert.</param>
    /// <param name="tryParse">The parser applied to non-empty input.</param>
    /// <param name="result">The converted value; <see langword="null"/> for empty input or a failed parse.</param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="from"/> was empty or parsed successfully; <see langword="false"/>
    /// when the text was present but unparseable, which lets a binding fall through to another converter.
    /// </returns>
    internal static bool TryConvert<T>(string? from, TryParse<T> tryParse, out T? result)
        where T : struct
    {
        if (string.IsNullOrEmpty(from))
        {
            result = null;
            return true;
        }

        if (tryParse(from, out var value))
        {
            result = value;
            return true;
        }

        result = null;
        return false;
    }
}
