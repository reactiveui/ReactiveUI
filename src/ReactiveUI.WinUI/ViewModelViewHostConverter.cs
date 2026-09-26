// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Turns an item's view model into the host that shows its view, for a default item template.</summary>
/// <param name="resourceKey">The key the converter is stored under in the application's resources.</param>
/// <param name="createHost">Creates the host for one item.</param>
/// <remarks>
/// WinUI can only build a <see cref="DataTemplate"/> from XAML, and XAML can only name a ReactiveUI type when the
/// application describes it through an <c>IXamlMetadataProvider</c>. The template this class builds names framework
/// types only: a <c>ContentControl</c> whose content is the item bound through this converter. The converter creates
/// the host in code, so no XAML type lookup of a ReactiveUI type is needed.
/// </remarks>
[DebuggerDisplay("{_resourceKey}")]
internal sealed partial class ViewModelViewHostConverter(string resourceKey, Func<ViewModelViewHost> createHost) : IValueConverter
{
    /// <summary>The key the converter is stored under in the application's resources.</summary>
    private readonly string _resourceKey = resourceKey;

    /// <summary>Gets the converter that hosts each item in a <see cref="ViewModelViewHost"/>.</summary>
    internal static ViewModelViewHostConverter Generated { get; } =
        new($"{typeof(ViewModelViewHost).FullName}Converter", static () => new ViewModelViewHost());

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var host = createHost();
        host.ViewModel = value;
        host.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        host.VerticalContentAlignment = VerticalAlignment.Stretch;
        host.IsTabStop = false;
        return host;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException($"{nameof(ViewModelViewHostConverter)} only converts a view model to its host.");

    /// <summary>Adds the converter to the current application's resources, where a loaded template finds it.</summary>
    internal void EnsureRegistered()
    {
        if (Application.Current?.Resources is not { } resources || resources.ContainsKey(_resourceKey))
        {
            return;
        }

        resources[_resourceKey] = this;
    }

    /// <summary>Builds an item template that hosts each item through this converter.</summary>
    /// <returns>The item template.</returns>
    internal DataTemplate CreateItemTemplate()
    {
        EnsureRegistered();

        var template = "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"
            + $"<ContentControl Content=\"{{Binding Converter={{StaticResource {_resourceKey}}}}}\" "
            + "VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />"
            + "</DataTemplate>";

        return (DataTemplate)XamlReader.Load(template);
    }
}
