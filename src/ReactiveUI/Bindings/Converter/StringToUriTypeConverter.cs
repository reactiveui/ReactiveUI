// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Converts <see cref="string"/> to <see cref="Uri"/> using <see cref="Uri.TryCreate(string?, UriKind, out Uri?)"/>.
/// </summary>
public sealed class StringToUriTypeConverter : BindingTypeConverter<string, Uri>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(string? from, object? conversionHint, [NotNullWhen(true)] out Uri? result)
    {
        if (from is null)
        {
            result = null;
            return false;
        }

        return Uri.TryCreate(from, UriKind.RelativeOrAbsolute, out result);
    }
}
