using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using ReactiveUI;
using Splat;

namespace ReactiveUI
{
    public static class Reflection 
    {
        static ExpressionRewriter expressionRewriter = new ExpressionRewriter();        
        
        public static Expression Rewrite(Expression expression)
        {
            return expressionRewriter.Visit(expression);
        }

        public static string ExpressionToPropertyNames(Expression expression)
        {
            Contract.Requires(expression != null);

            StringBuilder sb = new StringBuilder();

            foreach (var exp in expression.GetExpressionChain())
            {
                if (exp.NodeType != ExpressionType.Parameter)
                {
                    // Indexer expression
                    if (exp.NodeType == ExpressionType.Index)
                    {
                        var ie = (IndexExpression)exp;
                        sb.Append(ie.Indexer.Name);
                        sb.Append('[');
                        foreach (var argument in ie.Arguments)
                        {
                            sb.Append(((ConstantExpression)argument).Value);
                            sb.Append(',');
                        }
                        sb.Replace(',', ']', sb.Length - 1, 1);
                    }
                    else if (exp.NodeType == ExpressionType.MemberAccess)
                    {
                        var me = (MemberExpression)exp;
                        sb.Append(me.Member.Name);
                    }
                }
                sb.Append('.');
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static Func<object, object[], object> GetValueFetcherForProperty(Expression expression)
        {
            Contract.Requires(expression != null);

            MemberInfo member = expression.GetMemberInfo();

            FieldInfo field = member as FieldInfo;
            if (field != null)
            {
                return (obj, args) => field.GetValue(obj);
            }
            PropertyInfo property = member as PropertyInfo;
            if (property != null)
            {
                return property.GetValue;
            }

            return null;
        }

        public static Func<object, object[], object> GetValueFetcherOrThrow(Expression expression)
        {
            var ret = GetValueFetcherForProperty(expression);

            if (ret == null)
            {
                MemberInfo member = expression.GetMemberInfo();
                throw new ArgumentException(String.Format("Type '{0}' must have a property '{1}'", member.DeclaringType, member.Name));
            }
            return ret;
        }

        public static Action<object, object, object[]> GetValueSetterForProperty(Expression expression)
        {
            Contract.Requires(expression != null);

            MemberInfo member = expression.GetMemberInfo();

            FieldInfo field = member as FieldInfo;
            if(field != null)
            {
                return (obj, val, args) => field.SetValue(obj, val);
            }
            PropertyInfo property = member as PropertyInfo;
            if (property != null)
            {
                return property.SetValue;
            }

            return null;
        }

        public static Action<object, object, object[]> GetValueSetterOrThrow(Expression expression)
        {
            var ret = GetValueSetterForProperty(expression);

            if (ret == null) {
                MemberInfo member = expression.GetMemberInfo();
                throw new ArgumentException(String.Format("Type '{0}' must have a property '{1}'", member.DeclaringType, member.Name));
            }
            return ret;
        }

        public static bool TryGetValueForPropertyChain<TValue>(out TValue changeValue, object current, IEnumerable<Expression> expressionChain)
        {
            foreach (var expression in expressionChain.SkipLast(1)) {
                if (current == null) {
                    changeValue = default(TValue);
                    return false;
                }

                current = GetValueFetcherOrThrow(expression)(current, null);
            }

            if (current == null) {
                changeValue = default(TValue);
                return false;
            }

            Expression lastExpression = expressionChain.Last();
            changeValue = (TValue) GetValueFetcherOrThrow(lastExpression)(current, null);
            return true;
        }

        public static bool TryGetAllValuesForPropertyChain(out IObservedChange<object, object>[] changeValues, object current, IEnumerable<Expression> expressionChain)
        {
            int currentIndex = 0;
            changeValues = new IObservedChange<object,object>[expressionChain.Count()];

            foreach (var expression in expressionChain.SkipLast(1)) {
                if (current == null) {
                    changeValues[currentIndex] = null;
                    return false;
                }

                var sender = current;
                current = GetValueFetcherOrThrow(expression)(current, null);
                var box = new ObservedChange<object, object>(sender, expression, current);

                changeValues[currentIndex] = box;
                currentIndex++;
            }

            if (current == null) {
                changeValues[currentIndex] = null;
                return false;
            }

            Expression lastExpression = expressionChain.Last();
            changeValues[currentIndex] = new ObservedChange<object, object>(current, lastExpression, GetValueFetcherOrThrow(lastExpression)(current, null));

            return true;
        }

        public static bool TrySetValueToPropertyChain<TValue>(object target, IEnumerable<Expression> expressionChain, TValue value, bool shouldThrow = true)
        {
            foreach (var expression in expressionChain.SkipLast(1)) {
                var getter = shouldThrow ?
                    GetValueFetcherOrThrow(expression) :
                    GetValueFetcherForProperty(expression);

                target = getter(target, null);
            }

            if (target == null) return false;

            Expression lastExpression = expressionChain.Last();
            var setter = shouldThrow ?
                GetValueSetterOrThrow(lastExpression) :
                GetValueSetterForProperty(lastExpression);

            if (setter == null) return false;
            setter(target, value, null);
            return true;
        }

        static readonly MemoizingMRUCache<string, Type> typeCache = new MemoizingMRUCache<string, Type>((type,_) => {
            return Type.GetType(type, false);
        }, 20);

        public static Type ReallyFindType(string type, bool throwOnFailure) 
        {
            lock (typeCache) {
                var ret = typeCache.Get(type);
                if (ret != null || !throwOnFailure) return ret;
                throw new TypeLoadException();
            }
        }
    
        public static Type GetEventArgsTypeForEvent(Type type, string eventName)
        {
            var ti = type;
            var ei = ti.GetRuntimeEvent(eventName);
            if (ei == null) {
                throw new Exception(String.Format("Couldn't find {0}.{1}", type.FullName, eventName));
            }
    
            // Find the EventArgs type parameter of the event via digging around via reflection
            var eventArgsType = ei.EventHandlerType.GetRuntimeMethods().First(x => x.Name == "Invoke").GetParameters()[1].ParameterType;
            return eventArgsType;
        }

        internal static IObservable<object> ViewModelWhenAnyValue<TView, TViewModel>(TViewModel viewModel, TView view, Expression expression)
            where TView : IViewFor
            where TViewModel : class
        {
            return view.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => ((TViewModel)x).WhenAnyDynamic(expression, y => y.Value))
                .Switch();
        }

        internal static Expression getViewExpression(object view, Expression vmExpression)
        {
            var controlProperty = (MemberInfo)view.GetType().GetRuntimeField(vmExpression.GetMemberInfo().Name)
                ?? view.GetType().GetRuntimeProperty(vmExpression.GetMemberInfo().Name);
            if (controlProperty == null)
            {
                throw new Exception(String.Format("Tried to bind to control but it wasn't present on the object: {0}.{1}",
                    view.GetType().FullName, vmExpression.GetMemberInfo().Name));
            }

            return Expression.MakeMemberAccess(Expression.Parameter(view.GetType()), controlProperty);
        }

        internal static Expression getViewExpressionWithProperty(object view, Expression vmExpression)
        {
            var controlExpression = getViewExpression(view, vmExpression);

            var control = GetValueFetcherForProperty(controlExpression)(view, null);
            if (control == null)
            {
                throw new Exception(String.Format("Tried to bind to control but it was null: {0}.{1}", view.GetType().FullName,
                    controlExpression.GetMemberInfo().Name));
            }

            var defaultProperty = DefaultPropertyBinding.GetPropertyForControl(control);
            if (defaultProperty == null)
            {
                throw new Exception(String.Format("Couldn't find a default property for type {0}", control.GetType()));
            }
            return Expression.MakeMemberAccess(controlExpression, control.GetType().GetRuntimeProperty(defaultProperty));
        }
    }

    public static class ReflectionExtensions
    {
        public static bool IsStatic(this PropertyInfo This)
        {
            return (This.GetMethod ?? This.SetMethod).IsStatic;
        }
    }
}
