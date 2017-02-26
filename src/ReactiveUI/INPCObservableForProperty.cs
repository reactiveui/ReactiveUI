using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// Generates Observables based on observing INotifyPropertyChanged objects
    /// </summary>
    public class INPCObservableForProperty : ICreatesObservableForProperty
    {
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
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged)
        {
            var target = beforeChanged ? typeof(INotifyPropertyChanging) : typeof(INotifyPropertyChanged);
            return target.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 5 : 0;
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
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged)
        {
            var before = sender as INotifyPropertyChanging;
            var after = sender as INotifyPropertyChanged;

            if (beforeChanged ? before == null : after == null) {
                return Observable<IObservedChange<object, object>>.Never;
            }

            var memberInfo = expression.GetMemberInfo();
            if (beforeChanged) {
                var obs = Observable.FromEventPattern<PropertyChangingEventHandler, PropertyChangingEventArgs>(
                    x => before.PropertyChanging += x, x => before.PropertyChanging -= x);

                if (expression.NodeType == ExpressionType.Index) {
                    return obs.Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName)
                        || x.EventArgs.PropertyName.Equals(memberInfo.Name + "[]"))
                        .Select(x => new ObservedChange<object, object>(sender, expression));
                } else {
                    return obs.Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName)
                        || x.EventArgs.PropertyName.Equals(memberInfo.Name))
                    .Select(x => new ObservedChange<object, object>(sender, expression));
                }
            } else {
                var obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    x => after.PropertyChanged += x, x => after.PropertyChanged -= x);

                if (expression.NodeType == ExpressionType.Index) {
                    return obs.Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName)
                        || x.EventArgs.PropertyName.Equals(memberInfo.Name + "[]"))
                    .Select(x => new ObservedChange<object, object>(sender, expression));
                } else {
                    return obs.Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName)
                        || x.EventArgs.PropertyName.Equals(memberInfo.Name))
                    .Select(x => new ObservedChange<object, object>(sender, expression));
                }
            }
        }
    }
}