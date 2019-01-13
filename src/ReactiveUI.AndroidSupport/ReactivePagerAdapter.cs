// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Android.Support.V4.View;
using Android.Views;
using DynamicData;
using DynamicData.Binding;
using Splat;
using Object = Java.Lang.Object;

namespace ReactiveUI.AndroidSupport
{
    /// <summary>
    /// ReactivePagerAdapter is a PagerAdapter that will interface with a
    /// Observable change set, in a similar fashion to ReactiveTableViewSource.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    public class ReactivePagerAdapter<TViewModel> : PagerAdapter, IEnableLogger
        where TViewModel : class
    {
        private readonly SourceList<TViewModel> _list;
        private readonly Func<TViewModel, ViewGroup, View> _viewCreator;
        private readonly Action<TViewModel, View> _viewInitializer;
        private IDisposable _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePagerAdapter{TViewModel}"/> class.
        /// </summary>
        /// <param name="changeSet">The change set to page.</param>
        /// <param name="viewCreator">A function which will create the view.</param>
        /// <param name="viewInitializer">A action which will initialize a view.</param>
        public ReactivePagerAdapter(
            IObservable<IChangeSet<TViewModel>> changeSet,
            Func<TViewModel, ViewGroup, View> viewCreator,
            Action<TViewModel, View> viewInitializer = null)
        {
            _list = new SourceList<TViewModel>(changeSet);
            _viewCreator = viewCreator;
            _viewInitializer = viewInitializer;

            _inner = _list.Connect().Subscribe(_ => NotifyDataSetChanged());
        }

        /// <inheritdoc/>
        public override int Count => _list.Count;

        /// <inheritdoc/>
        public override bool IsViewFromObject(View view, Object @object)
        {
            return (View)@object == view;
        }

        /// <inheritdoc/>
        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var data = _list.Items.ElementAt(position);

            // NB: PagerAdapter does not recycle itself.
            var theView = _viewCreator(data, container);

            var ivf = theView.GetViewHost() as IViewFor<TViewModel>;
            if (ivf != null)
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
            var view = (View)@object;
            container.RemoveView(view);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
        }
    }

    /// <summary>
    /// ReactivePagerAdapter is a PagerAdapter that will interface with a
    /// Observable change set, in a similar fashion to ReactiveTableViewSource.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TCollection">The type of collection.</typeparam>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    public class ReactivePagerAdapter<TViewModel, TCollection> : ReactivePagerAdapter<TViewModel>
        where TViewModel : class
        where TCollection : INotifyCollectionChanged, IEnumerable<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePagerAdapter{TViewModel, TCollection}"/> class.
        /// </summary>
        /// <param name="collection">The collection to page.</param>
        /// <param name="viewCreator">The function which will create the view.</param>
        /// <param name="viewInitializer">A action which will initialize the view.</param>
        public ReactivePagerAdapter(
            TCollection collection,
            Func<TViewModel, ViewGroup, View> viewCreator,
            Action<TViewModel, View> viewInitializer = null)
            : base(collection.ToObservableChangeSet<TCollection, TViewModel>(), viewCreator, viewInitializer)
        {
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
