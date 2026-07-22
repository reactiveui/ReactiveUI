// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a UICollectionViewController that is both an UICollectionViewController and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public class ReactiveCollectionViewController<TViewModel> : ReactiveCollectionViewController, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.</summary>
    /// <param name="withLayout">The ui collection view layout.</param>
    protected ReactiveCollectionViewController(UICollectionViewLayout withLayout)
        : base(withLayout)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.</summary>
    /// <param name="nibName">The name.</param>
    /// <param name="bundle">The bundle.</param>
    protected ReactiveCollectionViewController(string nibName, NSBundle bundle)
        : base(nibName, bundle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveCollectionViewController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveCollectionViewController(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveCollectionViewController(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveCollectionViewController{TViewModel}"/> class.</summary>
    protected ReactiveCollectionViewController()
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
