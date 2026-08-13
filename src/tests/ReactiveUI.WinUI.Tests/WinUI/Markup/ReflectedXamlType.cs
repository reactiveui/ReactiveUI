// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Markup;

namespace ReactiveUI.Tests.WinUI.Markup;

/// <summary>Describes a type to the XAML runtime using reflection over its CLR members.</summary>
/// <param name="provider">Resolves related types such as the base type and member types.</param>
/// <param name="underlyingType">The type being described.</param>
/// <remarks>
/// The XAML compiler normally generates this description at build time. The tests parse their markup at runtime
/// instead, so the description is derived from the type itself.
/// </remarks>
internal sealed class ReflectedXamlType(TestXamlMetadataProvider provider, Type underlyingType) : IXamlType
{
    /// <summary>The members described so far, keyed by name.</summary>
    private readonly Dictionary<string, IXamlMember?> _members = new(StringComparer.Ordinal);

    /// <inheritdoc/>
    public IXamlType? BaseType => underlyingType.BaseType is { } baseType ? provider.GetXamlType(baseType) : null;

    /// <inheritdoc/>
    public IXamlType? BoxedType => null;

    /// <inheritdoc/>
    public IXamlMember? ContentProperty => null;

    /// <inheritdoc/>
    public string FullName => underlyingType.FullName ?? underlyingType.Name;

    /// <inheritdoc/>
    public bool IsArray => underlyingType.IsArray;

    /// <inheritdoc/>
    public bool IsBindable => true;

    /// <inheritdoc/>
    public bool IsCollection => false;

    /// <inheritdoc/>
    public bool IsConstructible => !underlyingType.IsAbstract && underlyingType.GetConstructor(Type.EmptyTypes) is not null;

    /// <inheritdoc/>
    public bool IsDictionary => false;

    /// <inheritdoc/>
    public bool IsMarkupExtension => false;

    /// <inheritdoc/>
    public IXamlType? ItemType => null;

    /// <inheritdoc/>
    public IXamlType? KeyType => null;

    /// <inheritdoc/>
    public Type UnderlyingType => underlyingType;

    /// <inheritdoc/>
    public void AddToMap(object instance, object key, object value) => throw new NotSupportedException(FullName);

    /// <inheritdoc/>
    public void AddToVector(object instance, object value) => throw new NotSupportedException(FullName);

    /// <inheritdoc/>
    public object ActivateInstance() =>
        Activator.CreateInstance(underlyingType) ?? throw new InvalidOperationException($"Could not create a {FullName}.");

    /// <inheritdoc/>
    public object CreateFromString(string value) =>
        underlyingType.IsEnum
            ? Enum.Parse(underlyingType, value, true)
            : Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public IXamlMember? GetMember(string name)
    {
        if (_members.TryGetValue(name, out var member))
        {
            return member;
        }

        var property = underlyingType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        member = property is null ? null : new ReflectedXamlMember(provider, this, property);
        _members[name] = member;

        return member;
    }

    /// <inheritdoc/>
    public void RunInitializer() => RuntimeHelpers.RunClassConstructor(underlyingType.TypeHandle);
}
