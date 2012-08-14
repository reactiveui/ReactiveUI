using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
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

        static MemoizingMRUCache<Tuple<Type, string>, FieldInfo> fieldInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, FieldInfo>((x,_) => {
                var ret = (x.Item1).GetField(x.Item2, BindingFlags.Public | BindingFlags.Instance);
                return ret;
            }, 15 /*items*/);

        static MemoizingMRUCache<Tuple<Type, string>, PropertyInfo> propInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, PropertyInfo>(
                (x, _) => (x.Item1).GetProperty(x.Item2), 
                15 /*items*/);
    #else
        static readonly MemoizingMRUCache<Tuple<Type, string>, FieldInfo> backingFieldInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, FieldInfo>((x, _) => {
                var fieldName = RxApp.GetFieldNameForProperty(x.Item2);
                var ret = (x.Item1).GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                return ret;
            }, 50/*items*/);

        static readonly MemoizingMRUCache<Tuple<Type, string>, FieldInfo> fieldInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, FieldInfo>((x,_) => {
                var ret = (x.Item1).GetField(x.Item2, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                return ret;
            }, 50/*items*/);

        static readonly MemoizingMRUCache<Tuple<Type, string>, PropertyInfo> propInfoTypeCache = 
            new MemoizingMRUCache<Tuple<Type,string>, PropertyInfo>((x,_) => {
                var ret = (x.Item1).GetProperty(x.Item2, BindingFlags.Public | BindingFlags.Instance);
                return ret;
            }, 50/*items*/);
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

        public static PropertyInfo GetPropertyInfoForProperty<TObj>(string propName)
        {
            return GetPropertyInfoForProperty(typeof (TObj), propName);
        }

        public static PropertyInfo GetPropertyInfoForProperty(Type type, string propName)
        {
            Contract.Requires(propName != null);
            PropertyInfo pi;

            lock(propInfoTypeCache) {
                pi = propInfoTypeCache.Get(new Tuple<Type,string>(type, propName));
            }

            return pi;
        }

        public static FieldInfo GetFieldInfoForField<TObj>(string propName)
        {
            return GetFieldInfoForField(typeof (TObj), propName);
        }

        public static FieldInfo GetFieldInfoForField(Type type, string propName)
        {
            Contract.Requires(propName != null);
            FieldInfo fi;

            lock(fieldInfoTypeCache) {
                fi = fieldInfoTypeCache.Get(new Tuple<Type,string>(type, propName));
            }

            return fi;
        }

        public static PropertyInfo GetPropertyInfoOrThrow(Type type, string propName)
        {
            var ret = GetPropertyInfoForProperty(type, propName);
            if (ret == null) {
                throw new ArgumentException(String.Format("Type '{0}' must have a property '{1}'", type, propName));
            }
            return ret;
        }

        public static bool TryGetValueForPropertyChain<TValue>(out TValue changeValue, object current, string[] propNames)
        {
            PropertyInfo pi;
            FieldInfo fi;
            Type currentType;

            foreach (var propName in propNames.SkipLast(1)) {
                if (current == null) {
                    changeValue = default(TValue);
                    return false;
                }

                currentType = current.GetType();
                fi = GetFieldInfoForField(currentType, propName);
                if (fi != null) {
                    current = fi.GetValue(current);
                    continue;
                }

                pi = GetPropertyInfoOrThrow(currentType, propName);
                current = pi.GetValue(current, null);
            }

            if (current == null) {
                changeValue = default(TValue);
                return false;
            }

            currentType = current.GetType();
            fi = GetFieldInfoForField(currentType, propNames.Last());
            if (fi != null) {
                changeValue = (TValue)fi.GetValue(current);
                return true;
            }

            pi = GetPropertyInfoOrThrow(current.GetType(), propNames.Last());
            changeValue = (TValue) pi.GetValue(current, null);
            return true;
        }

        public static bool SetValueToPropertyChain<TValue>(object target, string[] propNames, TValue value, bool shouldThrow = true)
        {
            PropertyInfo pi;
            FieldInfo fi;
            Type type;

            foreach (var propName in propNames.SkipLast(1)) {
                type = target.GetType();

                fi = GetFieldInfoForField(type, propName);
                if (fi != null) {
                    target = fi.GetValue(target);
                    continue;
                }

                pi = shouldThrow ? 
                    GetPropertyInfoOrThrow(type, propName) :
                    GetPropertyInfoForProperty(type, propName);

                if (pi == null) {
                    return false;
                }
                target = pi.GetValue(target, null);
            }

            type = target.GetType();
            fi = GetFieldInfoForField(type, propNames.Last());

            if (fi != null) {
                fi.SetValue(target, value);
                return true;
            }

            pi = shouldThrow ? 
                GetPropertyInfoOrThrow(target.GetType(), propNames.Last()) :
                GetPropertyInfoForProperty(target.GetType(), propNames.Last());

            if (pi == null) return false;

            pi.SetValue(target, value, null);
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
    }
}