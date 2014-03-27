using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI
{
    public class CanExecuteChangedEventManager : WeakEventManager<ICommand, EventHandler, EventArgs>
    {
    }

    public class PropertyChangingEventManager : WeakEventManager<INotifyPropertyChanging, PropertyChangingEventHandler, PropertyChangingEventArgs>
    {
    }

    public class PropertyChangedEventManager : WeakEventManager<INotifyPropertyChanged, PropertyChangedEventHandler, PropertyChangedEventArgs>
    {
    }

    public class CollectionChangingEventManager : WeakEventManager<INotifyCollectionChanging, NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
    {
    }

    public class CollectionChangedEventManager : WeakEventManager<INotifyCollectionChanged, NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
    {
    }

    /// <summary>
    /// WeakEventManager base class. Inspired by the WPF WeakEventManager class and the code in 
    /// http://social.msdn.microsoft.com/Forums/silverlight/en-US/34d85c3f-52ea-4adc-bb32-8297f5549042/command-binding-memory-leak?forum=silverlightbugs
    /// </summary>
    /// <typeparam name="TEventSource">The type of the event source.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    public class WeakEventManager<TEventSource, TEventHandler, TEventArgs>
    {
        static readonly object StaticSource = new object();

        /// <summary>
        /// Mapping between the target of the delegate (for example a Button) and the handler (EventHandler).
        /// Windows Phone needs this, otherwise the event handler gets garbage collected.
        /// </summary>
        ConditionalWeakTable<object, List<Delegate>> targetToEventHandler = new ConditionalWeakTable<object, List<Delegate>>();

        /// <summary>
        /// Mapping from the source of the event to the list of handlers. This is a CWT to ensure it does not leak the source of the event.
        /// </summary>
        ConditionalWeakTable<object, List<WeakHandler>> sourceToWeakHandlers = new ConditionalWeakTable<object, List<WeakHandler>>();

        static Lazy<WeakEventManager<TEventSource, TEventHandler, TEventArgs>> current = 
            new Lazy<WeakEventManager<TEventSource, TEventHandler, TEventArgs>>(() => new WeakEventManager<TEventSource, TEventHandler, TEventArgs>());

        static WeakEventManager<TEventSource, TEventHandler, TEventArgs> Current {
            get { return current.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventManager{TEventSource, TEventHandler, TEventArgs}"/> class.
        /// Protected to disallow instances of this class and force a subclass.
        /// </summary>
        protected WeakEventManager()
        {
        }

        /// <summary>
        /// Adds a weak reference to the handler and associates it with the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handler">The handler.</param>
        public static void AddHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (handler == null) throw new ArgumentNullException("handler");

            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("Handler must be Delegate type");
            }

            WeakEventManager<TEventSource, TEventHandler, TEventArgs>.Current.PrivateAddHandler(source, handler);
        }

        /// <summary>
        /// Removes the association between the source and the handler.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (handler == null) throw new ArgumentNullException("handler");

            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("handler must be Delegate type");
            }

            WeakEventManager<TEventSource, TEventHandler, TEventArgs>.Current.PrivateRemoveHandler(source, handler);
        }

        /// <summary>
        /// Delivers the event to the handlers registered for the source. 
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="TEventArgs"/> instance containing the event data.</param>
        public static void DeliverEvent(TEventSource sender, TEventArgs args)
        {
            WeakEventManager<TEventSource, TEventHandler, TEventArgs>.Current.PrivateDeliverEvent(sender, args);
        }

        /// <summary>
        /// Override this method to attach to an event.
        /// </summary>
        /// <param name="source">The source.</param>
        protected virtual void StartListening(object source)
        {
        }

        /// <summary>
        /// Override this method to detach from an event.
        /// </summary>
        /// <param name="source">The source.</param>
        protected virtual void StopListening(object source)
        {
        }

        void PrivateAddHandler(TEventSource source, TEventHandler handler)
        {
            this.AddWeakHandler(source, handler);
            this.AddTargetHandler(handler);
        }

        void AddWeakHandler(TEventSource source, TEventHandler handler)
        {
            WeakHandler handlerSink = new WeakHandler(source, handler);
            List<WeakHandler> weakHandlers;
            if (this.sourceToWeakHandlers.TryGetValue(source, out weakHandlers)) {
                weakHandlers.Add(handlerSink);
            } else {
                weakHandlers = new List<WeakHandler>();
                weakHandlers.Add(handlerSink);

                this.sourceToWeakHandlers.Add(source, weakHandlers);
                this.StartListening(source);
            }
        }

        void AddTargetHandler(TEventHandler handler)
        {
            var @delegate = handler as Delegate;
            object key = @delegate.Target ?? WeakEventManager<TEventSource, TEventHandler, TEventArgs>.StaticSource;
            List<Delegate> delegates;

            if (this.targetToEventHandler.TryGetValue(key, out delegates))
            {
                delegates.Add(@delegate);
            }
            else
            {
                delegates = new List<Delegate>();
                delegates.Add(@delegate);

                this.targetToEventHandler.Add(key, delegates);
            }
        }

        void PrivateRemoveHandler(TEventSource source, TEventHandler handler)
        {
            this.RemoveWeakHandler(source, handler);
            this.RemoveTargetHandler(handler);
        }

        void RemoveWeakHandler(TEventSource source, TEventHandler handler)
        {
            var weakHandlers = default(List<WeakHandler>);

            if (!this.sourceToWeakHandlers.TryGetValue(source, out weakHandlers)) return;

            foreach (var weakHandler in weakHandlers) {
                if (!weakHandler.Matches(source, handler)) continue;

                weakHandlers.Remove(weakHandler);

                if (weakHandlers.Count == 0) {
                    this.sourceToWeakHandlers.Remove(source);
                    this.StopListening(source);
                }

                break;
            }
        }

        void RemoveTargetHandler(TEventHandler handler)
        {
            var @delegate = handler as Delegate;
            object key = @delegate.Target ?? WeakEventManager<TEventSource, TEventHandler, TEventArgs>.StaticSource;

            var delegates = default(List<Delegate>);
            if (this.targetToEventHandler.TryGetValue(key, out delegates)) {
                delegates.Remove(@delegate);

                if (delegates.Count == 0) {
                    this.targetToEventHandler.Remove(key);
                }
            }
        }

        void PrivateDeliverEvent(object sender, TEventArgs args)
        {
            object source = sender != null ? sender : WeakEventManager<TEventSource, TEventHandler, TEventArgs>.StaticSource;
            var weakHandlers = default(List<WeakHandler>);

            if (this.sourceToWeakHandlers.TryGetValue(source, out weakHandlers)) {
                this.DeliverEventToList(source, args, weakHandlers);
            }
        }

        protected virtual void DeliverEventToList(object sender, TEventArgs args, List<WeakHandler> list)
        {
            foreach (var handler in list) {
                if (handler.IsActive) {
                    var @delegate = handler.Handler as Delegate;
                    @delegate.DynamicInvoke(sender, args);
                }
            }

            this.Purge(sender, list);
        }

        protected virtual void Purge(object sender, List<WeakHandler> list)
        {
            var inActive = default(List<WeakHandler>);

            foreach (var handler in list) {
                if (!handler.IsActive) {
                    if (inActive == null) inActive = new List<WeakHandler>();
                    inActive.Add(handler);
                }
            }

            if (inActive != null) {
                foreach (var handler in inActive) {
                    list.Remove(handler);
                }

                if (list.Count == 0) {
                    this.sourceToWeakHandlers.Remove(sender);
                }
            }
        }

        protected class WeakHandler
        {
            WeakReference source;
            WeakReference originalHandler;

            public bool IsActive {
                get { return this.source != null && this.source.IsAlive && this.originalHandler != null && this.originalHandler.IsAlive; }
            }

            public TEventHandler Handler {
                get {
                    if (this.originalHandler == null) {
                        return default(TEventHandler);
                    } else {
                        return (TEventHandler)this.originalHandler.Target;
                    }
                }
            }

            public WeakHandler(object source, TEventHandler originalHandler)
            {
                this.source = new WeakReference(source);
                this.originalHandler = new WeakReference(originalHandler);
            }

            public bool Matches(object source, TEventHandler handler)
            {
                return this.source != null && object.ReferenceEquals(this.source.Target, source)
                    && this.originalHandler != null && object.ReferenceEquals(this.originalHandler.Target, handler);
            }
        }
    }
}
