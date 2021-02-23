// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#if NETFX_CORE || HAS_UNO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
#endif

#if HAS_UNO
namespace ReactiveUI.Uno
#else
namespace ReactiveUI
#endif
{
    /// <summary>
    /// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
    /// that don't have DataTemplates, and assigns a default DataTemplate that
    /// loads the View associated with each ViewModel.
    /// </summary>
    public class AutoDataTemplateBindingHook : IPropertyBindingHook
    {
        /// <summary>
        /// Gets the default item template.
        /// </summary>
        [SuppressMessage("Design", "CA1307: Use the currency locale settings", Justification = "Not available on all platforms.")]
        public static Lazy<DataTemplate> DefaultItemTemplate { get; } = new(() =>
        {
#if NETFX_CORE || HAS_UNO
            const string template =
@"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:xaml='using:ReactiveUI'>
    <xaml:ViewModelViewHost ViewModel=""{Binding}"" VerticalContentAlignment=""Stretch"" HorizontalContentAlignment=""Stretch"" IsTabStop=""False"" />
</DataTemplate>";
            return (DataTemplate)XamlReader.Load(template);
#else
            const string template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                     "xmlns:xaml='clr-namespace:ReactiveUI;assembly=__ASSEMBLYNAME__'> " +
                 "<xaml:ViewModelViewHost ViewModel=\"{Binding Mode=OneWay}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
             "</DataTemplate>";

            string? assemblyName = typeof(AutoDataTemplateBindingHook).Assembly.FullName;
            assemblyName = assemblyName?.Substring(0, assemblyName.IndexOf(','));

            return (DataTemplate)XamlReader.Parse(template.Replace("__ASSEMBLYNAME__", assemblyName));
#endif
        });

        /// <inheritdoc/>
        public bool ExecuteHook(object? source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
        {
            if (getCurrentViewProperties == null)
            {
                throw new ArgumentNullException(nameof(getCurrentViewProperties));
            }

            var viewProperties = getCurrentViewProperties();
            var lastViewProperty = viewProperties.LastOrDefault();

            if (!(lastViewProperty?.Sender is ItemsControl itemsControl))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(itemsControl.DisplayMemberPath))
            {
                return true;
            }

            if (viewProperties.Last().GetPropertyName() != "ItemsSource")
            {
                return true;
            }

            if (itemsControl.ItemTemplate != null)
            {
                return true;
            }

            if (itemsControl.ItemTemplateSelector != null)
            {
                return true;
            }

            itemsControl.ItemTemplate = DefaultItemTemplate.Value;
            return true;
        }
    }
}
