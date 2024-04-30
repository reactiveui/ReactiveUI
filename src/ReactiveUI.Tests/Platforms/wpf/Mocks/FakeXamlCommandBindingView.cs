// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// A fake xaml command binding view.
/// </summary>
public class FakeXamlCommandBindingView : IViewFor<CommandBindingViewModel>
{
    private readonly Button _buttonDeclaredInXaml;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeXamlCommandBindingView"/> class.
    /// </summary>
    public FakeXamlCommandBindingView()
    {
        _buttonDeclaredInXaml = new Button();

        this.BindCommand(ViewModel, vm => vm!.Command2!, v => v._buttonDeclaredInXaml);
    }

    /// <summary>
    /// Gets the name of button declared in xaml.
    /// </summary>
    public string NameOfButtonDeclaredInXaml => nameof(_buttonDeclaredInXaml);

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (CommandBindingViewModel?)value;
    }

    /// <inheritdoc/>
    public CommandBindingViewModel? ViewModel { get; set; }
}
