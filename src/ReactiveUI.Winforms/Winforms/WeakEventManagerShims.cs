using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI.Winforms
{
    internal class CanExecuteChangedEventManager : WeakEventManager<ICommand, EventHandler, EventArgs>
    {
    }

    internal class PropertyChangingEventManager : WeakEventManager<INotifyPropertyChanging, PropertyChangingEventHandler, PropertyChangingEventArgs>
    {
    }

    internal class PropertyChangedEventManager : WeakEventManager<INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>
    {
    }

    internal class CollectionChangingEventManager : WeakEventManager<INotifyCollectionChanging, NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
    {
    }

    internal class CollectionChangedEventManager : WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
    {
    }
}
