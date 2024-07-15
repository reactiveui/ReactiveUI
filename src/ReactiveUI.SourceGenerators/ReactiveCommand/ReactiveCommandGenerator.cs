// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI.SourceGenerators.Extensions;
using ReactiveUI.SourceGenerators.Helpers;
using ReactiveUI.SourceGenerators.Input.Models;
using ReactiveUI.SourceGenerators.Models;

namespace ReactiveUI.SourceGenerators;

/// <summary>
/// A source generator for generating command properties from annotated methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class ReactiveCommandGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Gather info for all annotated command methods (starting from method declarations with at least one attribute)
        IncrementalValuesProvider<(ImmutableArray<HierarchyInfo> Hierarchy, Result<ImmutableArray<CommandInfo>> Info)> commandInfoWithErrors =
            context.SyntaxProvider
                .ForAllAttributes(
                static (node, _) => node is ClassDeclarationSyntax,
                static (context, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    using var hierarchys = ImmutableArrayBuilder<HierarchyInfo>.Rent();
                    using var commandInfos = ImmutableArrayBuilder<CommandInfo>.Rent();

                    if (context.Node is ClassDeclarationSyntax declaredClass)
                    {
                        if (declaredClass.Modifiers.Any(SyntaxKind.PartialKeyword))
                        {
                            var compilation = context.SemanticModel.Compilation;
                            var semanticModel = compilation.GetSemanticModel(context.SemanticModel.SyntaxTree);
                            Execute.GetCommandInfoFromClass(
                                hierarchys,
                                compilation,
                                semanticModel,
                                declaredClass,
                                out var commandInfo);

                            if (commandInfo?.CommandExtensionInfos.IsEmpty == false)
                            {
                                commandInfos.Add(commandInfo);
                            }
                        }
                    }

                    ImmutableArray<DiagnosticInfo> diagnostics = default;
                    return (Hierarchy: hierarchys.ToImmutable(), new Result<ImmutableArray<CommandInfo>>(commandInfos.ToImmutable(), diagnostics));
                })
            .Where(static item => item.Hierarchy.Any())!;

        ////// Output the diagnostics
        ////context.ReportDiagnostics(commandInfoWithErrors.Select(static (item, _) => item.Info.Errors));

        // Get the filtered sequence to enable caching
        var commandInfos = commandInfoWithErrors
            .Where(static item => item.Info.Value.All(x => x is not null))!;

        // Generate the commands
        context.RegisterSourceOutput(commandInfos, static (context, item) =>
        {
            var mergedInfoAndHierarchy = item.Hierarchy.Zip(item.Info.Value, (hierarchy, info) => (Hierarchy: hierarchy, Info: info));
            foreach (var (hierarchy1, info1) in mergedInfoAndHierarchy)
            {
                context.AddSource($"{hierarchy1.FilenameHint}.ReactiveCommands.g.cs", Execute.GetSyntax(info1));
            }
        });
    }
}
