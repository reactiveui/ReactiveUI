using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;

namespace ReactiveUI
{
    public static class Reflection 
    {
    #if SILVERLIGHT || WINRT
        static MemoizingMRUCache<Tuple<Type, string>, FieldInfo> backingFieldInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, FieldInfo>(
                (x, _) => (x.Item1).GetField(RxApp.GetFieldNameForProperty(x.Item2)), 
                15 /*items*/);

        static readonly MemoizingMRUCache<Tuple<Type, string>, Func<object, object>> propReaderCache = 
            new MemoizingMRUCache<Tuple<Type, string>, Func<object, object>>((x,_) => {
                var fi = (x.Item1).GetField(x.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                if (fi != null) {
                    return (fi.GetValue);
                }
                var pi = (x.Item1).GetProperty(x.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                if (pi != null) {
                    return (y => pi.GetValue(y, null));
                }

                return null;
            }, 15);
    #else
        static readonly MemoizingMRUCache<Tuple<Type, string>, FieldInfo> backingFieldInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, FieldInfo>((x, _) => {
                var fieldName = RxApp.GetFieldNameForProperty(x.Item2);
                var ret = (x.Item1).GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return ret;
            }, 50/*items*/);

        static readonly MemoizingMRUCache<Tuple<Type, string>, Func<object, object>> propReaderCache = 
            new MemoizingMRUCache<Tuple<Type, string>, Func<object, object>>((x,_) => {
                var fi = (x.Item1).GetField(x.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                if (fi != null) {
                    return (fi.GetValue);
                }
                var pi = (x.Item1).GetProperty(x.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                if (pi != null) {
                    return (y => pi.GetValue(y, null));
                }

                return null;
            }, 50);

        static readonly MemoizingMRUCache<Tuple<Type, string>, Action<object, object>> propWriterCache = 
            new MemoizingMRUCache<Tuple<Type, string>, Action<object, object>>((x,_) => {
                var fi = (x.Item1).GetField(x.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                if (fi != null) {
                    return (fi.SetValue);
                }
                var pi = (x.Item1).GetProperty(x.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                if (pi != null) {
                    return ((y,v) => pi.SetValue(y, v, null));
                }

                return null;
            }, 50);
    #endif

        public static string SimpleExpressionToPropertyName<TObj, TRet>(Expression<Func<TObj, TRet>> property)
        {
            Contract.Requires(property != null);

            string propName = null;

            try {
                var propExpr = property.Body as MemberExpression;
                if (propExpr.Expression.NodeType != ExpressionType.Parameter) {
                    throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
                }

                propName = propExpr.Member.Name;
            } catch (NullReferenceException) {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }

            return propName;
        }

        public static string[] ExpressionToPropertyNames<TObj, TRet>(Expression<Func<TObj, TRet>> property)
        {
            var ret = new List<string>();

            var current = property.Body;
            while(current.NodeType != ExpressionType.Parameter) {

                // This happens when a value type gets boxed
                if (current.NodeType == ExpressionType.Convert || current.NodeType == ExpressionType.ConvertChecked) {
                    var ue = (UnaryExpression) current;
                    current = ue.Operand;
                    continue;
                }

                if (current.NodeType != ExpressionType.MemberAccess) {
                    throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty.SomeOtherProperty'");
                }

                var me = (MemberExpression)current;
                ret.Insert(0, me.Member.Name);
                current = me.Expression;
            }

            return ret.ToArray();
        }

        public static FieldInfo GetBackingFieldInfoForProperty<TObj>(string propName, bool dontThrow = false)
            where TObj : IReactiveNotifyPropertyChanged
        {
            Contract.Requires(propName != null);
            FieldInfo field;

            lock(backingFieldInfoTypeCache) {
                field = backingFieldInfoTypeCache.Get(new Tuple<Type,string>(typeof(TObj), propName));
            }

            if (field == null && !dontThrow) {
                throw new ArgumentException("You must declare a backing field for this property named: " + 
                    RxApp.GetFieldNameForProperty(propName));
            }

            return field;
        }

        public static Func<TObj, object> GetValueFetcherForProperty<TObj>(string propName)
        {
            var ret = GetValueFetcherForProperty(typeof(TObj), propName);
            return x => (TObj) ret(x);
        }

        public static Func<object, object> GetValueFetcherForProperty(Type type, string propName)
        {
            Contract.Requires(type != null);
            Contract.Requires(propName != null);

            lock (propReaderCache) {
                return propReaderCache.Get(Tuple.Create(type, propName));
            }
        }

        public static Func<object, object> GetValueFetcherOrThrow(Type type, string propName)
        {
            var ret = GetValueFetcherForProperty(type, propName);

            if (ret == null) {
                throw new ArgumentException(String.Format("Type '{0}' must have a property '{1}'", type, propName));
            }
            return ret;
        }

        public static Action<object, object> GetValueSetterForProperty(Type type, string propName)
        {
            Contract.Requires(type != null);
            Contract.Requires(propName != null);

            lock (propReaderCache) {
                return propWriterCache.Get(Tuple.Create(type, propName));
            }
        }

        public static Action<object, object> GetValueSetterOrThrow(Type type, string propName)
        {
            var ret = GetValueSetterForProperty(type, propName);

            if (ret == null) {
                throw new ArgumentException(String.Format("Type '{0}' must have a property '{1}'", type, propName));
            }
            return ret;
        }

        public static bool TryGetValueForPropertyChain<TValue>(out TValue changeValue, object current, string[] propNames)
        {
            foreach (var propName in propNames.SkipLast(1)) {
                if (current == null) {
                    changeValue = default(TValue);
                    return false;
                }

                current = GetValueFetcherOrThrow(current.GetType(), propName)(current);
            }

            if (current == null) {
                changeValue = default(TValue);
                return false;
            }

            changeValue = (TValue) GetValueFetcherOrThrow(current.GetType(), propNames.Last())(current);
            return true;
        }

        public static bool SetValueToPropertyChain<TValue>(object target, string[] propNames, TValue value, bool shouldThrow = true)
        {
            foreach (var propName in propNames.SkipLast(1)) {
                var getter = shouldThrow ?
                    GetValueFetcherOrThrow(target.GetType(), propName) :
                    GetValueFetcherForProperty(target.GetType(), propName);

                target = getter(target);
            }

            var setter = shouldThrow ?
                GetValueSetterOrThrow(target.GetType(), propNames.Last()) :
                GetValueSetterForProperty(target.GetType(), propNames.Last());

            if (setter == null) return false;
            setter(target, value);
            return true;
        }

        static readonly MemoizingMRUCache<string, Type> typeCache = new MemoizingMRUCache<string, Type>((type,_) => {
    #if WINRT
            // WinRT hates your favorite band too.
            return Type.GetType(type, false);
    #else
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.FullName == type)
                .FirstOrDefault();
    #endif
        }, 20);

        public static Type ReallyFindType(string type, bool throwOnFailure) 
        {
            lock (typeCache) {
                var ret = typeCache.Get(type);
                if (ret != null || !throwOnFailure) return ret;
                throw new TypeLoadException();
            }
        }

        internal static IObservable<TProp> ViewModelWhenAnyValue<TView, TViewModel, TProp>(TViewModel viewModel, TView view, Expression<Func<TViewModel, TProp>> property)
            where TView : IViewForViewModel<TViewModel>
            where TViewModel : class
        {
            return view.WhenAny(x => x.ViewModel, x => x.Value)
                .Where(x => x != null)
                .SelectMany(x => x.WhenAny(property, y => y.Value));
        }
    }
}