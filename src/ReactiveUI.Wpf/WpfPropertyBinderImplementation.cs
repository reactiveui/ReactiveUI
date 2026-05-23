// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI;

/// <summary>
/// The WPF property binder implementation.
/// </summary>
internal sealed class WpfPropertyBinderImplementation : PropertyBinderImplementation
{
    /// <inheritdoc/>
    protected override IObservable<bool> ScheduleForBinding<TView>(TView view, bool value) =>
        new DispatcherValueObservable(view as DispatcherObject, value);

    /// <summary>
    /// Set the view value.
    /// </summary>
    /// <param name="view">The view to fetch the value from.</param>
    /// <param name="setter">The setter action.</param>
    /// <typeparam name="TView">The type of the view.</typeparam>
    protected override void SetViewValue<TView>(TView view, Action setter)
    {
        ArgumentExceptionHelper.ThrowIfNull(setter);

        if (view is not DispatcherObject dispatcherObject || dispatcherObject.CheckAccess())
        {
            setter();
            return;
        }

        dispatcherObject.Dispatcher.BeginInvoke(setter);
    }

    /// <summary>
    /// Emits a single value, inline when already on the view's dispatcher thread (or when the view is not a
    /// <see cref="DispatcherObject"/>), otherwise marshalled onto the dispatcher. A dedicated fusion of
    /// <c>Observable.Return</c>/<c>Observable.Create</c> with the dispatcher check — no operator chain.
    /// </summary>
    /// <param name="dispatcherObject">The dispatcher-affine view, or <see langword="null"/> to emit inline.</param>
    /// <param name="value">The value to emit.</param>
    private sealed class DispatcherValueObservable(DispatcherObject? dispatcherObject, bool value) : IObservable<bool>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            if (dispatcherObject?.CheckAccess() != false)
            {
                observer.OnNext(value);
                observer.OnCompleted();
                return EmptyDisposable.Instance;
            }

            var operation = dispatcherObject.Dispatcher.BeginInvoke(
                () =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                });

            return new ActionDisposable(() => operation.Abort());
        }
    }
}
