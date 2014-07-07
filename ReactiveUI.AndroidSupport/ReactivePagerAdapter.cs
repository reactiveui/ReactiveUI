using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using Android.Support.V4.View;
using Android.Views;
using Splat;
using Object = Java.Lang.Object;

namespace ReactiveUI
{
    /// <summary>
    /// ReactivePagerAdapter is a PagerAdapter that will interface with a
    /// ReactiveList, in a similar fashion to ReactiveTableViewSource.
    /// </summary>
    public class ReactivePagerAdapter<TViewModel> : PagerAdapter, IEnableLogger
        where TViewModel : class
    {
        readonly IReadOnlyReactiveList<TViewModel> list;
        readonly Func<TViewModel, ViewGroup, View> viewCreator;
        readonly Action<TViewModel, View> viewInitializer;
        IDisposable inner;

        public ReactivePagerAdapter(IReadOnlyReactiveList<TViewModel> backingList,
                                    Func<TViewModel, ViewGroup, View> viewCreator,
                                    Action<TViewModel, View> viewInitializer = null)
        {
            this.list = backingList;
            this.viewCreator = viewCreator;
            this.viewInitializer = viewInitializer;

            inner = this.list.Changed.Subscribe(_ => NotifyDataSetChanged());
        }

        public override bool IsViewFromObject(View view, Object @object)
        {
            return ((View)@object) == view;
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var data = list[position];

            // NB: PagerAdapter does not recycle itself.
            var theView = viewCreator(data, container);

            var ivf = theView.GetViewHost() as IViewFor<TViewModel>;
            if (ivf != null) {
                ivf.ViewModel = data;
            }

            if (viewInitializer != null) {
                viewInitializer(data, theView);
            }

            container.AddView(theView, 0);
            return theView;
        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            var view = (View)@object;
            container.RemoveView(view);
        }

        public override int Count {
            get { return list.Count; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref inner, Disposable.Empty).Dispose();
        }
    }
}
