// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ReactiveUI.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="IncrementalGeneratorInitializationContext"/>.
/// </summary>
internal static class IncrementalGeneratorInitializationContextExtensions
{
    /// <summary>
    /// Conditionally invokes <see cref="IncrementalGeneratorInitializationContext.RegisterSourceOutput{TSource}(IncrementalValueProvider{TSource}, Action{SourceProductionContext, TSource})"/>
    /// if the value produced by the input <see cref="IncrementalValueProvider{TValue}"/> is <see langword="true"/>.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> value being used.</param>
    /// <param name="source">The source <see cref="IncrementalValueProvider{TValues}"/> instance.</param>
    /// <param name="action">The conditional <see cref="Action"/> to invoke.</param>
    public static void RegisterConditionalSourceOutput(
        this in IncrementalGeneratorInitializationContext context,
        in IncrementalValueProvider<bool> source,
        Action<SourceProductionContext> action) => context.RegisterSourceOutput(source, (context, condition) =>
                                                        {
                                                            if (condition)
                                                            {
                                                                action(context);
                                                            }
                                                        });

    /// <summary>
    /// Conditionally invokes <see cref="IncrementalGeneratorInitializationContext.RegisterImplementationSourceOutput{TSource}(IncrementalValueProvider{TSource}, Action{SourceProductionContext, TSource})"/>
    /// if the value produced by the input <see cref="IncrementalValueProvider{TValue}"/> is <see langword="true"/>, and also supplying a given input state.
    /// </summary>
    /// <typeparam name="T">The type of state to pass to the source production callback to invoke.</typeparam>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> value being used.</param>
    /// <param name="source">The source <see cref="IncrementalValueProvider{TValues}"/> instance.</param>
    /// <param name="action">The conditional <see cref="Action{T}"/> to invoke.</param>
    public static void RegisterConditionalImplementationSourceOutput<T>(
        this in IncrementalGeneratorInitializationContext context,
        in IncrementalValueProvider<(bool Condition, T State)> source,
        Action<SourceProductionContext, T> action) =>
        context.RegisterImplementationSourceOutput(source, (context, item) =>
        {
            if (item.Condition)
            {
                action(context, item.State);
            }
        });

    /// <summary>
    /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
    public static void ReportDiagnostics(this in IncrementalGeneratorInitializationContext context, in IncrementalValuesProvider<ImmutableArray<Diagnostic>> diagnostics) =>
        context.RegisterSourceOutput(diagnostics, static (context, diagnostics) =>
        {
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        });

    /// <summary>
    /// Registers an output node into an <see cref="IncrementalGeneratorInitializationContext"/> to output diagnostics.
    /// </summary>
    /// <param name="context">The input <see cref="IncrementalGeneratorInitializationContext"/> instance.</param>
    /// <param name="diagnostics">The input <see cref="IncrementalValuesProvider{TValues}"/> sequence of diagnostics.</param>
    public static void ReportDiagnostics(this in IncrementalGeneratorInitializationContext context, in IncrementalValuesProvider<Diagnostic> diagnostics) =>
        context.RegisterSourceOutput(diagnostics, static (context, diagnostic) =>
            context.ReportDiagnostic(diagnostic));
}
