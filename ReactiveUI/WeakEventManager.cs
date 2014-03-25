using System;
using System.Collections.Generic;
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

    public class WeakEventManager<TEventSource, TEventHandler, TEventArgs>
    {
        private static readonly object StaticSource = new object();
        private ConditionalWeakTable<object, IList<Delegate>> targetToEventHandler = new ConditionalWeakTable<object, IList<Delegate>>();
        private ConditionalWeakTable<object, IList<WeakHandler>> sourceToWeakHandlers = new ConditionalWeakTable<object, IList<WeakHandler>>();

        private static Lazy<WeakEventManager<TEventSource, TEventHandler, TEventArgs>> current = new Lazy<WeakEventManager<TEventSource, TEventHandler, TEventArgs>>(() => new WeakEventManager<TEventSource, TEventHandler, TEventArgs>());

        private static WeakEventManager<TEventSource, TEventHandler, TEventArgs> Current
        {
            get { return current.Value; }
        }

        protected WeakEventManager()
        {
        }

        public static void AddHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("handler must be Delegate type");
            }
            WeakEventManager<TEventSource, TEventHandler, TEventArgs>.Current.PrivateAddHandler(source, handler);
        }

        public static void RemoveHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("handler must be Delegate type");
            }
            WeakEventManager<TEventSource, TEventHandler, TEventArgs>.Current.PrivateRemoveHandler(source, handler);
        }

        public static void DeliverEvent(TEventSource sender, TEventArgs args)
        {
            WeakEventManager<TEventSource, TEventHandler, TEventArgs>.Current.PrivateDeliverEvent(sender, args);
        }

        private void PrivateAddHandler(TEventSource source, TEventHandler handler)
        {
            this.AddWeakHandler(source, handler);
            this.AddTargetHandler(handler);
        }

        private void AddWeakHandler(TEventSource source, TEventHandler handler)
        {
            WeakHandler handlerSink = new WeakHandler(source, handler);
            IList<WeakHandler> weakHandlers;
            if (this.sourceToWeakHandlers.TryGetValue(source, out weakHandlers))
            {
                weakHandlers.Add(handlerSink);
            }
            else
            {
                weakHandlers = new List<WeakHandler>();
                weakHandlers.Add(handlerSink);
                this.sourceToWeakHandlers.Add(source, weakHandlers);
            }
        }

        private void AddTargetHandler(TEventHandler handler)
        {
            Delegate @delegate = handler as Delegate;
            object key = @delegate.Target ?? WeakEventManager<TEventSource, TEventHandler, TEventArgs>.StaticSource;
            IList<Delegate> delegates;
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

        private void PrivateRemoveHandler(TEventSource source, TEventHandler handler)
        {
            this.RemoveWeakHandler(source, handler);
            this.RemoveTargetHandler(handler);
        }

        private void RemoveWeakHandler(TEventSource source, TEventHandler handler)
        {
            IList<WeakHandler> weakHandlers;
            if (this.sourceToWeakHandlers.TryGetValue(source, out weakHandlers))
            {
                foreach (WeakHandler weakHandler in weakHandlers)
                {
                    if (weakHandler.Matches(source, handler))
                    {
                        weakHandlers.Remove(weakHandler);
                        break;
                    }
                }
            }
        }

        private void RemoveTargetHandler(TEventHandler handler)
        {
            Delegate @delegate = handler as Delegate;
            object key = @delegate.Target ?? WeakEventManager<TEventSource, TEventHandler, TEventArgs>.StaticSource;
            IList<Delegate> delegates;
            if (this.targetToEventHandler.TryGetValue(key, out delegates))
            {
                delegates.Remove(@delegate);
                if (delegates.Count == 0)
                {
                    this.targetToEventHandler.Remove(key);
                }
            }
        }

        private void PrivateDeliverEvent(object sender, TEventArgs args)
        {
            object source = sender != null ? sender : WeakEventManager<TEventSource, TEventHandler, TEventArgs>.StaticSource;
            IList<WeakHandler> weakHandlers;
            if (this.sourceToWeakHandlers.TryGetValue(source, out weakHandlers))
            {
                this.DeliverEventToList(source, args, weakHandlers);
            }
        }

        private void DeliverEventToList(object sender, TEventArgs args, IList<WeakHandler> list)
        {
            foreach (var handler in list)
            {
                if (handler.IsActive)
                {
                    Delegate @delegate = handler.Handler as Delegate;
                    @delegate.DynamicInvoke(sender, args);
                }
            }
        }

        private class WeakHandler
        {
            private WeakReference source;
            private WeakReference originalHandler;

            public bool IsActive
            {
                get { return this.source != null && this.source.IsAlive && this.originalHandler != null && this.originalHandler.IsAlive; }
            }

            public TEventHandler Handler
            {
                get
                {
                    if (this.originalHandler == null)
                    {
                        return default(TEventHandler);
                    }
                    else
                    {
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
