// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
/// that don't have DataTemplates, and assigns a default DataTemplate that
/// loads the View associated with each ViewModel.
/// </summary>
public class AutoDataTemplateBindingHook : IPropertyBindingHook
{
    /// <summary>Gets the default item template.</summary>
    public static Lazy<DataTemplate> DefaultItemTemplate { get; } = new(static () =>
    {
        // The clr-namespace in the inline XAML template must match the namespace
        // this type is actually compiled into. Under REACTIVE_SHIM the shared
        // source is recompiled into the ReactiveUI.Reactive namespace (see the
        // conditional namespace above), so the XAML must reference that namespace
        // too — otherwise XamlReader.Parse throws a XamlObjectReaderException
        // because '{clr-namespace:ReactiveUI;assembly=ReactiveUI.Wpf.Reactive}'
        // cannot resolve ViewModelViewHost. See issue #4398.
#if REACTIVE_SHIM
        const string XamlClrNamespace = "clr-namespace:ReactiveUI.Reactive";
#else
        const string XamlClrNamespace = "clr-namespace:ReactiveUI";
#endif
        const string Template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                                "xmlns:xaml='" + XamlClrNamespace + ";assembly=__ASSEMBLYNAME__'> " +
                                "<xaml:ViewModelViewHost ViewModel=\"{Binding Mode=OneWay}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
                                "</DataTemplate>";

        var assemblyName = typeof(AutoDataTemplateBindingHook).Assembly.FullName;
        assemblyName = assemblyName?.Substring(0, assemblyName.IndexOf(",", StringComparison.Ordinal));

#if NET8_0_OR_GREATER
        return (DataTemplate)XamlReader.Parse(Template.Replace("__ASSEMBLYNAME__", assemblyName ?? string.Empty, StringComparison.Ordinal));
#else
        return (DataTemplate)XamlReader.Parse(Template.Replace("__ASSEMBLYNAME__", assemblyName ?? string.Empty));
#endif
    });

    /// <inheritdoc/>
    public bool ExecuteHook(
        object? source,
        object target,
        Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
        Func<IObservedChange<object, object>[]> getCurrentViewProperties,
        BindingDirection direction)
    {
        ArgumentExceptionHelper.ThrowIfNull(getCurrentViewProperties);

        var viewProperties = getCurrentViewProperties();
        var lastViewProperty = viewProperties.Length > 0 ? viewProperties[^1] : null;

        if (lastViewProperty?.Sender is not ItemsControl itemsControl)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(itemsControl.DisplayMemberPath))
        {
            return true;
        }

        if (viewProperties[^1].GetPropertyName() != "ItemsSource")
        {
            return true;
        }

        if (itemsControl.ItemTemplate is not null)
        {
            return true;
        }

        if (itemsControl.ItemTemplateSelector is not null)
        {
            return true;
        }

        itemsControl.ItemTemplate = DefaultItemTemplate.Value;
        return true;
    }
}
