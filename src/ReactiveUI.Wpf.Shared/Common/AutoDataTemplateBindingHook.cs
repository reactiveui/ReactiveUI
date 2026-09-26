// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Windows;
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
/// <remarks>
/// The default template hosts each item in a <see cref="ViewModelViewHost"/>, which finds the view through the
/// generated view lookup and the view locator's <c>Map</c> registrations. For view models whose view is registered
/// only with the service locator, also register <see cref="AutoDataTemplateBindingHookUnsafe"/>.
/// </remarks>
public class AutoDataTemplateBindingHook : IPropertyBindingHook
{
    /// <summary>Gets the default item template.</summary>
    public static Lazy<DataTemplate> DefaultItemTemplate { get; } = new(static () => CreateItemTemplate(nameof(ViewModelViewHost)));

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

    /// <summary>Builds an item template that hosts each item in the named host type from this assembly.</summary>
    /// <param name="hostTypeName">The name of the host type, which lives in this type's namespace and assembly.</param>
    /// <returns>The item template.</returns>
    internal static DataTemplate CreateItemTemplate(string hostTypeName)
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
        var assemblyName = typeof(AutoDataTemplateBindingHook).Assembly.FullName;
        assemblyName = assemblyName?.Substring(0, assemblyName.IndexOf(','));

        var template =
            $"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:xaml='{XamlClrNamespace};assembly={assemblyName}'>"
            + $" <xaml:{hostTypeName} ViewModel=\"{{Binding Mode=OneWay}}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" /></DataTemplate>";

        return (DataTemplate)XamlReader.Parse(template);
    }
}
