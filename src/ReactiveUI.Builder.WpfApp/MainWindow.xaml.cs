// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using Splat;

namespace ReactiveUI.Builder.WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window, IViewFor<ViewModels.AppBootstrapper>
{
    /// <summary>
    /// The view model property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(ViewModels.AppBootstrapper), typeof(MainWindow), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        // Set up content host with routing
        var host = new RoutedViewHost
        {
            Router = Locator.Current.GetService<IScreen>()!.Router,
            DefaultContent = new System.Windows.Controls.TextBlock { Text = "Loading...", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center },
        };

        Content = host;
        ViewModel = (ViewModels.AppBootstrapper)Locator.Current.GetService<IScreen>()!;
    }

    /// <summary>
    /// Gets or sets the ViewModel corresponding to this specific View. This should be
    /// a DependencyProperty if you're using XAML.
    /// </summary>
    public ViewModels.AppBootstrapper? ViewModel
    {
        get => (ViewModels.AppBootstrapper?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the ViewModel corresponding to this specific View. This should be
    /// a DependencyProperty if you're using XAML.
    /// </summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ViewModels.AppBootstrapper?)value;
    }
}
