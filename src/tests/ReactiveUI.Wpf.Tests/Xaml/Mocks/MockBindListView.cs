// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Windows;
using System.Windows.Controls;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>
/// MockBindListView.
/// </summary>
/// <seealso cref="UserControl" />
public class MockBindListView : UserControl, IViewFor<MockBindListViewModel>
{
    /// <summary>
    /// Identifies the <see cref="ViewModel"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(MockBindListViewModel), typeof(MockBindListView), new(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="MockBindListView"/> class.
    /// </summary>
    public MockBindListView()
    {
        ItemList = new();
        ViewModel = new();

        this.WhenActivated(d => this.OneWayBind(ViewModel, vm => vm.ListItems, v => v.ItemList.ItemsSource).DisposeWith(d));
    }

    /// <summary>
    /// Gets or sets the ViewModel corresponding to this specific View. This should be
    /// a DependencyProperty if you're using XAML.
    /// </summary>
    public MockBindListViewModel? ViewModel
    {
        get => (MockBindListViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets the list control used to display the items.
    /// </summary>
    public ListView ItemList { get; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MockBindListViewModel?)value;
    }
}
