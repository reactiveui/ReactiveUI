// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

#if UIKIT
using NSSplitViewController = UIKit.UISplitViewController;
#else
using AppKit;
#endif

namespace ReactiveUI;

/// <summary>
/// This is a View that is both a NSSplitViewController and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public abstract class ReactiveSplitViewController<TViewModel> : ReactiveSplitViewController, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>The backing field for the <see cref="ViewModel"/> property.</summary>
    private TViewModel? _viewModel;

#if UIKIT
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveSplitViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="nibName">The name.</param>
    /// <param name="bundle">The bundle.</param>
    protected ReactiveSplitViewController(string nibName, NSBundle bundle)
        : base(nibName, bundle)
    {
    }

#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveSplitViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveSplitViewController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveSplitViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveSplitViewController(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveSplitViewController{TViewModel}"/> class.
    /// </summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveSplitViewController(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveSplitViewController{TViewModel}"/> class.
    /// </summary>
    protected ReactiveSplitViewController()
    {
    }

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value!;
    }
}
