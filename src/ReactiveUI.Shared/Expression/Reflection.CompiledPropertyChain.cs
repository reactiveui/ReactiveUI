// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>Compiled property-chain helpers used by <see cref="Reflection"/> to move member resolution out of hot paths.</summary>
public static partial class Reflection
{
    /// <summary>Pre-compiled property chain that caches getter delegates and indexer arguments.</summary>
    /// <typeparam name="TSource">The root type expected by the expression chain.</typeparam>
    /// <typeparam name="TValue">The final value type.</typeparam>
    /// <remarks>
    /// <para>
    /// This type exists to move expression-chain enumeration and member resolution out of observable hot paths.
    /// After construction, <see cref="TryGetValue"/> and <see cref="TryGetAllValues"/> execute using cached delegates.
    /// </para>
    /// <para>
    /// Trimming note: constructing this type typically involves expression parsing and may be trimming-sensitive;
    /// callers should treat construction as the “reflection boundary”.
    /// </para>
    /// </remarks>
    internal sealed class CompiledPropertyChain<TSource, TValue>
    {
        /// <summary>Cached getter delegates for each expression step.</summary>
        private readonly Func<object?, object?[]?, object?>[] _getters;

        /// <summary>Cached argument arrays for each expression step (indexers); entries may be <see langword="null"/>.</summary>
        private readonly object?[]?[] _arguments;

        /// <summary>Cached expressions for each step, used for constructing observed changes.</summary>
        private readonly Expression[] _expressions;

        /// <summary>Initializes a new instance of the <see cref="CompiledPropertyChain{TSource, TValue}"/> class.</summary>
        /// <param name="expressionChain">The expression chain to compile. Must contain at least one expression.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expressionChain"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="expressionChain"/> is empty.</exception>
        public CompiledPropertyChain(Expression[] expressionChain)
        {
            ArgumentExceptionHelper.ThrowIfNull(expressionChain);

            if (expressionChain.Length == 0)
            {
                throw new InvalidOperationException(EmptyExpressionChainMessage);
            }

            _expressions = expressionChain;
            _getters = new Func<object?, object?[]?, object?>[expressionChain.Length];
            _arguments = new object?[]?[expressionChain.Length];

            for (var i = 0; i < expressionChain.Length; i++)
            {
                var expr = expressionChain[i];
                _getters[i] = GetValueFetcherOrThrow(expr.GetMemberInfo());
                _arguments[i] = expr.GetArgumentsArray();
            }
        }

        /// <summary>Attempts to get the final value from the property chain.</summary>
        /// <param name="source">The root object.</param>
        /// <param name="value">Receives the final value when successful.</param>
        /// <returns><see langword="true"/> if successful; otherwise <see langword="false"/>.</returns>
        internal bool TryGetValue(TSource? source, out TValue value)
        {
            object? current = source;
            var lastIndex = _getters.Length - 1;

            for (var i = 0; i < lastIndex; i++)
            {
                if (current is null)
                {
                    value = default!;
                    return false;
                }

                current = _getters[i](current, _arguments[i]);
            }

            if (current is null)
            {
                value = default!;
                return false;
            }

            value = (TValue)_getters[lastIndex](current, _arguments[lastIndex])!;
            return true;
        }

        /// <summary>Attempts to get all intermediate values in the property chain as observed changes.</summary>
        /// <param name="source">The root object.</param>
        /// <param name="changeValues">Receives an array with one entry per expression in the chain.</param>
        /// <returns><see langword="true"/> if successful; otherwise <see langword="false"/>.</returns>
        /// <remarks>
        /// Mirrors <see cref="TryGetAllValuesForPropertyChain"/> behavior: on early failure at index <c>i</c>,
        /// writes <c>changeValues[i] = null!</c> and returns <see langword="false"/>.
        /// </remarks>
        internal bool TryGetAllValues(TSource? source, out IObservedChange<object, object?>[] changeValues)
        {
            var count = _expressions.Length;
            changeValues = new IObservedChange<object, object?>[count];

            object? current = source;
            var lastIndex = count - 1;

            for (var i = 0; i < lastIndex; i++)
            {
                if (current is null)
                {
                    changeValues[i] = null!;
                    return false;
                }

                var sender = current;
                current = _getters[i](current, _arguments[i]);
                changeValues[i] = new ObservedChange<object, object?>(sender, _expressions[i], current);
            }

            if (current is null)
            {
                changeValues[lastIndex] = null!;
                return false;
            }

            changeValues[lastIndex] = new ObservedChange<object, object?>(
                current,
                _expressions[lastIndex],
                _getters[lastIndex](current, _arguments[lastIndex]));

            return true;
        }
    }

