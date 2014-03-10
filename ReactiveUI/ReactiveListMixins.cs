using System;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Linq;

namespace ReactiveUI {
    public static class ReactiveListMixins {
        public static IDisposable Combine<T>(this ReactiveList<T> listOne, ReactiveList<T> listTwo, out ReactiveList<T> combinedList) {
            if (listOne == null) {
                throw new ArgumentNullException("listOne");
            }

            if (listTwo == null) {
                throw new ArgumentNullException("listTwo");
            }

            var list = new ReactiveList<T>();

            combinedList = list;

            combinedList.AddRange(listOne);
            combinedList.AddRange(listTwo);

            Action<NotifyCollectionChangedEventArgs> listChanged = (ea) => {
                switch (ea.Action) {
                    case NotifyCollectionChangedAction.Add:
                        list.AddRange(ea.NewItems.Cast<T>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        list.RemoveAll(ea.OldItems.Cast<T>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        list.RemoveAll(ea.OldItems.Cast<T>());
                        list.AddRange(ea.NewItems.Cast<T>());
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        using (list.SuppressChangeNotifications()) {
                            list.Clear();
                            list.AddRange(listOne);
                            list.AddRange(listTwo);
                        }
                        list.Reset();
                        break;
                }
            };

            return new CompositeDisposable(
                listOne.Changed.Subscribe(listChanged),
                listTwo.Changed.Subscribe(listChanged)
            );
        }
    }
}

