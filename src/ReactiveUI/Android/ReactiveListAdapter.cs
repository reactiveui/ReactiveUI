using System;
using System.Reactive.Disposables;
using System.Threading;
using Android.Views;
using Android.Widget;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Reactive List Adapter
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <seealso cref="Android.Widget.BaseAdapter{TViewModel}"/>
    /// <seealso cref="Splat.IEnableLogger"/>
    public class ReactiveListAdapter<TViewModel> : BaseAdapter<TViewModel>, IEnableLogger
        where TViewModel : class
    {
        private readonly IReadOnlyReactiveList<TViewModel> list;
        private readonly Func<TViewModel, ViewGroup, View> viewCreator;
        private readonly Action<TViewModel, View> viewInitializer;

        private IDisposable _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveListAdapter{TViewModel}"/> class.
        /// </summary>
        /// <param name="backingList">The backing list.</param>
        /// <param name="viewCreator">The view creator.</param>
        /// <param name="viewInitializer">The view initializer.</param>
        public ReactiveListAdapter(
            IReadOnlyReactiveList<TViewModel> backingList,
            Func<TViewModel, ViewGroup, View> viewCreator,
            Action<TViewModel, View> viewInitializer = null)
        {
            this.list = backingList;
            this.viewCreator = viewCreator;
            this.viewInitializer = viewInitializer;

            this._inner = this.list.Changed.Subscribe(_ => NotifyDataSetChanged());
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <value>To be added.</value>
        /// <remarks>To be added.</remarks>
        public override int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// Indicates whether the item ids are stable across changes to the underlying data.
        /// </summary>
        /// <value>To be added.</value>
        /// <remarks>
        /// <para tool="javadoc-to-mdoc">
        /// Indicates whether the item ids are stable across changes to the underlying data.
        /// </para>
        /// <para tool="javadoc-to-mdoc">
        /// <format type="text/html"><a
        /// href="http://developer.android.com/reference/android/widget/BaseAdapter.html#hasStableIds()"
        /// target="_blank">[Android Documentation]</a></format>
        /// </para>
        /// </remarks>
        /// <since version="Added in API level 1"/>
        public override bool HasStableIds
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the ViewModel at the specified index.
        /// </summary>
        /// <value>The ViewModel.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override TViewModel this[int index]
        {
            get { return this.list[index]; }
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <param name="position">To be added.</param>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public override long GetItemId(int position)
        {
            return this.list[position].GetHashCode();
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <param name="position">To be added.</param>
        /// <param name="convertView">To be added.</param>
        /// <param name="parent">To be added.</param>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View theView = convertView;
            var data = this.list[position];

            if (theView == null) {
                theView = this.viewCreator(data, parent);
            }

            var ivf = theView.GetViewHost() as IViewFor<TViewModel>;
            if (ivf != null) {
                ivf.ViewModel = data;
            }

            if (this.viewInitializer != null) {
                this.viewInitializer(data, theView);
            }

            return theView;
        }

        /// <summary>
        /// Provides disposable support
        /// </summary>
        /// <param name="disposing">To be added.</param>
        /// <remarks>To be added.</remarks>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Interlocked.Exchange(ref this._inner, Disposable.Empty).Dispose();
        }
    }
}