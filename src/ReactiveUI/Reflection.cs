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
    /// <summary>
    /// Reflection class
    /// </summary>
    public static class Reflection
    {
        private static readonly MemoizingMRUCache<string, Type> typeCache = new MemoizingMRUCache<string, Type>((type, _) => {
            return Type.GetType(type, false);
        }, 20);

        private static ExpressionRewriter expressionRewriter = new ExpressionRewriter();

        /// <summary>
        /// Expressions to property names.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string ExpressionToPropertyNames(Expression expression)
        {
            Contract.Requires(expression != null);

            StringBuilder sb = new StringBuilder();

            foreach (var exp in expression.GetExpressionChain()) {
                if (exp.NodeType != ExpressionType.Parameter) {

                    // Indexer expression
                    if (exp.NodeType == ExpressionType.Index) {
                        var ie = (IndexExpression)exp;
                        sb.Append(ie.Indexer.Name);
                        sb.Append('[');

                        foreach (var argument in ie.Arguments) {
                            sb.Append(((ConstantExpression)argument).Value);
                            sb.Append(',');
                        }
                        sb.Replace(',', ']', sb.Length - 1, 1);
                    } else if (exp.NodeType == ExpressionType.MemberAccess) {
                        var me = (MemberExpression)exp;
                        sb.Append(me.Member.Name);
                    }
                }

                sb.Append('.');
            }

            if (sb.Length > 0) {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the event arguments type for event.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Type GetEventArgsTypeForEvent(Type type, string eventName)
        {
            var ti = type;
            var ei = ti.GetRuntimeEvent(eventName);
            if (ei == null) {
                throw new Exception(string.Format("Couldn't find {0}.{1}", type.FullName, eventName));
            }

            // Find the EventArgs type parameter of the event via digging around via reflection
            var eventArgsType = ei.EventHandlerType.GetRuntimeMethods().First(x => x.Name == "Invoke").GetParameters()[1].ParameterType;
            return eventArgsType;
        }

        /// <summary>
        /// Gets the value fetcher for property.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static Func<object, object[], object> GetValueFetcherForProperty(MemberInfo member)
        {
            Contract.Requires(member != null);

            FieldInfo field = member as FieldInfo;
            if (field != null) {
                return (obj, args) => field.GetValue(obj);
            }

            PropertyInfo property = member as PropertyInfo;
            if (property != null) {
                return property.GetValue;
            }

            return null;
        }

        /// <summary>
        /// Gets the value fetcher or throw.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static Func<object, object[], object> GetValueFetcherOrThrow(MemberInfo member)
        {
            var ret = GetValueFetcherForProperty(member);

            if (ret == null) {
                throw new ArgumentException(string.Format("Type '{0}' must have a property '{1}'", member.DeclaringType, member.Name));
            }
            return ret;
        }

        /// <summary>
        /// Gets the value setter for property.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static Action<object, object, object[]> GetValueSetterForProperty(MemberInfo member)
        {
            Contract.Requires(member != null);

            FieldInfo field = member as FieldInfo;
            if (field != null) {
                return (obj, val, args) => field.SetValue(obj, val);
            }

            PropertyInfo property = member as PropertyInfo;
            if (property != null) {
                return property.SetValue;
            }

            return null;
        }

        /// <summary>
        /// Gets the value setter or throw.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static Action<object, object, object[]> GetValueSetterOrThrow(MemberInfo member)
        {
            var ret = GetValueSetterForProperty(member);

            if (ret == null) {
                throw new ArgumentException(string.Format("Type '{0}' must have a property '{1}'", member.DeclaringType, member.Name));
            }
            return ret;
        }

        /// <summary>
        /// Reallies the type of the find.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="throwOnFailure">if set to <c>true</c> [throw on failure].</param>
        /// <returns></returns>
        /// <exception cref="System.TypeLoadException"></exception>
        public static Type ReallyFindType(string type, bool throwOnFailure)
        {
            lock (typeCache) {
                var ret = typeCache.Get(type);
                if (ret != null || !throwOnFailure) return ret;
                throw new TypeLoadException();
            }
        }

        /// <summary>
        /// Rewrites the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Expression Rewrite(Expression expression)
        {
            return expressionRewriter.Visit(expression);
        }

        /// <summary>
        /// Throws if methods not overloaded.
        /// </summary>
        /// <param name="callingTypeName">Name of the calling type.</param>
        /// <param name="targetObject">The target object.</param>
        /// <param name="methodsToCheck">The methods to check.</param>
        /// <exception cref="System.Exception"></exception>
        public static void ThrowIfMethodsNotOverloaded(string callingTypeName, object targetObject, params string[] methodsToCheck)
        {
            var missingMethod = methodsToCheck
                .Select(x => {
                    var methods = targetObject.GetType().GetTypeInfo().DeclaredMethods;
                    return Tuple.Create(x, methods.FirstOrDefault(y => y.Name == x));
                })
                .FirstOrDefault(x => x.Item2 == null);

            if (missingMethod != null) {
                throw new Exception(string.Format("Your class must implement {0} and call {1}.{0}", missingMethod.Item1, callingTypeName));
            }
        }

        /// <summary>
        /// Tries the get all values for property chain.
        /// </summary>
        /// <param name="changeValues">The change values.</param>
        /// <param name="current">The current.</param>
        /// <param name="expressionChain">The expression chain.</param>
        /// <returns></returns>
        public static bool TryGetAllValuesForPropertyChain(out IObservedChange<object, object>[] changeValues, object current, IEnumerable<Expression> expressionChain)
        {
            int currentIndex = 0;
            changeValues = new IObservedChange<object, object>[expressionChain.Count()];

            foreach (var expression in expressionChain.SkipLast(1)) {
                if (current == null) {
                    changeValues[currentIndex] = null;
                    return false;
                }

                var sender = current;
                current = GetValueFetcherOrThrow(expression.GetMemberInfo())(current, expression.GetArgumentsArray());
                var box = new ObservedChange<object, object>(sender, expression, current);

                changeValues[currentIndex] = box;
                currentIndex++;
            }

            if (current == null) {
                changeValues[currentIndex] = null;
                return false;
            }

            Expression lastExpression = expressionChain.Last();
            changeValues[currentIndex] = new ObservedChange<object, object>(current, lastExpression, GetValueFetcherOrThrow(lastExpression.GetMemberInfo())(current, lastExpression.GetArgumentsArray()));

            return true;
        }

        /// <summary>
        /// Tries the get value for property chain.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="changeValue">The change value.</param>
        /// <param name="current">The current.</param>
        /// <param name="expressionChain">The expression chain.</param>
        /// <returns></returns>
        public static bool TryGetValueForPropertyChain<TValue>(out TValue changeValue, object current, IEnumerable<Expression> expressionChain)
        {
            foreach (var expression in expressionChain.SkipLast(1)) {
                if (current == null) {
                    changeValue = default(TValue);
                    return false;
                }

                current = GetValueFetcherOrThrow(expression.GetMemberInfo())(current, expression.GetArgumentsArray());
            }

            if (current == null) {
                changeValue = default(TValue);
                return false;
            }

            Expression lastExpression = expressionChain.Last();
            changeValue = (TValue)GetValueFetcherOrThrow(lastExpression.GetMemberInfo())(current, lastExpression.GetArgumentsArray());
            return true;
        }

        /// <summary>
        /// Tries the set value to property chain.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="expressionChain">The expression chain.</param>
        /// <param name="value">The value.</param>
        /// <param name="shouldThrow">if set to <c>true</c> [should throw].</param>
        /// <returns></returns>
        public static bool TrySetValueToPropertyChain<TValue>(object target, IEnumerable<Expression> expressionChain, TValue value, bool shouldThrow = true)
        {
            foreach (var expression in expressionChain.SkipLast(1)) {
                var getter = shouldThrow ?
                    GetValueFetcherOrThrow(expression.GetMemberInfo()) :
                    GetValueFetcherForProperty(expression.GetMemberInfo());

                target = getter(target, expression.GetArgumentsArray());
            }

            if (target == null) return false;

            Expression lastExpression = expressionChain.Last();
            var setter = shouldThrow ?
                GetValueSetterOrThrow(lastExpression.GetMemberInfo()) :
                GetValueSetterForProperty(lastExpression.GetMemberInfo());

            if (setter == null) return false;
            setter(target, value, lastExpression.GetArgumentsArray());
            return true;
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
    }

    /// <summary>
    /// Reflection Extensions
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Determines whether this instance is static.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <returns><c>true</c> if the specified this is static; otherwise, <c>false</c>.</returns>
        public static bool IsStatic(this PropertyInfo This)
        {
            return (This.GetMethod ?? This.SetMethod).IsStatic;
        }
    }
}