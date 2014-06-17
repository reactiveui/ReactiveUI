using System;
using ReactiveUI;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Linq.Expressions;

namespace ReactiveUI
{
    public abstract class UIKitObservableForPropertyBase :
        ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (beforeChanged)
                return 0;

            var match = config.Keys
                .Where(x=> x.IsAssignableFrom(type) && config[x].Keys.Contains(propertyName))
                .Select(x=> config[x][propertyName])
                .OrderByDescending(x=> x.Affinity)
                .FirstOrDefault();

            if(match == null)
                return 0;

            return match.Affinity;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            if (beforeChanged)
                return Observable.Never<IObservedChange<object, object>>();

            var type = sender.GetType();
            var propertyName = expression.GetMemberInfo().Name;

            var match = config.Keys
                .Where(x=> x.IsAssignableFrom(type) && config[x].Keys.Contains(propertyName))
                .Select(x=> config[x][propertyName])
                .OrderByDescending(x=> x.Affinity)
                .FirstOrDefault();

            if(match == null)
                throw new NotSupportedException(string.Format("Notifications for {0}.{1} are not supported", type.Name, propertyName));

            return match.CreateObservable((NSObject) sender, expression);
        }

        internal class ObservablePropertyInfo
        {
            public int Affinity;
            public Func<NSObject, Expression, IObservable<IObservedChange<object, object>>> CreateObservable;
        }

        /// <summary>
        /// Configuration map
        /// </summary>
        readonly Dictionary<Type, Dictionary<string, ObservablePropertyInfo>> config =
            new Dictionary<Type, Dictionary<string, ObservablePropertyInfo>>();

        /// <summary>
        /// Registers an observable factory for the specified type and property.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="property">Property.</param>
        /// <param name="createObservable">Create observable.</param>
        protected void Register(Type type, string property, int affinity, Func<NSObject, Expression, IObservable<IObservedChange<object, object>>> createObservable)
        {
            Dictionary<string, ObservablePropertyInfo> typeProperties;
            if(!config.TryGetValue(type, out typeProperties))
            {
                typeProperties = new Dictionary<string, ObservablePropertyInfo>();
                config[type] = typeProperties;
            }

            var info = new ObservablePropertyInfo { Affinity = affinity, CreateObservable = createObservable };
            typeProperties[property] = info;
        }

        /// <summary>
        /// Creates an Observable for a UIControl Event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="expression">The expression.</param>
        /// <param name="evt">The control event to listen for</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// An observable.
        /// </returns>
        protected static IObservable<IObservedChange<object, object>> ObservableFromUIControlEvent(NSObject sender, Expression expression, UIControlEvent evt, Func<NSObject, object> value)
        {
            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                var control = (UIControl) sender;

                EventHandler handler = (s,e)=>
                {
                    subj.OnNext(new ObservedChange<object, object>(sender, expression, value(sender)));
                };

                control.AddTarget(handler, evt);

                return Disposable.Create(() =>
                {
                    control.RemoveTarget(handler, evt);
                });
            });
        }

        /// <summary>
        /// Creates an Observable for a NSNotificationCenter notification
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="notification">Notification.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The from notification.
        /// </returns>
        protected static IObservable<IObservedChange<object, object>> ObservableFromNotification(NSObject sender, Expression expression, NSString notification, Func<NSObject, object> value)
        {
            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                var handle = NSNotificationCenter.DefaultCenter.AddObserver (notification, (e)=>
                {
                    subj.OnNext(new ObservedChange<object, object>(sender, expression, value(sender)));
                }, sender);

                return Disposable.Create(() =>
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(handle);
                });
            });
        }

        /// <summary>
        /// Creates an Observable for a NSNotificationCenter notification
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The from notification.
        /// </returns>
        protected static IObservable<IObservedChange<object, object>> ObservableFromEvent(NSObject sender, Expression expression, string eventName, Func<NSObject, object> value)
        {
            return Observable.Create<IObservedChange<object, object>>(subj =>
            {
                return Observable.FromEventPattern(sender, eventName).Subscribe((e) =>
                {
                    subj.OnNext(new ObservedChange<object, object>(sender, expression, value(sender)));
                });
            });
        }
    }
}

