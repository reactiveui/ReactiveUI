using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// Generates Observables based on observing Reactive objects
    /// </summary>
    public class IROObservableForProperty : ICreatesObservableForProperty
    {
        /// <summary>
        /// Returns a positive integer when this class supports GetNotificationForProperty for this
        /// particular Type. If the method isn't supported at all, return a non-positive integer.
        /// When multiple implementations return a positive value, the host will use the one which
        /// returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="beforeChanged">if set to <c>true</c> [before changed].</param>
        /// <returns>A positive integer if GNFP is supported, zero or a negative value otherwise</returns>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            // NB: Since every IReactiveObject is also an INPC, we need to bind more tightly than
            // INPCObservableForProperty, so we return 10 here instead of one
            return typeof(IReactiveObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 10 : 0;
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
        /// <exception cref="System.ArgumentException">Sender doesn't implement IReactiveObject</exception>
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            var iro = sender as IReactiveObject;
            if (iro == null) {
                throw new ArgumentException("Sender doesn't implement IReactiveObject");
            }

            var obs = beforeChanged ? iro.getChangingObservable() : iro.getChangedObservable();

            var memberInfo = expression.GetMemberInfo();
            if (beforeChanged) {
                if (expression.NodeType == ExpressionType.Index) {
                    return obs.Where(x => x.PropertyName.Equals(memberInfo.Name + "[]"))
                        .Select(x => new ObservedChange<object, object>(sender, expression));
                } else {
                    return obs.Where(x => x.PropertyName.Equals(memberInfo.Name))
                        .Select(x => new ObservedChange<object, object>(sender, expression));
                }
            } else {
                if (expression.NodeType == ExpressionType.Index) {
                    return obs.Where(x => x.PropertyName.Equals(memberInfo.Name + "[]"))
                        .Select(x => new ObservedChange<object, object>(sender, expression));
                } else {
                    return obs.Where(x => x.PropertyName.Equals(memberInfo.Name))
                        .Select(x => new ObservedChange<object, object>(sender, expression));
                }
            }
        }
    }
}