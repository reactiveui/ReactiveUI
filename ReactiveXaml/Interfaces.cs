using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Concurrency;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    public class ObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public string PropertyName { get; set; }
        public TValue Value { get; set; }
    }

    public interface IReactiveNotifyPropertyChanged : INotifyPropertyChanged, INotifyPropertyChanging, IObservable<PropertyChangedEventArgs> 
    { 
        IObservable<PropertyChangingEventArgs> BeforeChange {get;}
        IDisposable SuppressChangeNotifications();
    }

    public interface IReactiveCollection<T> : IList<T>, INotifyCollectionChanged, IEnableLogger
    {
        IObservable<T> ItemsAdded { get; }
        IObservable<T> BeforeItemsAdded { get; }
        IObservable<T> ItemsRemoved { get; }
        IObservable<T> BeforeItemsRemoved { get; }
        IObservable<int> CollectionCountChanged { get; }
        IObservable<int> CollectionCountChanging { get; }

        bool ChangeTrackingEnabled { get; set; }
        IDisposable SuppressChangeNotifications();
        IObservable<ObservedChange<T, object>> ItemPropertyChanging { get; }
        IObservable<ObservedChange<T, object>> ItemPropertyChanged { get; }
    }

    public interface IReactiveCommand : ICommand, IObservable<object> 
    {
        IObservable<bool> CanExecuteObservable {get;} 
    }

    public interface IReactiveAsyncCommand : IReactiveCommand
    {
        IObservable<int> ItemsInFlight { get; }
        IObservable<Unit> AsyncCompletedNotification { get; }
    }

    public interface IPromptUserForNewModel<T>
    {
        T Prompt(object Parameter);
    }

    public interface IViewForModel<T>
    {
        IDisposable Present(T Model, bool AsModal, Action OnClosed);
    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :
