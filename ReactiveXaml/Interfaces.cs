using System;
using System.Collections;
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
    public interface IObservedChange<TSender, TValue>
    {
        TSender Sender { get; }
        string PropertyName { get; }
        TValue Value { get; }   
    }

    public class ObservedChange<TSender, TValue> : IObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public string PropertyName { get; set; }
        public TValue Value { get; set; }
    }

    public interface IReactiveNotifyPropertyChanged : INotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger
    { 
        IObservable<IObservedChange<object, object>> Changing { get; }
        IObservable<IObservedChange<object, object>> Changed { get; }

        IDisposable SuppressChangeNotifications();
    }

    public interface IReactiveNotifyPropertyChanged<TSender> : IReactiveNotifyPropertyChanged
    {    
        new IObservable<IObservedChange<TSender, object>> Changing { get; }
        new IObservable<IObservedChange<TSender, object>> Changed { get; }
    }

    public interface IReactiveCollection : IReactiveNotifyPropertyChanged, IEnumerable, INotifyCollectionChanged
    {
        IObservable<object> ItemsAdded { get; }
        IObservable<object> BeforeItemsAdded { get; }
        IObservable<object> ItemsRemoved { get; }
        IObservable<object> BeforeItemsRemoved { get; }
        IObservable<int> CollectionCountChanged { get; }
        IObservable<int> CollectionCountChanging { get; }

        IObservable<IObservedChange<object, object>> ItemChanging { get; }
        IObservable<IObservedChange<object, object>> ItemChanged { get; }

        bool ChangeTrackingEnabled { get; set; }
        IDisposable SuppressChangeNotifications();
    }

    public interface IReactiveCollection<T> : IList<T>, IReactiveCollection
    {
        new IObservable<T> ItemsAdded { get; }
        new IObservable<T> BeforeItemsAdded { get; }
        new IObservable<T> ItemsRemoved { get; }
        new IObservable<T> BeforeItemsRemoved { get; }

        IObservable<IObservedChange<T, object>> ItemChanging { get; }
        IObservable<IObservedChange<T, object>> ItemChanged { get; }
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

    public interface IMessageBus : IEnableLogger
    {
        IObservable<T> Listen<T>(string Contract = null);
        bool IsRegistered(Type Type, string Contract = null);
        void RegisterMessageSource<T>(IObservable<T> Source, string Contract = null);
        void SendMessage<T>(T Message, string Contract = null);
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

// vim: tw=120 ts=4 sw=4 et :
