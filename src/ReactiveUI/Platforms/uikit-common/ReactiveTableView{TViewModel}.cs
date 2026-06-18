// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using CoreGraphics;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This is a TableView that is both an TableView and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
public abstract class ReactiveTableView<TViewModel> : ReactiveTableView, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.</summary>
    protected ReactiveTableView()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveTableView(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.</summary>
    /// <param name="coder">The pointer.</param>
    protected ReactiveTableView(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveTableView(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.</summary>
    /// <param name="frame">The frmae.</param>
    /// <param name="style">The ui view style.</param>
    protected ReactiveTableView(CGRect frame, UITableViewStyle style)
        : base(frame, style)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableView{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveTableView(in IntPtr handle)
        : base(handle)
    {
    }

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value!;
    }
}
