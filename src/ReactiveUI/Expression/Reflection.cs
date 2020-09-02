﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;

using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Helper class for handling Reflection amd Expression tree related items.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "nullable object array.")]
    public static class Reflection
    {
        private static readonly ExpressionRewriter expressionRewriter = new ExpressionRewriter();

        private static readonly MemoizingMRUCache<string, Type?> _typeCache = new MemoizingMRUCache<string, Type?>(
            (type, _) =>
            {
                return Type.GetType(
                                    type,
                                    assemblyName =>
                                    {
                                        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(z => z.FullName == assemblyName.FullName);
                                        if (assembly != null)
                                        {
                                            return assembly;
                                        }

                                        try
                                        {
                                            return Assembly.Load(assemblyName);
                                        }
                                        catch
                                        {
                                            return null;
                                        }
                                    },
                                    null,
                                    false);
            },
            20);

        /// <summary>
        /// Uses the expression re-writer to simplify the Expression down to it's simplest Expression.
        /// </summary>
        /// <param name="expression">The expression to rewrite.</param>
        /// <returns>The rewritten expression.</returns>
        public static Expression Rewrite(Expression expression)
        {
            return expressionRewriter.Visit(expression);
        }

        /// <summary>
        /// Will convert a Expression which points towards a property
        /// to a string containing the property names.
        /// The sub-properties will be separated by the '.' character.
        /// Index based values will include [] after the name.
        /// </summary>
        /// <param name="expression">The expression to generate the property names from.</param>
        /// <returns>A string form for the property the expression is pointing to.</returns>
        public static string ExpressionToPropertyNames(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            StringBuilder sb = new StringBuilder();

            foreach (Expression exp in expression.GetExpressionChain())
            {
                if (exp.NodeType != ExpressionType.Parameter)
                {
                    // Indexer expression
                    if (exp.NodeType == ExpressionType.Index)
                    {
                        var ie = (IndexExpression)exp;
                        sb.Append(ie.Indexer.Name).Append('[');

                        foreach (var argument in ie.Arguments)
                        {
                            sb.Append(((ConstantExpression)argument).Value).Append(',');
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

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a MemberInfo into a Func which will fetch the value for the Member.
        /// Handles either fields or properties.
        /// </summary>
        /// <param name="member">The member info to convert.</param>
        /// <returns>A Func that takes in the object/indexes and returns the value.</returns>
        public static Func<object, object[]?, object?>? GetValueFetcherForProperty(MemberInfo member)
        {
            Contract.Requires(member != null);

            FieldInfo? field = member as FieldInfo;
            if (field != null)
            {
                return (obj, _) => field.GetValue(obj) ?? throw new InvalidOperationException();
            }

            PropertyInfo? property = member as PropertyInfo;
            if (property != null)
            {
                return property.GetValue;
            }

            return null;
        }

        /// <summary>
        /// Converts a MemberInfo into a Func which will fetch the value for the Member.
        /// Handles either fields or properties.
        /// If there is no field or property with the matching MemberInfo it'll throw
        /// an ArgumentException.
        /// </summary>
        /// <param name="member">The member info to convert.</param>
        /// <returns>A Func that takes in the object/indexes and returns the value.</returns>
        public static Func<object, object[]?, object?> GetValueFetcherOrThrow(MemberInfo member)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            var ret = GetValueFetcherForProperty(member);

            if (ret == null)
            {
                throw new ArgumentException($"Type '{member.DeclaringType}' must have a property '{member.Name}'");
            }

            return ret;
        }

        /// <summary>
        /// Converts a MemberInfo into a Func which will set the value for the Member.
        /// Handles either fields or properties.
        /// If there is no field or property with the matching MemberInfo it'll throw
        /// an ArgumentException.
        /// </summary>
        /// <param name="member">The member info to convert.</param>
        /// <returns>A Func that takes in the object/indexes and sets the value.</returns>
        public static Action<object, object?, object[]?> GetValueSetterForProperty(MemberInfo member)
        {
            Contract.Requires(member != null);

            FieldInfo? field = member as FieldInfo;
            if (field != null)
            {
                return (obj, val, _) => field.SetValue(obj, val);
            }

            PropertyInfo? property = member as PropertyInfo;
            if (property != null)
            {
                return property.SetValue;
            }

            return null!;
        }

        /// <summary>
        /// Converts a MemberInfo into a Func which will set the value for the Member.
        /// Handles either fields or properties.
        /// If there is no field or property with the matching MemberInfo it'll throw
        /// an ArgumentException.
        /// </summary>
        /// <param name="member">The member info to convert.</param>
        /// <returns>A Func that takes in the object/indexes and sets the value.</returns>
        public static Action<object, object?, object[]?>? GetValueSetterOrThrow(MemberInfo member)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            var ret = GetValueSetterForProperty(member);

            if (ret == null)
            {
                throw new ArgumentException($"Type '{member.DeclaringType}' must have a property '{member.Name}'");
            }

            return ret;
        }

        /// <summary>
        /// Based on a list of Expressions get the value of the last property in the chain if possible.
        /// The Expressions are typically property chains. Eg Property1.Property2.Property3
        /// The method will make sure that each Expression can get a value along the way
        /// and get each property until each expression is evaluated.
        /// </summary>
        /// <param name="changeValue">A output value where to store the value if the value can be fetched.</param>
        /// <param name="current">The object that starts the property chain.</param>
        /// <param name="expressionChain">A list of expressions which will point towards a property or field.</param>
        /// <typeparam name="TValue">The type of the end value we are trying to get.</typeparam>
        /// <returns>If the value was successfully retrieved or not.</returns>
        public static bool TryGetValueForPropertyChain<TValue>(out TValue changeValue, object? current, IEnumerable<Expression> expressionChain)
        {
            var expressions = expressionChain.ToList();
            foreach (Expression expression in expressions.SkipLast(1))
            {
                if (current == null)
                {
                    changeValue = default!;
                    return false;
                }

                current = GetValueFetcherOrThrow(expression.GetMemberInfo())(current, expression.GetArgumentsArray());
            }

            if (current == null)
            {
                changeValue = default!;
                return false;
            }

            Expression lastExpression = expressions.Last();
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8605 // Unboxing a possibly null value.
            changeValue = (TValue)GetValueFetcherOrThrow(lastExpression.GetMemberInfo())(current, lastExpression.GetArgumentsArray());
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning restore CS8601 // Possible null reference assignment.
            return true;
        }

        /// <summary>
        /// Based on a list of Expressions get a IObservedChanged for the value
        /// of the last property in the chain if possible.
        /// The Expressions are property chains. Eg Property1.Property2.Property3
        /// The method will make sure that each Expression can get a value along the way
        /// and get each property until each expression is evaluated.
        /// </summary>
        /// <param name="changeValues">A IObservedChanged for the value.</param>
        /// <param name="current">The object that starts the property chain.</param>
        /// <param name="expressionChain">A list of expressions which will point towards a property or field.</param>
        /// <returns>If the value was successfully retrieved or not.</returns>
        public static bool TryGetAllValuesForPropertyChain(out IObservedChange<object, object?>[] changeValues, object? current, IEnumerable<Expression> expressionChain)
        {
            int currentIndex = 0;
            var expressions = expressionChain.ToList();
            changeValues = new IObservedChange<object, object>[expressions.Count];

            foreach (Expression expression in expressions.SkipLast(1))
            {
                if (current == null)
                {
                    changeValues[currentIndex] = null!;
                    return false;
                }

                var sender = current;
                current = GetValueFetcherOrThrow(expression.GetMemberInfo())(current, expression.GetArgumentsArray());
#pragma warning disable CS8604 // Possible null reference argument.
                changeValues[currentIndex] = new ObservedChange<object, object>(sender, expression, current);
#pragma warning restore CS8604 // Possible null reference argument.
                currentIndex++;
            }

            if (current == null)
            {
                changeValues[currentIndex] = null!;
                return false;
            }

            Expression lastExpression = expressions.Last();
#pragma warning disable CS8604 // Possible null reference argument.
            changeValues[currentIndex] = new ObservedChange<object, object>(current, lastExpression, GetValueFetcherOrThrow(lastExpression.GetMemberInfo())(current, lastExpression.GetArgumentsArray()));
#pragma warning restore CS8604 // Possible null reference argument.

            return true;
        }

        /// <summary>
        /// Based on a list of Expressions set a value
        /// of the last property in the chain if possible.
        /// The Expressions are property chains. Eg Property1.Property2.Property3
        /// The method will make sure that each Expression can use each value along the way
        /// and set the last value.
        /// </summary>
        /// <param name="target">The object that starts the property chain.</param>
        /// <param name="expressionChain">A list of expressions which will point towards a property or field.</param>
        /// <param name="value">The value to set on the last property in the Expression chain.</param>
        /// <param name="shouldThrow">If we should throw if we are unable to set the value.</param>
        /// <typeparam name="TValue">The type of the end value we are trying to set.</typeparam>
        /// <returns>If the value was successfully retrieved or not.</returns>
        public static bool TrySetValueToPropertyChain<TValue>(object? target, IEnumerable<Expression> expressionChain, TValue value, bool shouldThrow = true)
        {
            var expressions = expressionChain.ToList();
            foreach (Expression expression in expressions.SkipLast(1))
            {
                var getter = shouldThrow ?
                    GetValueFetcherOrThrow(expression.GetMemberInfo()) :
                    GetValueFetcherForProperty(expression.GetMemberInfo());

                if (getter != null)
                {
                    target = getter(target ?? throw new ArgumentNullException(nameof(target)), expression.GetArgumentsArray());
                }
            }

            if (target == null)
            {
                return false;
            }

            Expression lastExpression = expressions.Last();
            Action<object, object?, object[]?>? setter = shouldThrow ?
                GetValueSetterOrThrow(lastExpression.GetMemberInfo()) :
                GetValueSetterForProperty(lastExpression.GetMemberInfo());

            if (setter == null)
            {
                return false;
            }

            setter(target, value, lastExpression.GetArgumentsArray());
            return true;
        }

        /// <summary>
        /// Gets a Type from the specified type name.
        /// Uses a cache to avoid having to use Reflection every time.
        /// </summary>
        /// <param name="type">The name of the type.</param>
        /// <param name="throwOnFailure">If we should throw an exception if the type can't be found.</param>
        /// <returns>The type that was found or null.</returns>
        /// <exception cref="TypeLoadException">If we were unable to find the type.</exception>
        public static Type? ReallyFindType(string? type, bool throwOnFailure)
        {
            Type? ret = _typeCache.Get(type ?? string.Empty);
            if (ret != null || !throwOnFailure)
            {
                return ret;
            }

            throw new TypeLoadException();
        }

        /// <summary>
        /// Gets the appropriate EventArgs derived object for the specified event name for a Type.
        /// </summary>
        /// <param name="type">The type of object to find the event on.</param>
        /// <param name="eventName">The mame of the event.</param>
        /// <returns>The Type of the EventArgs to use.</returns>
        /// <exception cref="Exception">If there is no event matching the name on the target type.</exception>
        public static Type GetEventArgsTypeForEvent(Type type, string eventName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type ti = type;
            EventInfo? ei = ti.GetRuntimeEvent(eventName);
            if (ei == null || ei.EventHandlerType == null)
            {
                throw new Exception($"Couldn't find {type.FullName}.{eventName}");
            }

            // Find the EventArgs type parameter of the event via digging around via reflection
            return ei.EventHandlerType.GetRuntimeMethods().First(x => x.Name == "Invoke").GetParameters()[1].ParameterType;
        }

        /// <summary>
        /// Checks to make sure that the specified method names on the target object
        /// are overriden.
        /// </summary>
        /// <param name="callingTypeName">The name of the calling type.</param>
        /// <param name="targetObject">The object to check.</param>
        /// <param name="methodsToCheck">The name of the methods to check.</param>
        /// <exception cref="Exception">Thrown if the methods aren't overriden on the target object.</exception>
        public static void ThrowIfMethodsNotOverloaded(string callingTypeName, object targetObject, params string[] methodsToCheck)
        {
            var missingMethod = methodsToCheck
                .Select(x =>
                {
                    IEnumerable<MethodInfo> methods = targetObject.GetType().GetTypeInfo().DeclaredMethods;
                    return (methodName: x, methodImplementation: methods.FirstOrDefault(y => y.Name == x));
                })
                .FirstOrDefault(x => x.methodImplementation == null);

            if (missingMethod.methodName != default)
            {
                throw new Exception($"Your class must implement {missingMethod.methodName} and call {callingTypeName}.{missingMethod.methodName}");
            }
        }

        /// <summary>
        /// Determines if the specified property is static or not.
        /// </summary>
        /// <param name="item">The property information to check.</param>
        /// <returns>If the property is static or not.</returns>
        public static bool IsStatic(this PropertyInfo item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var method = (item.GetMethod ?? item.SetMethod) !;
            return method.IsStatic;
        }

        [SuppressMessage("Microsoft.Performance", "CA1801", Justification = "TViewModel used to help generic calling.")]
        internal static IObservable<object> ViewModelWhenAnyValue<TView, TViewModel>(TViewModel? viewModel, TView view, Expression expression)
            where TView : class, IViewFor
            where TViewModel : class
        {
            return view.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Select(x => ((TViewModel)x!).WhenAnyDynamic(expression, y => y.Value))
                .Switch();
        }
    }
}
