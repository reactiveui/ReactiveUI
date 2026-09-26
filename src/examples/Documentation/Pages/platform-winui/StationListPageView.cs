// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// The view for <see cref="StationListPageViewModel"/>. The list has no <c>ItemTemplate</c>, so
/// <see cref="AutoDataTemplateBindingHook"/> assigns one that hosts each <see cref="WeatherReading"/> in a
/// <see cref="ViewModelViewHost"/>, which finds <see cref="WeatherReadingRowView"/> through the generated view lookup.
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
            // ReadingsList has no ItemTemplate, so AutoDataTemplateBindingHook gives it the default one as this binds.
            IDisposable subscription = this.OneWayBind(ViewModel, viewModel => viewModel.Readings, view => view.ReadingsList.ItemsSource);
            disposables(subscription);

            bool usesDefaultTemplate = ReferenceEquals(ReadingsList.ItemTemplate, AutoDataTemplateBindingHook.DefaultItemTemplate.Value);
            Console.WriteLine($"Default item template assigned: {usesDefaultTemplate}, items: {ReadingsList.Items.Count}");
        });
    }

    /// <summary>Gets the list of station readings.</summary>
    internal ItemsControl ReadingsList { get; } = new();
}
