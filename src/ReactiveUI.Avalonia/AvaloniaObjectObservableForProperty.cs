using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Splat;

namespace ReactiveUI.Avalonia
{
    public class AvaloniaObjectObservableForProperty : ICreatesObservableForProperty
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (!typeof(AvaloniaObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) return 0;
            return GetAvaloniaProperty(type, propertyName) != null ? 4 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(
            object sender, Expression expression, string propertyName, bool beforeChanged = false)
        {
            var type = sender.GetType();
            var avaloniaProperty = GetAvaloniaProperty(type, propertyName);
            if (avaloniaProperty == null) {
                var message = "Couldn't find avalonia property " + propertyName + " on " + type.Name;
                this.Log().Error(message);
                throw new NullReferenceException(message);
            }

            return avaloniaProperty.Changed.Select(args => {
                return new ObservedChange<object, object>(args, expression);
            });
        }
         
        private static AvaloniaProperty GetAvaloniaProperty(Type type, string propertyName)
        {
            var propertyFieldInfo = type.GetTypeInfo()
                .GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(x => x.Name == propertyName + "Property" && x.IsStatic);

            return propertyFieldInfo != null 
                ? (AvaloniaProperty)propertyFieldInfo.GetValue(null) 
                : null;
        }
    }
}
