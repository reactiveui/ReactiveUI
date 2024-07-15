// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Microsoft.CodeAnalysis;

#pragma warning disable IDE0090 // Use 'new DiagnosticDescriptor(...)'

namespace ReactiveUI.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
internal static class DiagnosticDescriptors
{
    /// <summary>
    /// The diagnostic id for <see cref="InheritFromReactiveObjectInsteadOfUsingINotifyPropertyChangedAttributeWarning"/>.
    /// </summary>
    public const string InheritFromReactiveObjectInsteadOfUsingINotifyPropertyChangedAttributeId = "RXUI0032";

    /// <summary>
    /// The diagnostic id for <see cref="InheritFromReactiveObjectInsteadOfUsingReactiveObjectAttributeWarning"/>.
    /// </summary>
    public const string InheritFromReactiveObjectInsteadOfUsingObservableObjectAttributeId = "RXUI0033";

    /// <summary>
    /// The diagnostic id for <see cref="FieldReferenceForReactivePropertyFieldWarning"/>.
    /// </summary>
    public const string FieldReferenceForReactivePropertyFieldId = "RXUI0034";

    /// <summary>
    /// The diagnostic id for <see cref="AsyncVoidReturningReactiveCommandMethod"/>.
    /// </summary>
    public const string AsyncVoidReturningReactiveCommandMethodId = "RXUI0039";

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an annotated method to generate a command for has an invalid signature.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidReactiveCommandMethodSignatureError = new DiagnosticDescriptor(
        id: "RXUI0007",
        title: "Invalid ReactiveCommand method signature",
        messageFormat: "The method {0}.{1} cannot be used to generate a command property, as its signature isn't compatible with any of the existing relay command types",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply [ReactiveCommand] to methods with a signature that doesn't match any of the existing relay command types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0007");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an unsupported C# language version is being used.
    /// </summary>
    public static readonly DiagnosticDescriptor UnsupportedCSharpLanguageVersionError = new DiagnosticDescriptor(
        id: "RXUI0008",
        title: "Unsupported C# language version",
        messageFormat: "The source generator features from ReactiveUI require consuming projects to set the C# language version to at least C# 8.0",
        category: typeof(UnsupportedCSharpLanguageVersionAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The source generator features from ReactiveUI require consuming projects to set the C# language version to at least C# 8.0. Make sure to add <LangVersion>8.0</LangVersion> (or above) to your .csproj file.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0008");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a specified <c>CanExecute</c> name has no matching member.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a valid member, but "{0}" has no matches in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidCanExecuteMemberNameError = new DiagnosticDescriptor(
        id: "RXUI0009",
        title: "Invalid ReactiveCommand.CanExecute member name",
        messageFormat: "The CanExecute name must refer to a valid member, but \"{0}\" has no matches in type {1}",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The CanExecute name in [ReactiveCommand] must refer to a valid member in its parent type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0009");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a specified <c>CanExecute</c> name maps to multiple members.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a single member, but "{0}" has multiple matches in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MultipleCanExecuteMemberNameMatchesError = new DiagnosticDescriptor(
        id: "RXUI0010",
        title: "Multiple ReactiveCommand.CanExecute member name matches",
        messageFormat: "The CanExecute name must refer to a single member, but \"{0}\" has multiple matches in type {1}",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot set the CanExecute name in [ReactiveCommand] to one that has multiple matches in its parent type (it must refer to a single compatible member).",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0010");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a a specified <c>CanExecute</c> name maps to an invalid member.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a compatible member, but no valid members were found for "{0}" in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidCanExecuteMemberError = new DiagnosticDescriptor(
        id: "RXUI0011",
        title: "No valid ReactiveCommand.CanExecute member match",
        messageFormat: "The CanExecute name must refer to a compatible member, but no valid members were found for \"{0}\" in type {1}",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The CanExecute name in [ReactiveCommand] must refer to a compatible member (either a property or a method) to be used in a generated command.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0011");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>ReactiveCommandAttribute.AllowConcurrentExecutions</c> is being set for a non-asynchronous method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [ReactiveCommand] attribute specifying a concurrency control option, as it maps to a non-asynchronous command type"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidConcurrentExecutionsParameterError = new DiagnosticDescriptor(
        id: "RXUI0012",
        title: "Invalid concurrency control option usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [ReactiveCommand] attribute specifying a concurrency control option, as it maps to a non-asynchronous command type",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [ReactiveCommand] attribute specifying a concurrency control option to methods mapping to non-asynchronous command types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0012");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>ReactiveCommandAttribute.IncludeCancelCommandParameter</c> is being set for an invalid method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [ReactiveCommand] attribute specifying to include a cancel command, as it does not map to an asynchronous command type taking a cancellation token"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidIncludeCancelCommandParameterError = new DiagnosticDescriptor(
        id: "RXUI0013",
        title: "Invalid include cancel command setting usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [ReactiveCommand] attribute specifying to include a cancel command, as it does not map to an asynchronous command type taking a cancellation token",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [ReactiveCommand] attribute specifying to include a cancel command to methods not mapping to an asynchronous command type accepting a cancellation token.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0013");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a generated property created with <c>[Reactive]</c> would collide with the source field.
    /// <para>
    /// Format: <c>"The field {0}.{1} cannot be used to generate an observable property, as its name would collide with the field name (instance fields should use the "lowerCamel", "_lowerCamel" or "m_lowerCamel" pattern)</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor ReactivePropertyNameCollisionError = new DiagnosticDescriptor(
        id: "RXUI0014",
        title: "Name collision for generated property",
        messageFormat: "The field {0}.{1} cannot be used to generate an observable property, as its name would collide with the field name (instance fields should use the \"lowerCamel\", \"_lowerCamel\" or \"m_lowerCamel\" pattern)",
        category: typeof(ReactiveGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The name of fields annotated with [Reactive] should use \"lowerCamel\", \"_lowerCamel\" or \"m_lowerCamel\" pattern to avoid collisions with the generated properties.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0014");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a specified <c>[ReactiveCommand]</c> method has any overloads.
    /// <para>
    /// Format: <c>"The CanExecute name must refer to a single member, but "{0}" has multiple matches in type {1}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor MultipleReactiveCommandMethodOverloadsError = new DiagnosticDescriptor(
        id: "RXUI0023",
        title: "Multiple overloads for method annotated with ReactiveCommand",
        messageFormat: "The method {0}.{1} cannot be annotated with [ReactiveCommand], has it has multiple overloads (command methods must be unique within their containing type)",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Methods with multiple overloads cannot be annotated with [ReactiveCommand], as command methods must be unique within their containing type.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0023");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <c>ReactiveCommandAttribute.FlowExceptionsToTaskScheduler</c> is being set for a non-asynchronous method.
    /// <para>
    /// Format: <c>"The method {0}.{1} cannot be annotated with the [ReactiveCommand] attribute specifying an exception flow option, as it maps to a non-asynchronous command type"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidFlowExceptionsToTaskSchedulerParameterError = new DiagnosticDescriptor(
        id: "RXUI0031",
        title: "Invalid task scheduler exception flow option usage",
        messageFormat: "The method {0}.{1} cannot be annotated with the [ReactiveCommand] attribute specifying a task scheduler exception flow option, as it maps to a non-asynchronous command type",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Cannot apply the [ReactiveCommand] attribute specifying a task scheduler exception flow option to methods mapping to non-asynchronous command types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0031");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a method with <c>[ReactiveCommand]</c> is using an invalid attribute targeting the field or property.
    /// <para>
    /// Format: <c>"The method {0} annotated with [ReactiveCommand] is using attribute "{1}" which was not recognized as a valid type (are you missing a using directive?)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidFieldOrPropertyTargetedAttributeOnReactiveCommandMethod = new DiagnosticDescriptor(
        id: "RXUI0036",
        title: "Invalid field or property targeted attribute type",
        messageFormat: "The method {0} annotated with [ReactiveCommand] is using attribute \"{1}\" which was not recognized as a valid type (are you missing a using directive?)",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated field or property for a method annotated with [ReactiveCommand] must correctly be resolved to valid types.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0036");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a method with <c>[ReactiveCommand]</c> is using an invalid attribute targeting the field or property.
    /// <para>
    /// Format: <c>"The method {0} annotated with [ReactiveCommand] is using attribute "{1}" with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidFieldOrPropertyTargetedAttributeExpressionOnReactiveCommandMethod = new DiagnosticDescriptor(
        id: "RXUI0038",
        title: "Invalid field or property targeted attribute expression",
        messageFormat: "The method {0} annotated with [ReactiveCommand] is using attribute \"{1}\" with an invalid expression (are you passing any incorrect parameters to the attribute constructor?)",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All attributes targeting the generated field or property for a method annotated with [ReactiveCommand] must be using valid expressions.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0038");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a method with <c>[ReactiveCommand]</c> is async void.
    /// <para>
    /// Format: <c>"The method {0} annotated with [ReactiveCommand] is async void (make sure to return a Task type instead)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor AsyncVoidReturningReactiveCommandMethod = new DiagnosticDescriptor(
        id: AsyncVoidReturningReactiveCommandMethodId,
        title: "Async void returning method annotated with ReactiveCommand",
        messageFormat: "The method {0} annotated with [ReactiveCommand] is async void (make sure to return a Task type instead)",
        category: typeof(ReactiveCommandGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All asynchronous methods annotated with [ReactiveCommand] should return a Task type, to benefit from the additional support provided by ReactiveCommand and ReactiveCommand<T>.",
        helpLinkUri: "https://aka.ms/mvvmtoolkit/errors/mvvmtk0039");
}
