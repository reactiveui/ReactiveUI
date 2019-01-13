// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// WeakEventManager base class. Inspired by the WPF WeakEventManager class and the code in
    /// http://social.msdn.microsoft.com/Forums/silverlight/en-US/34d85c3f-52ea-4adc-bb32-8297f5549042/command-binding-memory-leak?forum=silverlightbugs.
    /// </summary>
    /// <typeparam name="TEventSource">The type of the event source.</typeparam>
    /// <typeparam name="TEventHandler">The type of the event handler.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    public class WeakEventManager<TEventSource, TEventHandler, TEventArgs>
        where TEventSource : class
        where TEventHandler : class
    {
        private static readonly object StaticSource = new object();

        private static readonly Lazy<WeakEventManager<TEventSource, TEventHandler, TEventArgs>> current =
            new Lazy<WeakEventManager<TEventSource, TEventHandler, TEventArgs>>(() => new WeakEventManager<TEventSource, TEventHandler, TEventArgs>());

        /// <summary>
        /// Mapping between the target of the delegate (for example a Button) and the handler (EventHandler).
        /// Windows Phone needs this, otherwise the event handler gets garbage collected.
        /// </summary>
        private readonly ConditionalWeakTable<object, List<Delegate>> _targetToEventHandler = new ConditionalWeakTable<object, List<Delegate>>();

        /// <summary>
        /// Mapping from the source of the event to the list of handlers. This is a CWT to ensure it does not leak the source of the event.
        /// </summary>
        private readonly ConditionalWeakTable<object, WeakHandlerList> _sourceToWeakHandlers = new ConditionalWeakTable<object, WeakHandlerList>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakEventManager{TEventSource, TEventHandler, TEventArgs}"/> class.
        /// Protected to disallow instances of this class and force a subclass.
        /// </summary>
        protected WeakEventManager()
        {
        }

        private static WeakEventManager<TEventSource, TEventHandler, TEventArgs> Current => current.Value;

        /// <summary>
        /// Adds a weak reference to the handler and associates it with the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handler">The handler.</param>
        public static void AddHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("Handler must be Delegate type");
            }

            Current.PrivateAddHandler(source, handler);
        }

        /// <summary>
        /// Removes the association between the source and the handler.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveHandler(TEventSource source, TEventHandler handler)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!typeof(TEventHandler).GetTypeInfo().IsSubclassOf(typeof(Delegate)))
            {
                throw new ArgumentException("handler must be Delegate type");
            }

            Current.PrivateRemoveHandler(source, handler);
        }

        /// <summary>
        /// Delivers the event to the handlers registered for the source.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments instance containing the event data.</param>
        public static void DeliverEvent(TEventSource sender, TEventArgs args)
        {
            Current.PrivateDeliverEvent(sender, args);
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

        private void PrivateAddHandler(TEventSource source, TEventHandler handler)
        {
            AddWeakHandler(source, handler);
            AddTargetHandler(handler);
        }

        private void AddWeakHandler(TEventSource source, TEventHandler handler)
        {
            if (_sourceToWeakHandlers.TryGetValue(source, out var weakHandlers))
            {
                // clone list if we are currently delivering an event
                if (weakHandlers.IsDeliverActive)
                {
                    weakHandlers = weakHandlers.Clone();
                    _sourceToWeakHandlers.Remove(source);
                    _sourceToWeakHandlers.Add(source, weakHandlers);
                }

                weakHandlers.AddWeakHandler(source, handler);
            }
            else
            {
                weakHandlers = new WeakHandlerList();
                weakHandlers.AddWeakHandler(source, handler);

                _sourceToWeakHandlers.Add(source, weakHandlers);
                StartListening(source);
            }

            Purge(source);
        }

        private void AddTargetHandler(TEventHandler handler)
        {
            var @delegate = handler as Delegate;
            object key = @delegate.Target ?? StaticSource;

            if (_targetToEventHandler.TryGetValue(key, out var delegates))
            {
                delegates.Add(@delegate);
            }
            else
            {
                delegates = new List<Delegate>
                {
                    @delegate,
                };

                _targetToEventHandler.Add(key, delegates);
            }
        }

        private void PrivateRemoveHandler(TEventSource source, TEventHandler handler)
        {
            RemoveWeakHandler(source, handler);
            RemoveTargetHandler(handler);
        }

        private void RemoveWeakHandler(TEventSource source, TEventHandler handler)
        {
            if (_sourceToWeakHandlers.TryGetValue(source, out var weakHandlers))
            {
                // clone list if we are currently delivering an event
                if (weakHandlers.IsDeliverActive)
                {
                    weakHandlers = weakHandlers.Clone();
                    _sourceToWeakHandlers.Remove(source);
                    _sourceToWeakHandlers.Add(source, weakHandlers);
                }

                if (weakHandlers.RemoveWeakHandler(source, handler) && weakHandlers.Count == 0)
                {
                    _sourceToWeakHandlers.Remove(source);
                    StopListening(source);
                }
            }
        }

        private void RemoveTargetHandler(TEventHandler handler)
        {
            var @delegate = handler as Delegate;
            object key = @delegate?.Target ?? StaticSource;

            if (_targetToEventHandler.TryGetValue(key, out var delegates))
            {
                delegates.Remove(@delegate);

                if (delegates.Count == 0)
                {
                    _targetToEventHandler.Remove(key);
                }
            }
        }

        private void PrivateDeliverEvent(object sender, TEventArgs args)
        {
            object source = sender != null ? sender : StaticSource;

            bool hasStaleEntries = false;

            if (_sourceToWeakHandlers.TryGetValue(source, out var weakHandlers))
            {
                using (weakHandlers.DeliverActive())
                {
                    hasStaleEntries = weakHandlers.DeliverEvent(source, args);
                }
            }

            if (hasStaleEntries)
            {
                Purge(source);
            }
        }

        private void Purge(object source)
        {
            if (_sourceToWeakHandlers.TryGetValue(source, out var weakHandlers))
            {
                if (weakHandlers.IsDeliverActive)
                {
                    weakHandlers = weakHandlers.Clone();
                    _sourceToWeakHandlers.Remove(source);
                    _sourceToWeakHandlers.Add(source, weakHandlers);
                }
                else
                {
                    weakHandlers.Purge();
                }
            }
        }

        internal class WeakHandler
        {
            private readonly WeakReference _source;
            private readonly WeakReference _originalHandler;

            public WeakHandler(object source, TEventHandler originalHandler)
            {
                _source = new WeakReference(source);
                _originalHandler = new WeakReference(originalHandler);
            }

            public bool IsActive => _source != null && _source.IsAlive && _originalHandler != null && _originalHandler.IsAlive;

            public TEventHandler Handler
            {
                get
                {
                    if (_originalHandler == null)
                    {
                        return default(TEventHandler);
                    }

                    return (TEventHandler)_originalHandler.Target;
                }
            }

            public bool Matches(object source, TEventHandler handler)
            {
                return _source != null &&
                    ReferenceEquals(_source.Target, source) &&
                    _originalHandler != null &&
                    (ReferenceEquals(_originalHandler.Target, handler) ||
                    (_originalHandler.Target is PropertyChangedEventHandler eventHandler &&
                    handler is PropertyChangedEventHandler &&
                    Equals(
                        eventHandler.Target,
                        (handler as PropertyChangedEventHandler)?.Target)));
            }
        }

        internal class WeakHandlerList
        {
            private readonly List<WeakHandler> _handlers;
            private int _deliveries;

            public WeakHandlerList()
            {
                _handlers = new List<WeakHandler>();
            }

            public int Count => _handlers.Count;

            public bool IsDeliverActive => _deliveries > 0;

            public void AddWeakHandler(TEventSource source, TEventHandler handler)
            {
                WeakHandler handlerSink = new WeakHandler(source, handler);
                _handlers.Add(handlerSink);
            }

            public bool RemoveWeakHandler(TEventSource source, TEventHandler handler)
            {
                foreach (var weakHandler in _handlers)
                {
                    if (weakHandler.Matches(source, handler))
                    {
                        return _handlers.Remove(weakHandler);
                    }
                }

                return false;
            }

            public WeakHandlerList Clone()
            {
                WeakHandlerList newList = new WeakHandlerList();
                newList._handlers.AddRange(_handlers.Where(h => h.IsActive));

                return newList;
            }

            public IDisposable DeliverActive()
            {
                Interlocked.Increment(ref _deliveries);

                return Disposable.Create(() => Interlocked.Decrement(ref _deliveries));
            }

            public virtual bool DeliverEvent(object sender, TEventArgs args)
            {
                bool hasStaleEntries = false;

                foreach (var handler in _handlers)
                {
                    if (handler.IsActive)
                    {
                        var @delegate = handler.Handler as Delegate;
                        @delegate?.DynamicInvoke(sender, args);
                    }
                    else
                    {
                        hasStaleEntries = true;
                    }
                }

                return hasStaleEntries;
            }

            public void Purge()
            {
                for (int i = _handlers.Count - 1; i >= 0; i--)
                {
                    if (!_handlers[i].IsActive)
                    {
                        _handlers.RemoveAt(i);
                    }
                }
            }
        }
    }
}
