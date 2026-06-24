// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ReactiveUI.Builder.WpfApp.ViewModels;
using Splat;

namespace ReactiveUI.Builder.WpfApp;

/// <summary>The application shell window; hosts the router through a <see cref="RoutedViewHost"/>.</summary>
public partial class MainWindow : IViewFor<AppBootstrapper>
{
    /// <summary>Identifies the <see cref="ViewModel"/> dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(AppBootstrapper),
        typeof(MainWindow),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
    public MainWindow()
    {
        InitializeComponent();

        var screen = (AppBootstrapper)Locator.Current.GetService<IScreen>()!;
        ViewModel = screen;
        Content = new RoutedViewHost
        {
            Router = screen.Router,
            DefaultContent = new TextBlock
            {
                Text = "Loading…",
                Foreground = Brushes.Gray,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },
        };
    }

    /// <summary>Gets or sets the root screen that backs this shell.</summary>
    public AppBootstrapper? ViewModel
    {
        get => (AppBootstrapper?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (AppBootstrapper?)value;
    }
}
