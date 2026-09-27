// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// Bridges <see cref="VendorDialog"/>, a base class from a library this app does not control, to ReactiveUI by
/// implementing <see cref="IViewFor{TViewModel}"/>. This is the pattern for any base class an app must derive from
/// that ReactiveUI does not already provide: a third-party dialog, popup or window base. Keeping
/// <see cref="ViewModel"/> and <see cref="VendorDialog.DialogContext"/> in step, in both directions, and raising
/// <see cref="INotifyPropertyChanged.PropertyChanged"/> for <see cref="ViewModel"/> is what lets
/// <c>this.WhenAnyValue(x =&gt; x.ViewModel)</c> and the binding methods work on a dialog built this way.
/// </summary>
/// <typeparam name="TViewModel">The type of view model the dialog displays.</typeparam>
[System.Diagnostics.DebuggerDisplay("ReactiveVendorDialog ViewModel = {ViewModel}")]
public class ReactiveVendorDialog<TViewModel> : VendorDialog, IViewFor<TViewModel>, INotifyPropertyChanged
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveVendorDialog{TViewModel}"/> class.</summary>
    protected ReactiveVendorDialog() => DialogContextChanged += (_, _) => ViewModel = DialogContext as TViewModel;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets or sets the view model the dialog displays.</summary>
    public TViewModel? ViewModel
    {
        get;
        set
        {
            if (ReferenceEquals(field, value))
            {
                return;
            }

            field = value;
            DialogContext = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewModel)));
        }
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }
}
