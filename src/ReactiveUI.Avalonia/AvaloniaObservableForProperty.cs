using System;
using System.Linq.Expressions;

namespace ReactiveUI.Avalonia
{
    public class AvaloniaObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            throw new NotImplementedException();
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(
            object sender, Expression expression, string propertyName, bool beforeChanged = false)
        {
            throw new NotImplementedException();
        }
    }
}