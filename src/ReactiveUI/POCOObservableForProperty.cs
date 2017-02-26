using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// This class is the final fallback for WhenAny, and will simply immediately return the value of
    /// the type at the time it was created. It will also warn the user that this is probably not
    /// what they want to do
    /// </summary>
    public class POCOObservableForProperty : ICreatesObservableForProperty
    {
        private static readonly Dictionary<Type, bool> hasWarned = new Dictionary<Type, bool>();

        /// <summary>
        /// Returns a positive integer when this class supports GetNotificationForProperty for this
        /// particular Type. If the method isn't supported at all, return a non-positive integer.
        /// When multiple implementations return a positive value, the host will use the one which
        /// returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <param name="propertyName"></param>
        /// <param name="beforeChanged"></param>
        /// <returns>A positive integer if GNFP is supported, zero or a negative value otherwise</returns>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            return 1;
        }

        /// <summary>
        /// Subscribe to notifications on the specified property, given an object and a property name.
        /// </summary>
        /// <param name="sender">The object to observe.</param>
        /// <param name="expression">
        /// The expression on the object to observe. This will be either a MemberExpression or an
        /// IndexExpression dependending on the property.
        /// </param>
        /// <param name="beforeChanged">
        /// If true, signal just before the property value actually changes. If false, signal after
        /// the property changes.
        /// </param>
        /// <returns>
        /// An IObservable which is signalled whenever the specified property on the object changes.
        /// If this cannot be done for a specified value of beforeChanged, return Observable.Never
        /// </returns>
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            var type = sender.GetType();
            if (!hasWarned.ContainsKey(type)) {
                this.Log().Warn(
                    "{0} is a POCO type and won't send change notifications, WhenAny will only return a single value!",
                    type.FullName);
                hasWarned[type] = true;
            }

            return Observable.Return(new ObservedChange<object, object>(sender, expression), RxApp.MainThreadScheduler)
                .Concat(Observable<IObservedChange<object, object>>.Never);
        }
    }
}