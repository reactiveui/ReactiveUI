// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Observable Func Mixins.
/// </summary>
public static class ObservableFuncMixins
{
    /// <summary>
    /// Converts to observable.
    /// </summary>
    /// <typeparam name="TSource">The type of the view model.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="source">The view model.</param>
    /// <param name="beforeChange">if set to <c>true</c> [before change].</param>
    /// <param name="skipInitial">if set to <c>true</c> [skip initial].</param>
    /// <returns>
    /// An observable Result.
    /// </returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("This method uses reflection to access properties by name.")]
    [RequiresDynamicCode("This method uses reflection to access properties by name.")]
#endif
    public static IObservable<TResult?> ToObservable<TSource, TResult>(
        this Expression<Func<TSource, TResult?>> expression,
        TSource? source,
        bool beforeChange = false,
        bool skipInitial = false) // TODO: Create Test
    {
        expression.ArgumentNullExceptionThrowIfNull(nameof(expression));

        var sParam = Reflection.Rewrite(expression.Body);
        return source.SubscribeToExpressionChain<TSource, TResult?>(sParam, beforeChange, skipInitial, RxApp.SuppressViewCommandBindingMessage)
                     .Select(static x => x.GetValue())
                     .Retry();
    }
}
