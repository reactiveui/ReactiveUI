// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Winforms;
#else
namespace ReactiveUI.Winforms;
#endif

/// <summary>
/// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
/// that don't have DataTemplates, and assigns a default DataTemplate that
/// loads the View associated with each ViewModel.
/// </summary>
public class ContentControlBindingHook : IPropertyBindingHook
{
    /// <inheritdoc/>
    public bool ExecuteHook(
        object? source,
        object target,
        Func<IObservedChange<object, object>[]> getCurrentViewModelProperties,
        Func<IObservedChange<object, object>[]> getCurrentViewProperties,
        BindingDirection direction)
    {
        ArgumentExceptionHelper.ThrowIfNull(getCurrentViewProperties);

        // Always allow the binding to proceed: hosting a view inside a ContentControl/Panel is handled by the
        // routed/view-model view host, not by aborting the property binding here. (Matches the shipped behaviour.)
        return true;
    }
}
