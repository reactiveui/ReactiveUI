// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for creating observables from expression-based property accessors on view model
/// instances.
/// </summary>
/// <remarks>These extension methods enable reactive observation of property changes by converting expression
/// trees into observable sequences. This is useful for scenarios where you want to monitor changes to properties in
/// view models and react to those changes in a composable, declarative manner. The methods in this class rely on
/// reflection and may be affected by trimming in certain deployment scenarios.</remarks>
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
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IObservable<TResult?> ToObservable<TSource, TResult>(
        this Expression<Func<TSource, TResult?>> expression,
        TSource? source,
        bool beforeChange = false,
        bool skipInitial = false) // TODO: Create Test
    {
        ArgumentExceptionHelper.ThrowIfNull(expression);

        var sParam = Reflection.Rewrite(expression.Body);
        return source.SubscribeToExpressionChain<TSource, TResult?>(sParam, beforeChange, skipInitial, RxSchedulers.SuppressViewCommandBindingMessage)
                     .Select(static x => x.GetValue())
                     .Retry();
    }
}
