// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SyntaxToken"/> type.
/// </summary>
internal static class SyntaxTokenExtensions
{
    /// <summary>
    /// Deconstructs a <see cref="SyntaxToken"/> into its <see cref="SyntaxKind"/> value.
    /// </summary>
    /// <param name="syntaxToken">The input <see cref="SyntaxToken"/> value.</param>
    /// <param name="syntaxKind">The resulting <see cref="SyntaxKind"/> value for <paramref name="syntaxToken"/>.</param>
    public static void Deconstruct(this SyntaxToken syntaxToken, out SyntaxKind syntaxKind) => syntaxKind = syntaxToken.Kind();
}
