// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI.SourceGenerators.Extensions;
using ReactiveUI.SourceGenerators.Input.Models;
using ReactiveUI.SourceGenerators.Models;

namespace ReactiveUI.SourceGenerators;

/// <summary>
/// A source generator for generating reative properties.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class ReactiveGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Gather info for all annotated command methods (starting from method declarations with at least one attribute)
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, Result<PropertyInfo> Info)> propertyInfoWithErrors =
            context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "ReactiveUI.SourceGenerators.ReactiveAttribute",
                static (node, _) => node is VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Parent: FieldDeclarationSyntax { Parent: ClassDeclarationSyntax or RecordDeclarationSyntax, AttributeLists.Count: > 0 } } },
                static (context, token) =>
                {
                    var fieldDeclaration = (FieldDeclarationSyntax)context.TargetNode.Parent!.Parent!;
                    var fieldSymbol = (IFieldSymbol)context.TargetSymbol;

                    // Get the hierarchy info for the target symbol, and try to gather the property info
                    var hierarchy = HierarchyInfo.From(fieldSymbol.ContainingType);

                    token.ThrowIfCancellationRequested();

                    Execute.GetFieldInfoFromClass(fieldDeclaration, fieldSymbol, context.SemanticModel, token, out var propertyInfo, out var diagnostics);

                    token.ThrowIfCancellationRequested();
                    return (Hierarchy: hierarchy, new Result<PropertyInfo?>(propertyInfo, diagnostics));
                })
            .Where(static item => item.Hierarchy is not null)!;

        // Output the diagnostics
        context.ReportDiagnostics(propertyInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        var propertyInfo =
            propertyInfoWithErrors
            .Where(static item => item.Info.Value is not null)!;

        // Split and group by containing type
        var groupedPropertyInfo =
            propertyInfo
            .GroupBy(static item => item.Left, static item => item.Right.Value);

        // Generate the requested properties and methods
        context.RegisterSourceOutput(groupedPropertyInfo, static (context, item) =>
        {
            // Generate all member declarations for the current type
            var memberDeclarations =
                item.Right
                .Select(Execute.GetPropertySyntax)
                .ToImmutableArray();

            // Insert all members into the same partial type declaration
            var compilationUnit = item.Key.GetCompilationUnit(memberDeclarations);
            context.AddSource($"{item.Key.FilenameHint}.Properties.g.cs", compilationUnit);
        });
    }
}
