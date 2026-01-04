// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Guid"/> to <see cref="string"/> using the "D" format (standard hyphenated format).
/// </summary>
public sealed class GuidToStringTypeConverter : BindingTypeConverter<Guid, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 10;

    /// <inheritdoc/>
    public override bool TryConvert(Guid from, object? conversionHint, [NotNullWhen(true)] out string? result)
    {
        result = from.ToString("D");
        return true;
    }
}
