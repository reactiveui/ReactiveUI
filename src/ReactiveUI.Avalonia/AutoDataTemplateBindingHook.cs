// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace ReactiveUI.Avalonia;

/// <summary>
/// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
/// that don't have DataTemplates, and assigns a default DataTemplate that
/// loads the View associated with each ViewModel.
/// </summary>
public class AutoDataTemplateBindingHook : IPropertyBindingHook
{
    private static readonly FuncDataTemplate DefaultItemTemplate = new FuncDataTemplate<object>(
     (_, _) =>
     {
         var control = new ViewModelViewHost();
         var context = control.GetObservable(StyledElement.DataContextProperty);
         control.Bind(ViewModelViewHost.ViewModelProperty, context);
         control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
         control.VerticalContentAlignment = VerticalAlignment.Stretch;
         return control;
     },
     true);

    /// <inheritdoc/>
    public bool ExecuteHook(
        object? source,
        object target,
        Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
        Func<IObservedChange<object, object>[]> getCurrentViewProperties,
        BindingDirection direction)
    {
        if (getCurrentViewProperties is null)
        {
            throw new ArgumentNullException(nameof(getCurrentViewProperties));
        }

        var viewProperties = getCurrentViewProperties();
        var lastViewProperty = viewProperties.LastOrDefault();
        var itemsControl = lastViewProperty?.Sender as ItemsControl;
        if (itemsControl == null)
        {
            return true;
        }

        var propertyName = viewProperties.Last().GetPropertyName();
        if (propertyName != "Items" &&
            propertyName != "ItemsSource")
        {
            return true;
        }

        if (itemsControl.ItemTemplate != null)
        {
            return true;
        }

        if (itemsControl.DataTemplates != null &&
            itemsControl.DataTemplates.Count > 0)
        {
            return true;
        }

        itemsControl.ItemTemplate = DefaultItemTemplate;
        return true;
    }
}