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
    /// ReactivePagerAdapter is a PagerAdapter that will interface with a ReactiveList, in a similar
    /// fashion to ReactiveTableViewSource.
    /// </summary>
    public class ReactivePagerAdapter<TViewModel> : PagerAdapter, IEnableLogger
        where TViewModel : class
    {
        private readonly IReadOnlyReactiveList<TViewModel> list;
        private readonly Func<TViewModel, ViewGroup, View> viewCreator;
        private readonly Action<TViewModel, View> viewInitializer;
        private IDisposable inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactivePagerAdapter{TViewModel}"/> class.
        /// </summary>
        /// <param name="backingList">The backing list.</param>
        /// <param name="viewCreator">The view creator.</param>
        /// <param name="viewInitializer">The view initializer.</param>
        public ReactivePagerAdapter(IReadOnlyReactiveList<TViewModel> backingList,
                                    Func<TViewModel, ViewGroup, View> viewCreator,
                                    Action<TViewModel, View> viewInitializer = null)
        {
            this.list = backingList;
            this.viewCreator = viewCreator;
            this.viewInitializer = viewInitializer;

            this.inner = this.list.Changed.Subscribe(_ => NotifyDataSetChanged());
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public override int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// Destroys the item.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="position">The position.</param>
        /// <param name="object">The object.</param>
        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            var view = (View)@object;
            container.RemoveView(view);
        }

        /// <summary>
        /// Instantiates the item.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var data = this.list[position];

            // NB: PagerAdapter does not recycle itself.
            var theView = this.viewCreator(data, container);

            var ivf = theView.GetViewHost() as IViewFor<TViewModel>;
            if (ivf != null) {
                ivf.ViewModel = data;
            }

            if (this.viewInitializer != null) {
                this.viewInitializer(data, theView);
            }

            container.AddView(theView, 0);
            return theView;
        }

        /// <summary>
        /// Determines whether [is view from object] [the specified view].
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="object">The object.</param>
        /// <returns><c>true</c> if [is view from object] [the specified view]; otherwise, <c>false</c>.</returns>
        public override bool IsViewFromObject(View view, Object @object)
        {
            return ((View)@object) == view;
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <param name="disposing">To be added.</param>
        /// <remarks>To be added.</remarks>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref this.inner, Disposable.Empty).Dispose();
        }
    }
}