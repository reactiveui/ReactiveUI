// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="Uri"/> to <see cref="string"/>.
/// </summary>
public sealed class UriToStringTypeConverter : BindingTypeConverter<Uri, string>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(Uri? from, object? conversionHint, [NotNullWhen(true)] out string? result)
    {
        if (from is null)
        {
            result = null;
            return false;
        }

        result = from.ToString();
        return true;
    }
}
