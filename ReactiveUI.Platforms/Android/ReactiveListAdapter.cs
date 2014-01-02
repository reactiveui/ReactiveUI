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
    {
        readonly IReadOnlyReactiveList<TViewModel> list;
        readonly Action<TViewModel, TView> viewInitializer;
        readonly Func<TViewModel, int> viewResourceId;
        readonly Context ctx;
        readonly bool canRecycleViews;
        readonly LayoutInflater inflater;

        IDisposable _inner;


        public ReactiveListAdapter(Context ctx, IReadOnlyReactiveList<TViewModel> backingList, Func<TViewModel, int> viewResourceId, Action<TViewModel, TView> viewInitializer)
        {
            this.ctx = ctx;
            this.list = backingList;
            this.viewResourceId = viewResourceId;
            this.viewInitializer = viewInitializer;
            this.inflater = (LayoutInflater)ctx.GetSystemService(Context.LayoutInflaterService);

            // XXX: This is hella dumb.
            _inner = backingList.Changed
                .Buffer(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                .Subscribe(_ => this.NotifyDataSetChanged());
        }

        public ReactiveListAdapter(Context ctx, IReadOnlyReactiveList<TViewModel> backingList, int viewResourceId, Action<TViewModel, TView> viewInitializer)
            : this(ctx, backingList, _ => viewResourceId, viewInitializer)
        {
            canRecycleViews = true;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            
            View view;

            var data = list[position];

            if (canRecycleViews)
            {
                view = convertView ?? inflater.Inflate(viewResourceId(data), parent, false);
            }
            else
            {
                view = inflater.Inflate(viewResourceId(data), parent, false);
            }

            viewInitializer(data, (TView)view);
            return view;
        }

        public override TViewModel this[int index]
        {
            get { return list[index]; }
        }

        public override long GetItemId(int position)
        {
            return list[position].GetHashCode();
        }

        public override int Count
        {
            get { return list.Count; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref _inner, Disposable.Empty).Dispose();
        }
    }

}