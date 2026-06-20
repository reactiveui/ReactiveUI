// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Foundation;
using UIKit;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// This is a UIPageViewController that is both an UIPageViewController and has ReactiveObject powers
/// (i.e. you can call RaiseAndSetIfChanged).
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
public abstract class ReactivePageViewController<TViewModel> : ReactivePageViewController, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="style">The view controller transition style.</param>
    /// <param name="orientation">The view controller navigation orientation.</param>
    protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation)
        : base(style, orientation)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="style">The view controller transition style.</param>
    /// <param name="orientation">The view controller navigation orientation.</param>
    /// <param name="options">The options.</param>
    protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, NSDictionary options)
        : base(style, orientation, options)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="style">The view controller transition style.</param>
    /// <param name="orientation">The view controller navigation orientation.</param>
    /// <param name="spineLocation">The view controller spine location.</param>
    protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation)
        : base(style, orientation, spineLocation)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="style">The view controller transition style.</param>
    /// <param name="orientation">The view controller navigation orientation.</param>
    /// <param name="spineLocation">The view controller spine location.</param>
    /// <param name="interPageSpacing">The spacing between pages.</param>
    protected ReactivePageViewController(
        UIPageViewControllerTransitionStyle style,
        UIPageViewControllerNavigationOrientation orientation,
        UIPageViewControllerSpineLocation spineLocation,
        float interPageSpacing)
        : base(style, orientation, spineLocation, interPageSpacing)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="nibName">The name.</param>
    /// <param name="bundle">The bundle.</param>
    protected ReactivePageViewController(string nibName, NSBundle bundle)
        : base(nibName, bundle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="handle">The pointer.</param>
    protected ReactivePageViewController(in IntPtr handle)
        : base(handle)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="t">The object flag.</param>
    protected ReactivePageViewController(NSObjectFlag t)
        : base(t)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    /// <param name="coder">The coder.</param>
    protected ReactivePageViewController(NSCoder coder)
        : base(coder)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactivePageViewController{TViewModel}"/> class.</summary>
    protected ReactivePageViewController()
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
