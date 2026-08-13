// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.UI.Xaml.Markup;

namespace ReactiveUI.Tests.WinUI.Markup;

/// <summary>Tells the XAML runtime how to find and build the ReactiveUI types named in markup.</summary>
/// <remarks>
/// <para>
/// An application normally gets this from the XAML compiler, which walks the markup at build time and emits a
/// provider covering every type it names. These tests have no compiled markup — they parse a template string at
/// runtime — so the same job is done here by reflecting over the ReactiveUI assemblies.
/// </para>
/// <para>
/// Framework types are handed back as bare names rather than described: the XAML runtime already carries its own
/// metadata for everything it ships, and a generated provider names them the same way.
/// </para>
/// </remarks>
internal sealed class TestXamlMetadataProvider : IXamlMetadataProvider
{
    /// <summary>Namespace prefixes owned by the XAML runtime itself.</summary>
    private static readonly string[] FrameworkNamespacePrefixes = ["Microsoft.UI.", "Windows.", "System."];

    /// <summary>The assemblies searched when markup names a type.</summary>
    private readonly Assembly[] _assemblies =
    [
        typeof(AutoDataTemplateBindingHook).Assembly,
        typeof(TestXamlMetadataProvider).Assembly,
    ];

    /// <summary>The descriptions handed out so far, keyed by CLR type.</summary>
    private readonly Dictionary<Type, IXamlType> _types = [];

    /// <inheritdoc/>
    public IXamlType GetXamlType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (_types.TryGetValue(type, out var xamlType))
        {
            return xamlType;
        }

        xamlType = IsFrameworkType(type) ? new FrameworkXamlType(type) : new ReflectedXamlType(this, type);
        _types[type] = xamlType;

        return xamlType;
    }

    /// <inheritdoc/>
    public IXamlType? GetXamlType(string fullName)
    {
        ArgumentNullException.ThrowIfNull(fullName);

        foreach (var assembly in _assemblies)
        {
            if (assembly.GetType(fullName, false) is { } type)
            {
                return GetXamlType(type);
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public XmlnsDefinition[] GetXmlnsDefinitions() => [];

    /// <summary>Decides whether the XAML runtime already knows a type from its own metadata.</summary>
    /// <param name="type">The type to classify.</param>
    /// <returns><see langword="true"/> when the runtime owns the type.</returns>
    private static bool IsFrameworkType(Type type)
    {
        var name = type.FullName ?? type.Name;

        return Array.Exists(FrameworkNamespacePrefixes, prefix => name.StartsWith(prefix, StringComparison.Ordinal));
    }
}
