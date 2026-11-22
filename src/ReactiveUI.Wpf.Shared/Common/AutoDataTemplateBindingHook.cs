// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

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
/// only with the service locator, also register <c>AutoDataTemplateBindingHookUnsafe</c> from <c>ReactiveUI.WPF.Unsafe</c>.
/// </remarks>
public class AutoDataTemplateBindingHook : IPropertyBindingHook
{
    /// <summary>Gets the default item template.</summary>
    public static Lazy<DataTemplate> DefaultItemTemplate { get; } =
        new(static () => ItemsControlTemplateBinding.CreateWpfItemTemplate(nameof(ViewModelViewHost), typeof(AutoDataTemplateBindingHook).Assembly));

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
