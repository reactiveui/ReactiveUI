// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Winforms;

/// <summary>
/// This is an  UserControl that is both and UserControl and has a ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The type of the view model.</typeparam>
/// <seealso cref="UserControl" />
/// <seealso cref="IViewFor{TViewModel}" />
public partial class ReactiveUserControl<TViewModel> : UserControl, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUserControl{TViewModel}"/> class.
    /// </summary>
    public ReactiveUserControl() => InitializeComponent();

    /// <inheritdoc/>
    [Category("ReactiveUI")]
    [Description("The ViewModel.")]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public TViewModel? ViewModel { get; set; }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
