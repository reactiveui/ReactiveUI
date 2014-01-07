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
using Splat;

namespace ReactiveUI.Android
{
    public class ReactiveListAdapter<TViewModel, TViewHolder> : BaseAdapter<TViewModel>, IEnableLogger
        where TViewHolder : IViewHolder
        where TViewModel : class
    {
        readonly IReadOnlyReactiveList<TViewModel> list;
        readonly int viewLayoutId;
        readonly Action<TViewModel, TViewHolder> viewInitializer;
        readonly Func<View, TViewHolder> viewCreator;
        readonly Context ctx;
        private readonly LayoutInflater inflater;
        readonly bool usesBindingOnly;

        private const int VIEW_HOLDER = -1337;

        IDisposable _inner;

        public ReactiveListAdapter(
            Context ctx,
            IReadOnlyReactiveList<TViewModel> backingList,
            int viewLayoutId,
            Func<View, TViewHolder> viewCreator,
            Action<TViewModel, TViewHolder> viewInitializer,
            bool usesBindingOnly = true)
        {
            this.ctx = ctx;
            this.list = backingList;
            this.viewLayoutId = viewLayoutId;
            this.viewCreator = viewCreator;
            this.viewInitializer = viewInitializer;
            this.usesBindingOnly = usesBindingOnly;

            inflater = LayoutInflater.From(ctx);

            // XXX: This is hella dumb.
            _inner = backingList.Changed
         //       .Log(this, "Collection Changed")
                // .Buffer(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                .Subscribe(_ => this.NotifyDataSetChanged());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            var data = list[position];

            TViewHolder viewHolder;
            if (convertView == null)
            {
                convertView = inflater.Inflate(viewLayoutId, parent, false);
                viewHolder = viewCreator(convertView);

                convertView.SetTag(VIEW_HOLDER, viewHolder.ToJavaObject());

                // for binding only, call the inializer once here
                if (usesBindingOnly)
                {
                    viewInitializer(data, viewHolder);
                }
            }
            else
            {
                viewHolder = convertView.GetTag(VIEW_HOLDER).ToNetObject<TViewHolder>();
            }


            var ivf = viewHolder as IViewFor<TViewModel>;
            if (ivf != null)
            {
                ivf.ViewModel = data;
            }

            if (!usesBindingOnly)
            {
                // call the inializer here for a call back on every getView
                viewInitializer(data, viewHolder);
            }

            return convertView;
        }

        public override TViewModel this[int index]
        {
            get { return list[index]; }
        }

        public override long GetItemId(int position)
        {
            return position;
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