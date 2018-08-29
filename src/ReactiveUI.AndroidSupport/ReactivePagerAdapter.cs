// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public class ReactivePagerAdapter<TViewModel> : PagerAdapter, IEnableLogger
        where TViewModel : class
    {
        private readonly SourceList<TViewModel> list;
        private readonly Func<TViewModel, ViewGroup, View> viewCreator;
        private readonly Action<TViewModel, View> viewInitializer;
        IDisposable inner;

        public ReactivePagerAdapter(IObservable<IChangeSet<TViewModel>> changeSet,
                                    Func<TViewModel, ViewGroup, View> viewCreator,
                                    Action<TViewModel, View> viewInitializer = null)
        {
            this.list = new SourceList<TViewModel>(changeSet);
            this.viewCreator = viewCreator;
            this.viewInitializer = viewInitializer;

            inner = this.list.Connect().Subscribe(_ => NotifyDataSetChanged());
        }

        public override bool IsViewFromObject(View view, Object @object)
        {
            return ((View)@object) == view;
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var data = list.Items.ElementAt(position);

            // NB: PagerAdapter does not recycle itself.
            var theView = viewCreator(data, container);

            if (theView.GetViewHost() is IViewFor<TViewModel> ivf) {
                ivf.ViewModel = data;
            }

            viewInitializer?.Invoke(data, theView);

            container.AddView(theView, 0);
            return theView;
        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            var view = (View)@object;
            container.RemoveView(view);
        }

        public override int Count => list.Count;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref inner, Disposable.Empty).Dispose();
        }
    }

    public class ReactivePagerAdapter<TViewModel, TCollection> : ReactivePagerAdapter<TViewModel>
        where TViewModel : class
        where TCollection : INotifyCollectionChanged, IEnumerable<TViewModel>
    {
        public ReactivePagerAdapter(TCollection collection,
                                    Func<TViewModel, ViewGroup, View> viewCreator,
                                    Action<TViewModel, View> viewInitializer = null)
            : base(collection.ToObservableChangeSet<TCollection, TViewModel>(), viewCreator, viewInitializer)
        {
        }
    }
}
