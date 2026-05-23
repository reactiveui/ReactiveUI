// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;
using AndroidX.ViewPager.Widget;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using Splat;
using Object = Java.Lang.Object;

namespace ReactiveUI.AndroidX;

/// <summary>
/// ReactivePagerAdapter is a PagerAdapter that will interface with a
/// Observable change set, in a similar fashion to ReactiveTableViewSource.
/// </summary>
/// <typeparam name="TViewModel">The view model type.</typeparam>
public class ReactivePagerAdapter<TViewModel> : PagerAdapter, IEnableLogger
    where TViewModel : class
{
    /// <summary>The materialized change-set binding that drives the adapter.</summary>
    private readonly ChangeSetBinder<TViewModel> _binder;

    /// <summary>Creates the view for a given view model.</summary>
    private readonly Func<TViewModel, ViewGroup, View> _viewCreator;

    /// <summary>Optional initializer invoked for each created view.</summary>
    private readonly Action<TViewModel, View>? _viewInitializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePagerAdapter{TViewModel}"/> class.
    /// </summary>
    /// <param name="changeSet">The change set to page.</param>
    /// <param name="viewCreator">A function which will create the view.</param>
    public ReactivePagerAdapter(
        IObservable<IReactiveChangeSet<TViewModel>> changeSet,
        Func<TViewModel, ViewGroup, View> viewCreator)
        : this(changeSet, viewCreator, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactivePagerAdapter{TViewModel}"/> class.
    /// </summary>
    /// <param name="changeSet">The change set to page.</param>
    /// <param name="viewCreator">A function which will create the view.</param>
    /// <param name="viewInitializer">A action which will initialize a view.</param>
    public ReactivePagerAdapter(
        IObservable<IReactiveChangeSet<TViewModel>> changeSet,
        Func<TViewModel, ViewGroup, View> viewCreator,
        Action<TViewModel, View>? viewInitializer)
    {
        _viewCreator = viewCreator;
        _viewInitializer = viewInitializer;
        _binder = new(changeSet, onBatch: NotifyDataSetChanged);
    }

    /// <inheritdoc/>
    public override int Count => _binder.Count;

    /// <inheritdoc/>
    public override bool IsViewFromObject(View view, Object @object) => view.Equals(@object);

    /// <inheritdoc/>
    public override Object InstantiateItem(ViewGroup container, int position)
    {
        ArgumentExceptionHelper.ThrowIfNull(container);

        var data = _binder[position];

        // NB: PagerAdapter does not recycle itself.
        var theView = _viewCreator(data, container);

        if (theView.GetViewHost() is IViewFor<TViewModel> ivf)
        {
            ivf.ViewModel = data;
        }

        _viewInitializer?.Invoke(data, theView);

        container.AddView(theView, 0);
        return theView;
    }

    /// <inheritdoc/>
    public override void DestroyItem(ViewGroup container, int position, Object @object)
    {
        ArgumentExceptionHelper.ThrowIfNull(container);

        ArgumentExceptionHelper.ThrowIfNull(@object);

        if (@object is not View view)
        {
            throw new ArgumentException("Item must be of type View", nameof(@object));
        }

        container.RemoveView(view);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _binder.Dispose();
        }

        base.Dispose(disposing);
    }
}
