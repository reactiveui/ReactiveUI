// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Markup;

namespace ReactiveUI.Tests.WinUI.Markup;

/// <summary>Names a type the XAML runtime already knows about, so it resolves the type from its own metadata.</summary>
/// <param name="underlyingType">The framework type being named.</param>
/// <remarks>
/// This is the marker a generated metadata provider emits for framework base types. Only the identity of the type
/// is meaningful; the runtime never asks such a type to build or describe anything.
/// </remarks>
internal sealed class FrameworkXamlType(Type underlyingType) : IXamlType
{
    /// <inheritdoc/>
    public IXamlType? BaseType => null;

    /// <inheritdoc/>
    public IXamlType? BoxedType => null;

    /// <inheritdoc/>
    public IXamlMember? ContentProperty => null;

    /// <inheritdoc/>
    public string FullName { get; } = underlyingType.FullName ?? underlyingType.Name;

    /// <inheritdoc/>
    public bool IsArray => false;

    /// <inheritdoc/>
    public bool IsBindable => true;

    /// <inheritdoc/>
    public bool IsCollection => false;

    /// <inheritdoc/>
    public bool IsConstructible => false;

    /// <inheritdoc/>
    public bool IsDictionary => false;

    /// <inheritdoc/>
    public bool IsMarkupExtension => false;

    /// <inheritdoc/>
    public IXamlType? ItemType => null;

    /// <inheritdoc/>
    public IXamlType? KeyType => null;

    /// <inheritdoc/>
    public Type UnderlyingType { get; } = underlyingType;

    /// <inheritdoc/>
    public void AddToMap(object instance, object key, object value) => throw new NotSupportedException(FullName);

    /// <inheritdoc/>
    public void AddToVector(object instance, object value) => throw new NotSupportedException(FullName);

    /// <inheritdoc/>
    public object ActivateInstance() => throw new NotSupportedException(FullName);

    /// <inheritdoc/>
    public object CreateFromString(string value) => throw new NotSupportedException(FullName);

    /// <inheritdoc/>
    public IXamlMember? GetMember(string name) => null;

    /// <inheritdoc/>
    public void RunInitializer()
    {
    }
}
