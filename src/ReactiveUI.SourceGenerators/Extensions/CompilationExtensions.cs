// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="Compilation"/> type.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// Checks whether a given compilation (assumed to be for C#) is using at least a given language version.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/> to consider for analysis.</param>
    /// <param name="languageVersion">The minimum language version to check.</param>
    /// <returns>Whether <paramref name="compilation"/> is using at least the specified language version.</returns>
    public static bool HasLanguageVersionAtLeastEqualTo(this Compilation compilation, LanguageVersion languageVersion) =>
        ((CSharpCompilation)compilation).LanguageVersion >= languageVersion;

    /// <summary>
    /// <para>
    /// Checks whether or not a type with a specified metadata name is accessible from a given <see cref="Compilation"/> instance.
    /// </para>
    /// <para>
    /// This method enumerates candidate type symbols to find a match in the following order:
    /// <list type="number">
    ///   <item><description>
    ///     If only one type with the given name is found within the compilation and its referenced assemblies, check its accessibility.
    ///   </description></item>
    ///   <item><description>
    ///     If the current <paramref name="compilation"/> defines the symbol, check its accessibility.
    ///   </description></item>
    ///   <item><description>
    ///     Otherwise, check whether the type exists and is accessible from any of the referenced assemblies.
    ///   </description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/> to consider for analysis.</param>
    /// <param name="fullyQualifiedMetadataName">The fully-qualified metadata type name to find.</param>
    /// <returns>Whether a type with the specified metadata name can be accessed from the given compilation.</returns>
    public static bool HasAccessibleTypeWithMetadataName(this Compilation compilation, string fullyQualifiedMetadataName)
    {
        // Try to get the unique type with this name
        var type = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);

        // If there is only a single matching symbol, check its accessibility
        if (type is not null)
        {
            return type.CanBeAccessedFrom(compilation.Assembly);
        }

        // Otherwise, try to get the unique type with this name originally defined in 'compilation'
        type ??= compilation.Assembly.GetTypeByMetadataName(fullyQualifiedMetadataName);

        if (type is not null)
        {
            return type.CanBeAccessedFrom(compilation.Assembly);
        }

        // Otherwise, check whether the type is defined and accessible from any of the referenced assemblies
        foreach (var module in compilation.Assembly.Modules)
        {
            foreach (var referencedAssembly in module.ReferencedAssemblySymbols)
            {
                if (referencedAssembly.GetTypeByMetadataName(fullyQualifiedMetadataName) is not INamedTypeSymbol currentType)
                {
                    continue;
                }

                switch (currentType.GetEffectiveAccessibility())
                {
                    case Accessibility.Public:
                    case Accessibility.Internal when referencedAssembly.GivesAccessTo(compilation.Assembly):
                        return true;
                    default:
                        continue;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to build a map of <see cref="INamedTypeSymbol"/> instances form the input mapping of names.
    /// </summary>
    /// <typeparam name="T">The type of keys for each symbol.</typeparam>
    /// <param name="compilation">The <see cref="Compilation"/> to consider for analysis.</param>
    /// <param name="typeNames">The input mapping of <typeparamref name="T"/> keys to fully qualified type names.</param>
    /// <param name="typeSymbols">The resulting mapping of <typeparamref name="T"/> keys to resolved <see cref="INamedTypeSymbol"/> instances.</param>
    /// <returns>Whether all requested <see cref="INamedTypeSymbol"/> instances could be resolved.</returns>
    public static bool TryBuildNamedTypeSymbolMap<T>(
        this Compilation compilation,
        IEnumerable<KeyValuePair<T, string>> typeNames,
        [NotNullWhen(true)] out ImmutableDictionary<T, INamedTypeSymbol>? typeSymbols)
        where T : IEquatable<T>
    {
        var builder = ImmutableDictionary.CreateBuilder<T, INamedTypeSymbol>();

        foreach (var pair in typeNames)
        {
            if (compilation.GetTypeByMetadataName(pair.Value) is not INamedTypeSymbol attributeSymbol)
            {
                typeSymbols = null;

                return false;
            }

            builder.Add(pair.Key, attributeSymbol);
        }

        typeSymbols = builder.ToImmutable();

        return true;
    }
}
