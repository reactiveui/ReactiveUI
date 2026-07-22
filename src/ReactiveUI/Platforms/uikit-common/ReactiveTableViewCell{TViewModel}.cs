// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using CoreGraphics;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a UITableViewCell that is both an UITableViewCell and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public class ReactiveTableViewCell<TViewModel> : ReactiveTableViewCell, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.</summary>
    /// <param name="frame">The frame.</param>
    protected ReactiveTableViewCell(CGRect frame)
        : base(frame)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveTableViewCell(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveTableViewCell(NSCoder coder)
        : base(NSObjectFlag.Empty)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.</summary>
    protected ReactiveTableViewCell()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.</summary>
    /// <param name="style">The ui table view cell style.</param>
    /// <param name="reuseIdentifier">The reuse identifier.</param>
    protected ReactiveTableViewCell(UITableViewCellStyle style, string reuseIdentifier)
        : base(style, reuseIdentifier)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.</summary>
    /// <param name="style">The ui table view cell style.</param>
    /// <param name="reuseIdentifier">The reuse identifier.</param>
    protected ReactiveTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier)
        : base(style, reuseIdentifier)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTableViewCell{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveTableViewCell(in IntPtr handle)
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
