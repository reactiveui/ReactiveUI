// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// The view for <see cref="StationListPageViewModel"/>. The list has no <c>ItemTemplate</c>, so
/// <see cref="AutoDataTemplateBindingHook"/> assigns one that hosts each <see cref="WeatherReading"/> through the
/// view locator, which resolves <see cref="WeatherReadingRowView"/> for it.
/// </summary>
[System.Diagnostics.DebuggerDisplay("StationListPageView")]
public sealed class StationListPageView : ReactiveUserControl<StationListPageViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="StationListPageView"/> class.</summary>
    public StationListPageView()
    {
        Content = ReadingsList;

        this.WhenActivated(disposables =>
        {
            try
            {
                // AutoDataTemplateBindingHook only fires the first time an ItemsControl with no ItemTemplate is
                // bound; it needs the running app to resolve "using:ReactiveUI" through XamlReader, which requires
                // an IXamlMetadataProvider this sample app does not register (see the platform-winui report).
                IDisposable subscription = this.OneWayBind(ViewModel, viewModel => viewModel.Readings, view => view.ReadingsList.ItemsSource);
                disposables(subscription);
                Console.WriteLine($"AutoDataTemplateBindingHook resolved: {ReadingsList.ItemTemplate is not null}, items: {ReadingsList.Items.Count}");
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                Console.WriteLine($"AutoDataTemplateBindingHook could not resolve its default template: {ex} ");
            }
        });
    }

    /// <summary>Gets the list of station readings.</summary>
    internal ItemsControl ReadingsList { get; } = new();
}
