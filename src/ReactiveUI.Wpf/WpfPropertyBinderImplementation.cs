// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Threading;

namespace ReactiveUI;

internal sealed class WpfPropertyBinderImplementation : PropertyBinderImplementation
{
    internal override IObservable<bool> ScheduleForBinding<TView>(TView view, bool value)
    {
        if (view is not DispatcherObject dispatcherObject || dispatcherObject.CheckAccess())
        {
            return Observable.Return(value);
        }

        return Observable.Create<bool>(observer =>
        {
            var operation = dispatcherObject.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                }));

            return Disposable.Create(() => operation.Abort());
        });
    }

    internal override void SetViewValue<TView>(TView view, Action setter)
    {
        ArgumentExceptionHelper.ThrowIfNull(setter);

        if (view is not DispatcherObject dispatcherObject || dispatcherObject.CheckAccess())
        {
            setter();
            return;
        }

        dispatcherObject.Dispatcher.BeginInvoke(setter);
    }
}
