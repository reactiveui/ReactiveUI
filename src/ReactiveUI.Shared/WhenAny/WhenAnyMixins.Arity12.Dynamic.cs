// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides the arity-12 WhenAnyDynamic extension overloads.</summary>
[SuppressMessage(
    "Design",
    "SST1472:Method declares too many parameters",
    Justification = "Parameter count is intrinsic to the fixed WhenAny arity API.")]
public static partial class WhenAnyMixins
{
    /// <summary>Provides the arity-12 WhenAnyDynamic extension members for an observed source object.</summary>
    /// <typeparam name="TSender">The type of the source object.</typeparam>
    /// <param name="sender">The object whose properties are observed.</param>
    extension<TSender>(TSender? sender)
    {
        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Expression? property2,
            Expression? property3,
            Expression? property4,
            Expression? property5,
            Expression? property6,
            Expression? property7,
            Expression? property8,
            Expression? property9,
            Expression? property10,
            Expression? property11,
            Expression? property12,
            Func<
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                TRet> selector) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property3, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property4, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property5, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property6, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property7, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property8, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property9, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property10, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property11, false, false),
                sender.SubscribeToExpressionChain<TSender, object?>(property12, false, false),
                selector);

        /// <summary>Observes several dynamically-typed property chains and combines them with a selector.</summary>
        /// <typeparam name="TRet">The type of the resulting value.</typeparam>
        /// <param name="property1">An expression pointing to property 1.</param>
        /// <param name="property2">An expression pointing to property 2.</param>
        /// <param name="property3">An expression pointing to property 3.</param>
        /// <param name="property4">An expression pointing to property 4.</param>
        /// <param name="property5">An expression pointing to property 5.</param>
        /// <param name="property6">An expression pointing to property 6.</param>
        /// <param name="property7">An expression pointing to property 7.</param>
        /// <param name="property8">An expression pointing to property 8.</param>
        /// <param name="property9">An expression pointing to property 9.</param>
        /// <param name="property10">An expression pointing to property 10.</param>
        /// <param name="property11">An expression pointing to property 11.</param>
        /// <param name="property12">An expression pointing to property 12.</param>
        /// <param name="selector">Combines the observed change notifications into a result.</param>
        /// <param name="isDistinct">Whether to emit only when the combined value changes.</param>
        /// <returns>An observable that emits the projected result on each change.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> WhenAnyDynamic<TRet>(
            Expression? property1,
            Expression? property2,
            Expression? property3,
            Expression? property4,
            Expression? property5,
            Expression? property6,
            Expression? property7,
            Expression? property8,
            Expression? property9,
            Expression? property10,
            Expression? property11,
            Expression? property12,
            Func<
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                IObservedChange<TSender?, object?>,
                TRet> selector,
            bool isDistinct) =>
            new WhenAnyChangeSink<
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                IObservedChange<TSender, object?>,
                TRet>(
                sender.SubscribeToExpressionChain<TSender, object?>(property1, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property2, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property3, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property4, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property5, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property6, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property7, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property8, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property9, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property10, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property11, false, false, isDistinct),
                sender.SubscribeToExpressionChain<TSender, object?>(property12, false, false, isDistinct),
                selector);
    }
}
