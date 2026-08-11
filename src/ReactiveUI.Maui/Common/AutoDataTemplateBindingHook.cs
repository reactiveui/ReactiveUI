// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
#if WINUI_TARGET
using System.Diagnostics;
using Microsoft.UI.Xaml.Markup;

// Alias rather than import Microsoft.UI.Xaml: the Maui-windows TFM also imports Microsoft.Maui.Controls implicitly,
// so a bare DataTemplate would be ambiguous with Microsoft.Maui.Controls.DataTemplate.
using DataTemplate = Microsoft.UI.Xaml.DataTemplate;

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
[DebuggerDisplay("AutoDataTemplateBindingHook")]
public class AutoDataTemplateBindingHook : IPropertyBindingHook
{
    /// <summary>Gets the default item template.</summary>
    public static Lazy<DataTemplate> DefaultItemTemplate { get; } = new(static () =>
    {
        // WinUI's XAML parser only understands the 'using:' prefix; 'clr-namespace:' is WPF-only and makes
        // XamlReader.Load throw XamlParseException for an unknown namespace. The namespace must also match the
        // one this type is actually compiled into: under REACTIVE_SHIM the shared source is recompiled into
        // ReactiveUI.Reactive (see the conditional namespace above), so the XAML has to follow it. This is the
        // WinUI counterpart of the WPF fix in issue #4398.
#if REACTIVE_SHIM
        const string XamlNamespace = "using:ReactiveUI.Reactive";
#else
        const string XamlNamespace = "using:ReactiveUI";
#endif
        const string Template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' "
                 + $"xmlns:xaml='{XamlNamespace}'>"
             + "<xaml:ViewModelViewHost ViewModel=\"{Binding Mode=OneWay}\" "
             + "VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" "
             + "IsTabStop=\"False\" />"
         + "</DataTemplate>";

        return (DataTemplate)XamlReader.Load(Template);
    });

    /// <inheritdoc/>
    public bool ExecuteHook(
        object? source,
        object target,
        Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
        Func<IObservedChange<object, object>[]> getCurrentViewProperties,
        BindingDirection direction)
    {
        if (ItemsControlTemplateBinding.FindDefaultTemplateTarget(getCurrentViewProperties) is not { } itemsControl)
        {
            return true;
        }

        itemsControl.ItemTemplate = DefaultItemTemplate.Value;
        return true;
    }
}
#endif
