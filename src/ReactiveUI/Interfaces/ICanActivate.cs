// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;

namespace ReactiveUI
{
    /// <summary>
    /// This Interface is used by the framework to explicitly provide activation
    /// events. Usually you can ignore this unless you are porting RxUI to a new
    /// UI Toolkit.
    /// </summary>
    public interface ICanActivate
    {
        IObservable<Unit> Activated { get; }

        IObservable<Unit> Deactivated { get; }
    }
}
