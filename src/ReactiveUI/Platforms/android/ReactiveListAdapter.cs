// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using Android.Views;
using Android.Widget;
using Splat;

namespace ReactiveUI.Legacy
{
    [Obsolete("ReactiveList is no longer supported. We suggest replacing it with DynamicData https://github.com/rolandpheasant/dynamicdata")]
#pragma warning disable SA1600 // Elements should be documented
    public class ReactiveListAdapter<TViewModel> : BaseAdapter<TViewModel>, IEnableLogger
        where TViewModel : class
    {
        private readonly IReadOnlyReactiveList<TViewModel> _list;
        private readonly Func<TViewModel, ViewGroup, View> _viewCreator;
        private readonly Action<TViewModel, View> _viewInitializer;
        private IDisposable _inner;

        public ReactiveListAdapter(
            IReadOnlyReactiveList<TViewModel> backingList,
            Func<TViewModel, ViewGroup, View> viewCreator,
            Action<TViewModel, View> viewInitializer = null)
        {
            _list = backingList;
            _viewCreator = viewCreator;
            _viewInitializer = viewInitializer;

            _inner = _list.Changed.Subscribe(_ => NotifyDataSetChanged());
        }

        /// <inheritdoc/>
        public override bool HasStableIds
        {
            get { return true; }
        }

        /// <inheritdoc/>
        public override int Count
        {
            get { return _list.Count; }
        }

        /// <inheritdoc/>
        public override TViewModel this[int index]
        {
            get { return _list[index]; }
        }

        /// <inheritdoc/>
        public override long GetItemId(int position)
        {
            return _list[position].GetHashCode();
        }

        /// <inheritdoc/>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View theView = convertView;
            var data = _list[position];

            if (theView == null)
            {
                theView = _viewCreator(data, parent);
            }

            var ivf = theView.GetViewHost() as IViewFor<TViewModel>;
            if (ivf != null)
            {
                ivf.ViewModel = data;
            }

            if (_viewInitializer != null)
            {
                _viewInitializer(data, theView);
            }

            return theView;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
