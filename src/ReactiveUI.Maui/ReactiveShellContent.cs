// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Maui.Controls;
using Splat;

namespace ReactiveUI.Maui;

/// <summary>
/// ReactiveShellContent.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="Microsoft.Maui.Controls.ShellContent" />
/// <seealso cref="ReactiveUI.IActivatableView" />
public class ReactiveShellContent<TViewModel> : ShellContent, IActivatableView
    where TViewModel : class
{
    /// <summary>
    /// The contract property.
    /// </summary>
    public static readonly BindableProperty ContractProperty = BindableProperty.Create(
     nameof(Contract),
     typeof(string),
     typeof(ReactiveShellContent<TViewModel>),
     null,
     BindingMode.Default,
     propertyChanged: ViewModelChanged);

    /// <summary>
    /// The view model property.
    /// </summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
     nameof(ViewModel),
     typeof(TViewModel),
     typeof(ReactiveShellContent<TViewModel>),
     default(TViewModel),
     BindingMode.Default,
     propertyChanged: ViewModelChanged);

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveShellContent{TViewModel}" /> class.
    /// </summary>
    public ReactiveShellContent()
    {
        var view = Locator.Current.GetService<IViewFor<TViewModel>>(Contract);
        if (view is not null)
        {
            ContentTemplate = new DataTemplate(() => view);
        }
    }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    /// <value>
    /// The view model.
    /// </value>
    public TViewModel? ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the contract for the view.
    /// </summary>
    /// <value>
    /// The contract.
    /// </value>
    public string? Contract
    {
        get => (string?)GetValue(ContractProperty);
        set => SetValue(ContractProperty, value);
    }

    private static void ViewModelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (Locator.Current is null)
        {
            throw new NullReferenceException(nameof(Locator.Current));
        }

        if (bindable is ReactiveShellContent<TViewModel> svm)
        {
            var view = Locator.Current.GetService<IViewFor<TViewModel>>(svm.Contract);
            if (view is not null)
            {
                svm.ContentTemplate = new DataTemplate(() => view);
            }
        }
    }
}