    /// <summary>Pre-compiled setter for a property chain.</summary>
    /// <typeparam name="TSource">The root type expected by the expression chain.</typeparam>
    /// <typeparam name="TValue">The value type to set.</typeparam>
    /// <remarks>
    /// <para>
    /// This type is designed for binding hot paths: traversal and setter invocation is performed using cached delegates.
    /// It does not synthesize <see cref="NullReferenceException"/> for intermediate nulls; it follows “Try*” semantics.
    /// </para>
    /// <para>
    /// Trimming note: construction is typically the “reflection boundary”.
    /// </para>
    /// </remarks>
    internal sealed class CompiledPropertyChainSetter<TSource, TValue>
    {
        /// <summary>Cached getter delegates used to walk from the root to the parent of the final member.</summary>
        private readonly Func<object?, object?[]?, object?>[] _parentGetters;

        /// <summary>Cached argument arrays for each parent step (indexers); entries may be <see langword="null"/>.</summary>
        private readonly object?[]?[] _parentArguments;

        /// <summary>Cached setter delegate for the final member; may be <see langword="null"/> if the member is not settable.</summary>
        private readonly Action<object?, object?, object?[]?>? _setter;

        /// <summary>Cached argument array for the final setter (indexer); may be <see langword="null"/>.</summary>
        private readonly object?[]? _setterArguments;

        /// <summary>Cached error message for throwing when the final member is not settable.</summary>
        private readonly string _unsettableMemberMessage;

        /// <summary>Initializes a new instance of the <see cref="CompiledPropertyChainSetter{TSource, TValue}"/> class.</summary>
        /// <param name="expressionChain">The expression chain to compile. Must contain at least one expression.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expressionChain"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="expressionChain"/> is empty.</exception>
        public CompiledPropertyChainSetter(Expression[] expressionChain)
        {
            ArgumentExceptionHelper.ThrowIfNull(expressionChain);

            switch (expressionChain.Length)
            {
                case 0:
                    throw new InvalidOperationException(EmptyExpressionChainMessage);
                case 1:
                    {
                        _parentGetters = [];
                        _parentArguments = [];
                        break;
                    }

                default:
                    {
                        var parentCount = expressionChain.Length - 1;
                        _parentGetters = new Func<object?, object?[]?, object?>[parentCount];
                        _parentArguments = new object?[]?[parentCount];

                        for (var i = 0; i < parentCount; i++)
                        {
                            var expr = expressionChain[i];
                            _parentGetters[i] = GetValueFetcherOrThrow(expr.GetMemberInfo());
                            _parentArguments[i] = expr.GetArgumentsArray();
                        }

                        break;
                    }
            }

            var lastExpr = expressionChain[^1];
            _setter = GetValueSetterForProperty(lastExpr.GetMemberInfo());
            _setterArguments = lastExpr.GetArgumentsArray();

            var member = lastExpr.GetMemberInfo();
            _unsettableMemberMessage = $"Type '{member?.DeclaringType}' must have a property '{member?.Name}'";
        }

        /// <summary>Attempts to set the value at the end of the property chain.</summary>
        /// <param name="source">The root object.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="shouldThrow">
        /// If <see langword="true"/>, throws for a null root and for an unsettable final member.
        /// If <see langword="false"/>, returns <see langword="false"/> when the chain cannot be navigated or set.
        /// </param>
        /// <returns><see langword="true"/> if the set succeeded; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null"/> and <paramref name="shouldThrow"/> is <see langword="true"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the final member is not settable and <paramref name="shouldThrow"/> is <see langword="true"/>.</exception>
        internal bool TrySetValue(TSource? source, TValue value, bool shouldThrow = true)
        {
            object? current = source;

            if (current is null)
            {
                if (shouldThrow)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                return false;
            }

            for (var i = 0; i < _parentGetters.Length; i++)
            {
                current = _parentGetters[i](current, _parentArguments[i]);
                if (current is null)
                {
                    return false;
                }
            }

            if (_setter is null)
            {
                if (shouldThrow)
                {
                    throw new ArgumentException(_unsettableMemberMessage);
                }

                return false;
            }

            _setter(current, value, _setterArguments);
            return true;
        }
    }
}
