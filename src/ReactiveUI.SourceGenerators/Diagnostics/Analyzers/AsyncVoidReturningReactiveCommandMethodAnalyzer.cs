// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using ReactiveUI.SourceGenerators.Extensions;
using static ReactiveUI.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace ReactiveUI.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that generates a warning when using <c>[RelayCommand]</c> over an <see langword="async"/> <see cref="void"/> method.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncVoidReturningReactiveCommandMethodAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [AsyncVoidReturningReactiveCommandMethod];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbol for [RelayCommand]
            if (context.Compilation.GetTypeByMetadataName("ReactiveUI.SourceGenerators.ReactiveCommandAttribute") is not INamedTypeSymbol reactiveCommandSymbol)
            {
                return;
            }

            context.RegisterSymbolAction(
                context =>
            {
                // We're only looking for async void methods
                if (context.Symbol is not IMethodSymbol { IsAsync: true, ReturnsVoid: true } methodSymbol)
                {
                    return;
                }

                // We only care about methods annotated with [ReactiveCommand]
                if (!methodSymbol.HasAttributeWithType(reactiveCommandSymbol))
                {
                    return;
                }

                // Warn on async void methods using [ReactiveCommand] (they should return a Task instead)
                context.ReportDiagnostic(Diagnostic.Create(
                    AsyncVoidReturningReactiveCommandMethod,
                    context.Symbol.Locations.FirstOrDefault(),
                    context.Symbol));
            },
                SymbolKind.Method);
        });
    }
}
