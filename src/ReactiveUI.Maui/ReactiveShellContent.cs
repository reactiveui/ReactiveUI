// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>Represents reactive shell content for a view model.</summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="ShellContent" />
/// <seealso cref="IActivatableView" />
public class ReactiveShellContent<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes
        .PublicParameterlessConstructor)]
TViewModel> : ShellContent, IActivatableView
    where TViewModel : class
{
    /// <summary>The contract property.</summary>
    public static readonly BindableProperty ContractProperty = BindableProperty.Create(
        nameof(Contract),
        typeof(string),
        typeof(ReactiveShellContent<TViewModel>),
        defaultBindingMode: BindingMode.Default,
        propertyChanged: ViewModelChanged);

    /// <summary>The view model property.</summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
        nameof(ViewModel),
        typeof(TViewModel),
        typeof(ReactiveShellContent<TViewModel>),
        defaultBindingMode: BindingMode.Default,
        propertyChanged: ViewModelChanged);

    /// <summary>Initializes a new instance of the <see cref="ReactiveShellContent{TViewModel}" /> class.</summary>
    public ReactiveShellContent()
    {
        UpdateContentTemplate();
    }

    /// <summary>Gets or sets the view model.</summary>
    /// <value>
    /// The view model.
    /// </value>
    public TViewModel? ViewModel
    {
        get => (TViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>Gets or sets the contract for the view.</summary>
    /// <value>
    /// The contract.
    /// </value>
    public string? Contract
    {
        get => (string?)GetValue(ContractProperty);
        set => SetValue(ContractProperty, value);
    }

    /// <summary>Handles changes to the <see cref="ViewModel"/> or <see cref="Contract"/> properties by resolving and assigning the associated view as the content template.</summary>
    /// <param name="bindable">The bindable object whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void ViewModelChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not ReactiveShellContent<TViewModel> svm)
        {
            return;
        }

        svm.UpdateContentTemplate();
    }

    /// <summary>Rebuilds the content template when the view model or contract changes.</summary>
    private void UpdateContentTemplate()
    {
        if (AppLocator.Current is null)
        {
            throw new InvalidOperationException(nameof(AppLocator.Current));
        }

        var view = AppLocator.Current.GetService<IViewFor<TViewModel>>(Contract);
        if (view is null)
        {
            return;
        }

        ContentTemplate = new(() => view);
    }
}
