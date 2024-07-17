// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SyntaxNode"/> type.
/// </summary>
internal static class TypeDeclarationSyntaxExtensions
{
    /// <summary>
    /// Checks whether a given <see cref="TypeDeclarationSyntax"/> has or could possibly have any base types, using only syntax.
    /// </summary>
    /// <param name="typeDeclaration">The input <see cref="TypeDeclarationSyntax"/> instance to check.</param>
    /// <returns>Whether <paramref name="typeDeclaration"/> has or could possibly have any base types.</returns>
    public static bool HasOrPotentiallyHasBaseTypes(this TypeDeclarationSyntax typeDeclaration)
    {
        // If the base types list is not empty, the type can definitely has implemented interfaces
        if (typeDeclaration.BaseList is { Types.Count: > 0 })
        {
            return true;
        }

        // If the base types list is empty, check if the type is partial. If it is, it means
        // that there could be another partial declaration with a non-empty base types list.
        foreach (var modifier in typeDeclaration.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether a given <see cref="TypeDeclarationSyntax"/> has or could possibly have any attributes, using only syntax.
    /// </summary>
    /// <param name="typeDeclaration">The input <see cref="TypeDeclarationSyntax"/> instance to check.</param>
    /// <returns>Whether <paramref name="typeDeclaration"/> has or could possibly have any attributes.</returns>
    public static bool HasOrPotentiallyHasAttributes(this TypeDeclarationSyntax typeDeclaration)
    {
        // If the type has any attributes lists, then clearly it can have attributes
        if (typeDeclaration.AttributeLists.Count > 0)
        {
            return true;
        }

        // If the declaration has no attribute lists, check if the type is partial. If it is, it means
        // that there could be another partial declaration with some attribute lists over them.
        foreach (var modifier in typeDeclaration.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword))
            {
                return true;
            }
        }

        return false;
    }
}
