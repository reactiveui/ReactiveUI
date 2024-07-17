// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using ReactiveUI.SourceGenerators.Helpers;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="ITypeSymbol"/> type.
/// </summary>
internal static class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> is or inherits from <paramref name="name"/>.</returns>
    public static bool HasOrInheritsFromFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasFullyQualifiedMetadataName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="name"/>.</returns>
    public static bool InheritsFromFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        var baseType = typeSymbol.BaseType;

        while (baseType is not null)
        {
            if (baseType.HasFullyQualifiedMetadataName(name))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="baseTypeSymbol">The <see cref="ITypeSymbol"/> instane to check for inheritance from.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="baseTypeSymbol"/>.</returns>
    public static bool InheritsFromType(this ITypeSymbol typeSymbol, ITypeSymbol baseTypeSymbol)
    {
        var currentBaseTypeSymbol = typeSymbol.BaseType;

        while (currentBaseTypeSymbol is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(currentBaseTypeSymbol, baseTypeSymbol))
            {
                return true;
            }

            currentBaseTypeSymbol = currentBaseTypeSymbol.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> implements an interface with a specified name.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for interface implementation.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an interface with the specified name.</returns>
    public static bool HasInterfaceWithFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        foreach (var interfaceType in typeSymbol.AllInterfaces)
        {
            if (interfaceType.HasFullyQualifiedMetadataName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="predicate">The predicate used to match available attributes.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute matching <paramref name="predicate"/>.</returns>
    public static bool HasOrInheritsAttribute(this ITypeSymbol typeSymbol, Func<AttributeData, bool> predicate)
    {
        for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.GetAttributes().Any(predicate))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The name of the attribute to look for.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute with the specified type name.</returns>
    public static bool HasOrInheritsAttributeWithFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasAttributeWithFullyQualifiedMetadataName(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> has or inherits a specified attribute.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="baseTypeSymbol">The <see cref="ITypeSymbol"/> instane to check for inheritance from.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has or inherits an attribute of type <paramref name="baseTypeSymbol"/>.</returns>
    public static bool HasOrInheritsAttributeWithType(this ITypeSymbol typeSymbol, ITypeSymbol baseTypeSymbol)
    {
        for (var currentType = typeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.HasAttributeWithType(baseTypeSymbol))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits a specified attribute.
    /// If the type has no base type, this method will automatically handle that and return <see langword="false"/>.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The name of the attribute to look for.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute with the specified type name.</returns>
    public static bool InheritsAttributeWithFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        if (typeSymbol.BaseType is INamedTypeSymbol baseTypeSymbol)
        {
            return HasOrInheritsAttributeWithFullyQualifiedMetadataName(baseTypeSymbol, name);
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given type symbol has a specified fully qualified metadata name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedMetadataName(this ITypeSymbol symbol, string name)
    {
        using var builder = ImmutableArrayBuilder<char>.Rent();

        symbol.AppendFullyQualifiedMetadataName(builder);

        return builder.WrittenSpan.SequenceEqual(name.AsSpan());
    }

    /// <summary>
    /// Gets the fully qualified metadata name for a given <see cref="ITypeSymbol"/> instance.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <returns>The fully qualified metadata name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedMetadataName(this ITypeSymbol symbol)
    {
        using var builder = ImmutableArrayBuilder<char>.Rent();

        symbol.AppendFullyQualifiedMetadataName(builder);

        return builder.ToString();
    }

    /// <summary>
    /// Appends the fully qualified metadata name for a given symbol to a target builder.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <param name="builder">The target <see cref="ImmutableArrayBuilder{T}"/> instance.</param>
    private static void AppendFullyQualifiedMetadataName(this ITypeSymbol symbol, ImmutableArrayBuilder<char> builder)
    {
        static void BuildFrom(ISymbol? symbol, ImmutableArrayBuilder<char> builder)
        {
            switch (symbol)
            {
                // Namespaces that are nested also append a leading '.'
                case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                    BuildFrom(symbol.ContainingNamespace, builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Other namespaces (ie. the one right before global) skip the leading '.'
                case INamespaceSymbol { IsGlobalNamespace: false }:
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Types with no namespace just have their metadata name directly written
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol { IsGlobalNamespace: true } }:
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Types with a containing non-global namespace also append a leading '.'
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol namespaceSymbol }:
                    BuildFrom(namespaceSymbol, builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Nested types append a leading '+'
                case ITypeSymbol { ContainingSymbol: ITypeSymbol typeSymbol }:
                    BuildFrom(typeSymbol, builder);
                    builder.Add('+');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;
                default:
                    break;
            }
        }

        BuildFrom(symbol, builder);
    }
}
