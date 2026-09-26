// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using ReactiveUI.Winforms;

namespace ReactiveUI.Documentation.PlatformWinforms;

/// <summary>
/// A front-desk notice banner. The WinForms Designer cannot show a control whose base class is generic, so a
/// view meant to be edited there derives from the non-generic <see cref="ReactiveUserControlNonGeneric"/> instead
/// of <see cref="ReactiveUserControl{TViewModel}"/>, and adds its own strongly typed <see cref="ViewModel"/>
/// property over the base's non-generic <see cref="IViewFor.ViewModel"/>.
/// </summary>
[DebuggerDisplay("LibraryNoticeView ViewModel = {ViewModel}")]
public sealed class LibraryNoticeView : ReactiveUserControlNonGeneric
{
    /// <summary>Initializes a new instance of the <see cref="LibraryNoticeView"/> class.</summary>
    public LibraryNoticeView() => Controls.Add(NoticeLabel);

    /// <summary>Gets or sets the notice to show, stored through the base's non-generic <see cref="IViewFor.ViewModel"/>.</summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public AnnouncementViewModel? ViewModel
    {
        get => (AnnouncementViewModel?)((IViewFor)this).ViewModel;
        set
        {
            ((IViewFor)this).ViewModel = value;
            NoticeLabel.Text = value?.Message ?? string.Empty;
        }
    }

    /// <summary>Gets the label that shows the current notice.</summary>
    internal Label NoticeLabel { get; } = new() { AutoSize = true, Name = "NoticeLabel" };

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NoticeLabel.Dispose();
        }

        base.Dispose(disposing);
    }
}
