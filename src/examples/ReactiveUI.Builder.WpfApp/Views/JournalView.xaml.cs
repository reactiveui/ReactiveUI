// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.Builder.WpfApp.ViewModels;

namespace ReactiveUI.Builder.WpfApp.Views;

/// <summary>The transaction journal view.</summary>
public partial class JournalView : IViewFor<JournalViewModel>
{
    /// <summary>Identifies the <see cref="ViewModel"/> dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(JournalViewModel),
        typeof(JournalView),
        new(null));

    /// <summary>Initializes a new instance of the <see cref="JournalView"/> class.</summary>
    public JournalView()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>Gets or sets the view model that backs this view.</summary>
    public JournalViewModel? ViewModel
    {
        get => (JournalViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (JournalViewModel?)value;
    }
}
