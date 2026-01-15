// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace ReactiveUI.Tests.Wpf.Mocks;

/// <summary>
/// A fake xaml command binding view.
/// </summary>
public class FakeXamlCommandBindingView : Control, IViewFor<CommandBindingViewModel>
{
    /// <summary>
    /// The view model property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(CommandBindingViewModel), typeof(FakeXamlCommandBindingView), new PropertyMetadata(null));

    /// <summary>
    /// The button declared in xaml property.
    /// </summary>
    public static readonly DependencyProperty ButtonDeclaredInXamlProperty =
        DependencyProperty.Register(nameof(ButtonDeclaredInXaml), typeof(Button), typeof(FakeXamlCommandBindingView), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeXamlCommandBindingView"/> class.
    /// </summary>
    public FakeXamlCommandBindingView()
    {
        ButtonDeclaredInXaml = new Button();

        this.BindCommand(ViewModel, static vm => vm!.Command2!, static v => v.ButtonDeclaredInXaml);
    }

    /// <summary>
    /// Gets or sets the button declared in xaml.
    /// </summary>
    public Button ButtonDeclaredInXaml
    {
        get => (Button)GetValue(ButtonDeclaredInXamlProperty);
        set => SetValue(ButtonDeclaredInXamlProperty, value);
    }

    /// <summary>
    /// Gets the name of button declared in xaml.
    /// </summary>
    public string NameOfButtonDeclaredInXaml => nameof(ButtonDeclaredInXaml);

    /// <inheritdoc/>
    public CommandBindingViewModel? ViewModel
    {
        get => (CommandBindingViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (CommandBindingViewModel?)value;
    }
}
