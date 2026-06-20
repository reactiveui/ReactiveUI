// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Disposables;

namespace ReactiveUI.Internal;

/// <summary>
/// Runs a function synchronously on each subscription, then emits its result and completes.
/// A fault in the function is forwarded as an error. Backs the synchronous command factories.
/// </summary>
/// <typeparam name="T">The result type.</typeparam>
/// <param name="execute">The function to run on subscription.</param>
public sealed class SyncExecuteObservable<T>(Func<T> execute) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        try
        {
            var result = execute();
            observer.OnNext(result);
            observer.OnCompleted();
        }
        catch (Exception ex)
        {
            observer.OnError(ex);
        }

        return EmptyDisposable.Instance;
    }
}
