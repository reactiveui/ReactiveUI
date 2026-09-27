// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Documentation.Controls;

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// A library desk's book-return dialog, built on <see cref="ReactiveVendorDialog{TViewModel}"/> rather than on a
/// ReactiveUI base, because <see cref="VendorDialog"/> is the base the (pretend) dialog library requires.
/// </summary>
[System.Diagnostics.DebuggerDisplay("BookReturnDialog ViewModel = {ViewModel}")]
public sealed class BookReturnDialog : ReactiveVendorDialog<BookReturnViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="BookReturnDialog"/> class.</summary>
    public BookReturnDialog() =>
        this.WhenActivated(
            disposables =>
            {
                disposables(this.OneWayBind(ViewModel, x => x.BookTitle, v => v.TitleLabel.Text));
                disposables(this.Bind(ViewModel, x => x.IsDamaged, v => v.DamagedCheckBox.IsChecked));
            },
            this.WhenAnyValue(x => x.ViewModel));

    /// <summary>Gets the label that shows the title of the book being returned.</summary>
    public Label TitleLabel { get; } = new();

    /// <summary>Gets the check box the librarian ticks when the book comes back damaged.</summary>
    public CheckBox DamagedCheckBox { get; } = new();
}
