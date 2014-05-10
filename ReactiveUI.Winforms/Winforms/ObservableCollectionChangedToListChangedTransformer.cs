using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ReactiveUI.Winforms
{
    static class ObservableCollectionChangedToListChangedTransformer
    {
        /// <summary>
        ///     Transforms a NotifyCollectionChangedEventArgs into zero or more ListChangedEventArgs
        /// </summary>
        /// <param name="ea"></param>
        /// <returns></returns>
        internal static IEnumerable<ListChangedEventArgs> AsListChangedEventArgs(this NotifyCollectionChangedEventArgs ea)
        {
            if (ea == null) {
                yield break;
            }

            switch (ea.Action) {
            case NotifyCollectionChangedAction.Reset:
                yield return new ListChangedEventArgs(ListChangedType.Reset, -1);
                break;
            case NotifyCollectionChangedAction.Replace:
                yield return new ListChangedEventArgs(ListChangedType.ItemChanged, ea.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (int index in Enumerable.Range(ea.OldStartingIndex, ea.OldItems.Count)) {
                    yield return new ListChangedEventArgs(ListChangedType.ItemDeleted, index);
                }
                break;
            case NotifyCollectionChangedAction.Add:
                foreach (int index in Enumerable.Range(ea.NewStartingIndex, ea.NewItems.Count)) {
                    yield return new ListChangedEventArgs(ListChangedType.ItemAdded, index);
                }
                break;
            case NotifyCollectionChangedAction.Move:
                //this one is actually not supported by the default BindingList<T> implementation
                //maybe we should do a reset instead?
		yield return new ListChangedEventArgs(ListChangedType.ItemDeleted, ea.OldStartingIndex);
		yield return new ListChangedEventArgs(ListChangedType.ItemAdded, ea.NewStartingIndex);
                break;
            }
        }
    }
}
