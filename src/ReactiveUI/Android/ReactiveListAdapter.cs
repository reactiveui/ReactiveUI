using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Android.Views;
using Android.Widget;
using Splat;

namespace ReactiveUI
{
    public class ReactiveListAdapter<TViewModel> : BaseAdapter<TViewModel>, IEnableLogger
        where TViewModel : class
    {
        IReadOnlyReactiveList<TViewModel> list;
        readonly Func<TViewModel, ViewGroup, View> viewCreator;
        readonly Action<TViewModel, View> viewInitializer;

        IDisposable _inner;
		SerialDisposable _backingListUpdatesObservable = new SerialDisposable();

        public ReactiveListAdapter(
            IReadOnlyReactiveList<TViewModel> backingList,
            Func<TViewModel, ViewGroup, View> viewCreator,
            Action<TViewModel, View> viewInitializer = null)
        {
            this.list = backingList;
            this.viewCreator = viewCreator;
            this.viewInitializer = viewInitializer;

            _inner = this.list.Changed.Subscribe(_ => NotifyDataSetChanged());
        }

		public ReactiveListAdapter(
			IObservable<IReadOnlyReactiveList<TViewModel>> backingListObservable,
			Func<TViewModel, ViewGroup, View> viewCreator,
			Action<TViewModel, View> viewInitializer = null)
		{
			this.viewCreator = viewCreator;
			this.viewInitializer = viewInitializer;

			_inner = backingListObservable
				.StartWith(new ReactiveList<TViewModel>())
				.Subscribe(backingList => {
					this.list = backingList;

					_backingListUpdatesObservable.Disposable = this.list.Changed.Subscribe(_ => NotifyDataSetChanged());
				});
		}

        public override TViewModel this[int position] {
            get { return list[position]; }
        }

        public override long GetItemId(int position) {
            return list[position].GetHashCode();
        }

        public override bool HasStableIds {
            get { return true; }
        }

        public override int Count {
            get { return list.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View theView = convertView;
            var data = list[position];

            if (theView == null) {
                theView = viewCreator(data, parent);
            }

            var ivf = theView.GetViewHost() as IViewFor<TViewModel>;
            if (ivf != null) {
                ivf.ViewModel = data;
            }

            if (viewInitializer != null) {
                viewInitializer(data, theView);
            }

            return theView;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
			_backingListUpdatesObservable?.Dispose();
        }
    }
}