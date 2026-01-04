// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts any value to <see cref="bool"/> by comparing it with a hint value using <see cref="object.Equals(object, object)"/>.
/// </summary>
/// <remarks>
/// <para>
/// This converter is useful for binding scenarios where you need to determine if a value
/// equals a specific comparison value. The comparison value should be provided via the
/// <c>conversionHint</c> parameter.
/// </para>
/// <para>
/// Example: Convert an enum value to bool by comparing with a specific enum member.
/// </para>
/// </remarks>
public sealed class EqualityTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public Type FromType => typeof(object);

    /// <inheritdoc/>
    public Type ToType => typeof(bool);

    /// <inheritdoc/>
    public int GetAffinityForObjects() => 5;

    /// <inheritdoc/>
    public bool TryConvertTyped(object? from, object? conversionHint, [NotNullWhen(true)] out object? result)
    {
        // Always return a bool result
        result = Equals(from, conversionHint);
        return true;
    }
}
