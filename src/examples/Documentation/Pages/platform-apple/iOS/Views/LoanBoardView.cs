// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CoreGraphics;
using UIKit;

namespace ReactiveUI.Documentation.PlatformApple;

/// <summary>A plain <see cref="ReactiveTableView{TViewModel}"/> hosted directly as a subview, listing books currently on loan.</summary>
[System.Diagnostics.DebuggerDisplay("LoanBoardView")]
public sealed class LoanBoardView : ReactiveTableView<BookCatalogViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="LoanBoardView"/> class.</summary>
    /// <param name="frame">The board's initial frame.</param>
    public LoanBoardView(CGRect frame)
        : base(frame, UITableViewStyle.Plain)
    {
        _ = this.WhenActivated(d =>
        {
            ObservableCollection<Book> onLoan = new(ViewModel!.Books.Where(static book => book.IsOnLoan));
            d(Signal.Emit<INotifyCollectionChanged>(onLoan).BindTo<Book, LoanedBookCell>(this, sizeHint: 44F));
        });
    }
}
