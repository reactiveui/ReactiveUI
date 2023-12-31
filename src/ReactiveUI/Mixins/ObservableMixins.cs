// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with observables.
/// </summary>
public static class ObservableMixins
{
    /// <summary>
    /// Returns only values that are not null.
    /// Converts the nullability.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the observable.</typeparam>
    /// <param name="observable">The observable that can contain nulls.</param>
    /// <returns>A non nullable version of the observable that only emits valid values.</returns>
    public static IObservable<T> WhereNotNull<T>(this IObservable<T?> observable) =>
        observable
            .Where(x => x is not null)
            .Select(x => x!);

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<Unit> Result, Action Cancel)> FromAsyncWithAllNotifications(
        Func<CancellationToken, Task> actionAsync) => Observable.Defer(
            () =>
            {
                var cts = new CancellationTokenSource();
                var result = Observable.FromAsync(
                    async ctsBase =>
                    {
                        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctsBase);
                        await actionAsync(linkedCts.Token);
                    });
                return Observable.Return<(IObservable<Unit> Result, Action Cancel)>((result, () => cts.Cancel()));
            });

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<Unit> Result, Action Cancel)> FromAsyncWithAllNotifications<TParam>(
        Func<TParam, CancellationToken, Task> actionAsync, TParam param) => Observable.Defer(
            () =>
            {
                var cts = new CancellationTokenSource();
                var result = Observable.FromAsync(
                    async ctsBase =>
                    {
                        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctsBase);
                        await actionAsync(param, linkedCts.Token);
                    });

                return Observable.Return<(IObservable<Unit> Result, Action Cancel)>((result, () => cts.Cancel()));
            });

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<TResult> Result, Action Cancel)> FromAsyncWithAllNotifications<TResult>(
        Func<CancellationToken, Task<TResult>> actionAsync) => Observable.Defer(
            () =>
            {
                var cts = new CancellationTokenSource();
                var result = Observable.FromAsync(
                    async ctsBase =>
                    {
                        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ctsBase);
                        return await actionAsync(linkedCts.Token);
                    });

                return Observable.Return<(IObservable<TResult> Result, Action Cancel)>((result, () => cts.Cancel()));
            });

    /// <summary>
    /// Converts an asynchronous action into an observable sequence. Each subscription
    ///     to the resulting sequence causes the action to be started. The CancellationToken
    ///     passed to the asynchronous action is tied to the observable sequence's subscription
    ///     that triggered the action's invocation and can be used for best-effort cancellation.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="actionAsync">Asynchronous action to convert.</param>
    /// <param name="param">The parameter.</param>
    /// <returns>An observable sequence exposing a Unit value upon completion of the action, or an exception.</returns>
    internal static IObservable<(IObservable<TResult> Result, Action Cancel)> FromAsyncWithAllNotifications<TParam, TResult>(
        Func<TParam, CancellationToken, Task<TResult>> actionAsync, TParam param) => Observable.Defer(
            () =>
            {
                var cts = new CancellationTokenSource();
                var result = Observable.FromAsync(
                    async cancelFromRx =>
                    {
                        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancelFromRx);
                        return await actionAsync(param, linkedCts.Token);
                    });

                return Observable.Return<(IObservable<TResult> Result, Action Cancel)>((result, () => cts.Cancel()));
            });
}
