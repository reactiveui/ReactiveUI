// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace ReactiveUI.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="SuppressionDescriptors"/> instances for suppressed diagnostics by analyzers in this project.
/// </summary>
internal static class SuppressionDescriptors
{
    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a field using [ReactiveProperty] with an attribute list targeting a property.
    /// </summary>
    public static readonly SuppressionDescriptor PropertyAttributeListForReactivePropertyField = new(
        id: "RXUISPR0001",
        suppressedDiagnosticId: "CS0657",
        justification: "Fields using [ReactiveProperty] can use [property:] attribute lists to forward attributes to the generated properties");

    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a method using [ReactiveCommand] with an attribute list targeting a field or property.
    /// </summary>
    public static readonly SuppressionDescriptor FieldOrPropertyAttributeListForReactiveCommandMethod = new(
        id: "RXUISPR0002",
        suppressedDiagnosticId: "CS0657",
        justification: "Methods using [ReactiveCommand] can use [field:] and [property:] attribute lists to forward attributes to the generated fields and properties");
}
