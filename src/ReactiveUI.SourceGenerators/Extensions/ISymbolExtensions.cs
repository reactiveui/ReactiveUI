// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="ISymbol"/> type.
/// </summary>
internal static class ISymbolExtensions
{
    /// <summary>
    /// Gets the fully qualified name for a given symbol.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The fully qualified name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedName(this ISymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Gets the fully qualified name for a given symbol, including nullability annotations.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The fully qualified name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedNameWithNullabilityAnnotations(this ISymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier));

    /// <summary>
    /// Checks whether or not a given type symbol has a specified full name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedName(this ISymbol symbol, string name) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == name;

    /// <summary>
    /// Checks whether or not a given symbol has an attribute with the specified fully qualified metadata name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The attribute name to look for.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
    public static bool HasAttributeWithFullyQualifiedMetadataName(this ISymbol symbol, string name)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.HasFullyQualifiedMetadataName(name) == true)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given symbol has an attribute with the specified type.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> instance for the attribute type to look for.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified type.</returns>
    public static bool HasAttributeWithType(this ISymbol symbol, ITypeSymbol typeSymbol) => TryGetAttributeWithType(symbol, typeSymbol, out _);

    /// <summary>
    /// Tries to get an attribute with the specified type.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="typeSymbol">The <see cref="ITypeSymbol"/> instance for the attribute type to look for.</param>
    /// <param name="attributeData">The resulting attribute, if it was found.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified type.</returns>
    public static bool TryGetAttributeWithType(this ISymbol symbol, ITypeSymbol typeSymbol, [NotNullWhen(true)] out AttributeData? attributeData)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, typeSymbol))
            {
                attributeData = attribute;

                return true;
            }
        }

        attributeData = null;

        return false;
    }

#if !ROSLYN_4_3_1_OR_GREATER
    /// <summary>
    /// Tries to get an attribute with the specified fully qualified metadata name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The attribute name to look for.</param>
    /// <param name="attributeData">The resulting attribute, if it was found.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
    public static bool TryGetAttributeWithFullyQualifiedMetadataName(this ISymbol symbol, string name, [NotNullWhen(true)] out AttributeData? attributeData)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.HasFullyQualifiedMetadataName(name) == true)
            {
                attributeData = attribute;

                return true;
            }
        }

        attributeData = null;

        return false;
    }
#endif

    /// <summary>
    /// Calculates the effective accessibility for a given symbol.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol"/> instance to check.</param>
    /// <returns>The effective accessibility for <paramref name="symbol"/>.</returns>
    public static Accessibility GetEffectiveAccessibility(this ISymbol symbol)
    {
        // Start by assuming it's visible
        var visibility = Accessibility.Public;

        // Handle special cases
        switch (symbol.Kind)
        {
            case SymbolKind.Alias: return Accessibility.Private;
            case SymbolKind.Parameter: return GetEffectiveAccessibility(symbol.ContainingSymbol);
            case SymbolKind.TypeParameter: return Accessibility.Private;
        }

        // Traverse the symbol hierarchy to determine the effective accessibility
        while (symbol is not null && symbol.Kind != SymbolKind.Namespace)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return Accessibility.Private;
                case Accessibility.Internal:
                case Accessibility.ProtectedAndInternal:
                    visibility = Accessibility.Internal;
                    break;
            }

            symbol = symbol.ContainingSymbol;
        }

        return visibility;
    }

    /// <summary>
    /// Checks whether or not a given symbol can be accessed from a specified assembly.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="assembly">The assembly to check the accessibility of <paramref name="symbol"/> for.</param>
    /// <returns>Whether <paramref name="assembly"/> can access <paramref name="symbol"/>.</returns>
    public static bool CanBeAccessedFrom(this ISymbol symbol, IAssemblySymbol assembly)
    {
        var accessibility = symbol.GetEffectiveAccessibility();

        return
            accessibility == Accessibility.Public ||
            (accessibility == Accessibility.Internal && symbol.ContainingAssembly.GivesAccessTo(assembly));
    }
}
