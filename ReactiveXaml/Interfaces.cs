using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IObservedChange<TSender, TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        TSender Sender { get; }

        /// <summary>
        /// 
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        ///
        /// </summary>
        TValue Value { get; }   
    }

    public class ObservedChange<TSender, TValue> : IObservedChange<TSender, TValue>
    {
        public TSender Sender { get; set; }
        public string PropertyName { get; set; }
        public TValue Value { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IReactiveNotifyPropertyChanged : INotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger
    { 
        /// <summary>
        /// 
        /// </summary>
        IObservable<IObservedChange<object, object>> Changing { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<IObservedChange<object, object>> Changed { get; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IDisposable SuppressChangeNotifications();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    public interface IReactiveNotifyPropertyChanged<TSender> : IReactiveNotifyPropertyChanged
    {    
        /// <summary>
        /// 
        /// </summary>
        new IObservable<IObservedChange<TSender, object>> Changing { get; }

        /// <summary>
        /// 
        /// </summary>
        new IObservable<IObservedChange<TSender, object>> Changed { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IReactiveCollection : IReactiveNotifyPropertyChanged, IEnumerable, INotifyCollectionChanged
    {
        //
        // Collection Tracking
        //

        /// <summary>
        /// 
        /// </summary>
        IObservable<object> ItemsAdded { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<object> BeforeItemsAdded { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<object> ItemsRemoved { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<object> BeforeItemsRemoved { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<int> CollectionCountChanged { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<int> CollectionCountChanging { get; }

        //
        // Change Tracking
        //

        /// <summary>
        /// 
        /// </summary>
        IObservable<IObservedChange<object, object>> ItemChanging { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<IObservedChange<object, object>> ItemChanged { get; }

        /// <summary>
        /// 
        /// </summary>
        bool ChangeTrackingEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDisposable SuppressChangeNotifications();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReactiveCollection<T> : IList<T>, IReactiveCollection
    {
        /// <summary>
        /// 
        /// </summary>
        new IObservable<T> ItemsAdded { get; }

        /// <summary>
        /// 
        /// </summary>
        new IObservable<T> BeforeItemsAdded { get; }

        /// <summary>
        /// 
        /// </summary>
        new IObservable<T> ItemsRemoved { get; }

        /// <summary>
        /// 
        /// </summary>
        new IObservable<T> BeforeItemsRemoved { get; }

        /// <summary>
        /// 
        /// </summary>
        IObservable<IObservedChange<T, object>> ItemChanging { get; }

        /// <summary>
        ///  
        /// </summary>
        IObservable<IObservedChange<T, object>> ItemChanged { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IReactiveCommand : ICommand, IObservable<object> 
    {
        /// <summary>
        /// 
        /// </summary>
        IObservable<bool> CanExecuteObservable { get; } 
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IReactiveAsyncCommand : IReactiveCommand
    {
        /// <summary>
        /// 
        /// </summary>
        IObservable<int> ItemsInFlight { get; }

        /// <summary>
        ///  
        /// </summary>
        IObservable<Unit> AsyncCompletedNotification { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMessageBus : IEnableLogger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contract"></param>
        /// <returns></returns>
        IObservable<T> Listen<T>(string contract = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        bool IsRegistered(Type type, string contract = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="contract"></param>
        void RegisterMessageSource<T>(IObservable<T> source, string contract = null);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="contract"></param>
        void SendMessage<T>(T message, string contract = null);
    }

#if DEBUG
    public interface IPromptUserForNewModel<T>
    {
        T Prompt(object parameter);
    }

    public interface IViewForModel<T>
    {
        IDisposable Present(T model, bool asModal, Action onClosed);
    }
#endif
}

// vim: tw=120 ts=4 sw=4 et :
