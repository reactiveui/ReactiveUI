using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace ReactiveUI.Android
{
    public class ReactiveListAdapter<TViewModel, TView> : BaseAdapter<TViewModel>
        where TView : View
        where TViewModel : class
    {
        readonly IReadOnlyReactiveList<TViewModel> list;
        readonly Action<TViewModel, TView> viewInitializer;
        readonly Func<Context, TViewModel, TView> viewCreator;
        readonly Context ctx;

        IDisposable _inner;

        public ReactiveListAdapter(Context ctx, IReadOnlyReactiveList<TViewModel> backingList, Func<Context, TViewModel, TView> viewCreator, Action<TViewModel, TView> viewInitializer)
        {
            this.ctx = ctx;
            this.list = backingList;
            this.viewCreator = viewCreator;
            this.viewInitializer = viewInitializer;

            // XXX: This is hella dumb.
            _inner = backingList.Changed
                .Buffer(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                .Subscribe(_ => this.NotifyDataSetChanged());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view;
            var data = list[position];

            view = convertView ?? viewCreator(ctx, data);

            var ivf = view as IViewFor<TViewModel>;
            if (ivf != null) {
                ivf.ViewModel = data;
            }

            viewInitializer(data, (TView)view);
            return view;
        }

        public override TViewModel this[int index] {
            get { return list[index]; }
        }

        public override long GetItemId(int position)
        {
            return list[position].GetHashCode();
        }

        public override int Count {
            get { return list.Count; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
        }
    }
}