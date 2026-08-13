// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// WPF and WinUI each have their own ItemsControl with the same members; the alias lets the one
// implementation below compile against whichever of the two the consuming assembly targets.
#if WINUI_TARGET
using ItemsControl = Microsoft.UI.Xaml.Controls.ItemsControl;
#else
using ItemsControl = System.Windows.Controls.ItemsControl;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>Locates the items control a binding hook should give a default item template to.</summary>
internal static class ItemsControlTemplateBinding
{
    /// <summary>The view property name a binding must target for the default item template to apply.</summary>
    private const string ItemsSourcePropertyName = "ItemsSource";

    /// <summary>
    /// Returns the items control this binding writes its <c>ItemsSource</c> to, when that control renders its
    /// items through a template it has not been given.
    /// </summary>
    /// <param name="getCurrentViewProperties">Supplies the view-side properties of the binding being set up.</param>
    /// <returns>
    /// The items control to template, or <see langword="null"/> when the binding targets something else, the
    /// control already has an item template or template selector, or it renders items by display member path.
    /// </returns>
    internal static ItemsControl? FindDefaultTemplateTarget(Func<IObservedChange<object, object>[]> getCurrentViewProperties)
    {
        ArgumentExceptionHelper.ThrowIfNull(getCurrentViewProperties);

        var viewProperties = getCurrentViewProperties();
        var lastViewProperty = viewProperties.Length > 0 ? viewProperties[^1] : null;

        if (lastViewProperty?.Sender is not ItemsControl itemsControl)
        {
            return null;
        }

        var wantsDefaultTemplate = string.IsNullOrEmpty(itemsControl.DisplayMemberPath)
            && itemsControl.ItemTemplate is null
            && itemsControl.ItemTemplateSelector is null
            && lastViewProperty.GetPropertyName() == ItemsSourcePropertyName;

        return wantsDefaultTemplate ? itemsControl : null;
    }
}
