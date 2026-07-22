// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This is a TabBar that is both an TabBar and has ReactiveObject powers (i.e. you can call RaiseAndSetIfChanged).</summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public class ReactiveTabBarController<TViewModel> : ReactiveTabBarController, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveTabBarController{TViewModel}"/> class.</summary>
    /// <param name="nibName">The name.</param>
    /// <param name="bundle">The bundle.</param>
    protected ReactiveTabBarController(string nibName, NSBundle bundle)
        : base(nibName, bundle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTabBarController{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactiveTabBarController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTabBarController{TViewModel}"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactiveTabBarController(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTabBarController{TViewModel}"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactiveTabBarController(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveTabBarController{TViewModel}"/> class.</summary>
    protected ReactiveTabBarController()
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
