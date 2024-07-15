// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="MethodDeclarationSyntax"/> type.
/// </summary>
internal static class MethodDeclarationSyntaxExtensions
{
    /// <summary>
    /// Checks whether a given <see cref="MethodDeclarationSyntax"/> has or could potentially have any attribute lists.
    /// </summary>
    /// <param name="methodDeclaration">The input <see cref="MethodDeclarationSyntax"/> to check.</param>
    /// <returns>Whether <paramref name="methodDeclaration"/> has or potentially has any attribute lists.</returns>
    public static bool HasOrPotentiallyHasAttributeLists(this MethodDeclarationSyntax methodDeclaration)
    {
        // If the declaration has any attribute lists, there's nothing left to do
        if (methodDeclaration.AttributeLists.Count > 0)
        {
            return true;
        }

        // If there are no attributes, check whether the method declaration has the partial keyword. If it
        // does, there could potentially be attribute lists on the other partial definition/implementation.
        return methodDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword);
    }
}
