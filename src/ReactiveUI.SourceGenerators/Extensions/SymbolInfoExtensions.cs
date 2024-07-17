// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SymbolInfo"/> type.
/// </summary>
internal static class SymbolInfoExtensions
{
    /// <summary>
    /// Tries to get the resolved attribute type symbol from a given <see cref="SymbolInfo"/> value.
    /// </summary>
    /// <param name="symbolInfo">The <see cref="SymbolInfo"/> value to check.</param>
    /// <param name="typeSymbol">The resulting attribute type symbol, if correctly resolved.</param>
    /// <returns>Whether <paramref name="symbolInfo"/> is resolved to a symbol.</returns>
    /// <remarks>
    /// This can be used to ensure users haven't eg. spelled names incorrecty or missed a using directive. Normally, code would just
    /// not compile if that was the case, but that doesn't apply for attributes using invalid targets. In that case, Roslyn will ignore
    /// any errors, meaning the generator has to validate the type symbols are correctly resolved on its own.
    /// </remarks>
    public static bool TryGetAttributeTypeSymbol(this SymbolInfo symbolInfo, [NotNullWhen(true)] out INamedTypeSymbol? typeSymbol)
    {
        var attributeSymbol = symbolInfo.Symbol;

        // If no symbol is selected and there is a single candidate symbol, use that
        if (attributeSymbol is null && symbolInfo.CandidateSymbols is [ISymbol candidateSymbol])
        {
            attributeSymbol = candidateSymbol;
        }

        // Extract the symbol from either the current one or the containing type
        if ((attributeSymbol as INamedTypeSymbol ?? attributeSymbol?.ContainingType) is not INamedTypeSymbol resultingSymbol)
        {
            typeSymbol = null;

            return false;
        }

        typeSymbol = resultingSymbol;

        return true;
    }
}
