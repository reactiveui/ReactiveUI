// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI;

/// <summary>
/// Fallback converter using System.ComponentModel.TypeDescriptor for reflection-based type conversion.
/// This converter is consulted only when no typed converter matches.
/// </summary>
/// <remarks>
/// This converter uses reflection and is not AOT-safe. It should be used as a last resort when no typed converter can
/// handle the conversion. The shared conversion logic lives in <see cref="ComponentModelConversion"/>.
/// </remarks>
public sealed class ComponentModelFallbackConverter : IBindingFallbackConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType) =>
        ComponentModelConversion.GetAffinity(fromType, toType);

    /// <inheritdoc/>
    public bool TryConvert(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
        object from,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType,
        object? conversionHint,
        [NotNullWhen(true)] out object? result) =>
        ComponentModelConversion.TryConvert(this, fromType, from, toType, out result);
}
