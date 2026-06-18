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
/// <summary>
/// This is a UICollectionView that is both an UICollectionView and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
public abstract class ReactiveCollectionView<TViewModel> : ReactiveCollectionView, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionView{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveCollectionView(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionView{TViewModel}"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveCollectionView(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionView{TViewModel}"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveCollectionView(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionView{TViewModel}"/> class.</summary>
    /// <param name="frame">The frame.</param>
    /// <param name="layout">The ui collection view layout.</param>
    protected ReactiveCollectionView(CGRect frame, UICollectionViewLayout layout)
        : base(frame, layout)
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
