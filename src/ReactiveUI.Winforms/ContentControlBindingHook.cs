// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Windows.Forms;

namespace ReactiveUI.Winforms;

/// <summary>
/// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
/// that don't have DataTemplates, and assigns a default DataTemplate that
/// loads the View associated with each ViewModel.
/// </summary>
public class ContentControlBindingHook : IPropertyBindingHook
{
    /// <inheritdoc/>
    public bool ExecuteHook(object? source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
    {
        if (getCurrentViewProperties is null)
        {
            throw new ArgumentNullException(nameof(getCurrentViewProperties));
        }

        var viewProperties = getCurrentViewProperties();
        var lastViewProperty = viewProperties.LastOrDefault();

        if (lastViewProperty?.Sender is not Panel)
        {
            return true;
        }

        if (viewProperties.Last().GetPropertyName() != "Controls")
        {
            return true;
        }

        return true;
    }
}