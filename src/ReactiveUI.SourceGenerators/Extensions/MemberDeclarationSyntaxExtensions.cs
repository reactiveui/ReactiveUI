// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="MemberDeclarationSyntax"/> type.
/// </summary>
internal static class MemberDeclarationSyntaxExtensions
{
    /// <summary>
    /// Replaces a specific modifier.
    /// </summary>
    /// <param name="memberDeclaration">The input <see cref="MemberDeclarationSyntax"/> instance.</param>
    /// <param name="oldKind">The target modifier kind to replace.</param>
    /// <param name="newKind">The new modifier kind to add or replace.</param>
    /// <returns>A <see cref="MemberDeclarationSyntax"/> instance with the target modifier.</returns>
    public static MemberDeclarationSyntax ReplaceModifier(this MemberDeclarationSyntax memberDeclaration, SyntaxKind oldKind, SyntaxKind newKind)
    {
        var index = memberDeclaration.Modifiers.IndexOf(oldKind);

        if (index != -1)
        {
            return memberDeclaration.WithModifiers(memberDeclaration.Modifiers.Replace(memberDeclaration.Modifiers[index], Token(newKind)));
        }

        return memberDeclaration;
    }

    /// <summary>
    /// Removes a specific modifier.
    /// </summary>
    /// <param name="memberDeclaration">The input <see cref="MemberDeclarationSyntax"/> instance.</param>
    /// <param name="kind">The modifier kind to remove.</param>
    /// <returns>A <see cref="MemberDeclarationSyntax"/> instance without the specified modifier.</returns>
    public static MemberDeclarationSyntax RemoveModifier(this MemberDeclarationSyntax memberDeclaration, SyntaxKind kind)
    {
        var index = memberDeclaration.Modifiers.IndexOf(kind);

        if (index != -1)
        {
            return memberDeclaration.WithModifiers(memberDeclaration.Modifiers.RemoveAt(index));
        }

        return memberDeclaration;
    }
}
