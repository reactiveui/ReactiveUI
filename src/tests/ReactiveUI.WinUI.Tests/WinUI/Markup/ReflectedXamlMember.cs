// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

namespace ReactiveUI.Tests.WinUI.Markup;

/// <summary>Describes one settable property of a type to the XAML runtime, using reflection.</summary>
/// <param name="provider">Resolves the property's own type.</param>
/// <param name="targetType">The type declaring the property.</param>
/// <param name="property">The property being described.</param>
internal sealed class ReflectedXamlMember(TestXamlMetadataProvider provider, IXamlType targetType, PropertyInfo property) : IXamlMember
{
    /// <inheritdoc/>
    public bool IsAttachable => false;

    /// <inheritdoc/>
    public bool IsDependencyProperty => FindDependencyProperty(property) is not null;

    /// <inheritdoc/>
    public bool IsReadOnly => !property.CanWrite;

    /// <inheritdoc/>
    public string Name => property.Name;

    /// <inheritdoc/>
    public IXamlType TargetType => targetType;

    /// <inheritdoc/>
    public IXamlType Type => provider.GetXamlType(property.PropertyType);

    /// <inheritdoc/>
    public object? GetValue(object instance) => property.GetValue(instance);

    /// <inheritdoc/>
    public void SetValue(object instance, object value)
    {
        // A binding is not a value of the property's type: it has to be attached to the backing dependency
        // property so the framework evaluates it, exactly as a compiler-generated member would.
        if (value is BindingBase binding && instance is DependencyObject target && FindDependencyProperty(property) is { } dependencyProperty)
        {
            BindingOperations.SetBinding(target, dependencyProperty, binding);
            return;
        }

        property.SetValue(instance, value);
    }

    /// <summary>Finds the dependency property backing a property, if it has one.</summary>
    /// <param name="property">The property whose backing dependency property is wanted.</param>
    /// <returns>The dependency property, or <see langword="null"/> when the property is a plain CLR property.</returns>
    private static DependencyProperty? FindDependencyProperty(PropertyInfo property)
    {
        var name = $"{property.Name}Property";
        var declaring = property.DeclaringType;

        while (declaring is not null)
        {
            if (declaring.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)?.GetValue(null) is DependencyProperty field)
            {
                return field;
            }

            if (declaring.GetProperty(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)?.GetValue(null) is DependencyProperty accessor)
            {
                return accessor;
            }

            declaring = declaring.BaseType;
        }

        return null;
    }
}
