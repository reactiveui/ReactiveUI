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
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            // NB: Since every IReactiveObject is also an INPC, we need to bind more 
            // tightly than INPCObservableForProperty, so we return 10 here 
            // instead of one
            return typeof (IReactiveObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 10 : 0;
        }

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
