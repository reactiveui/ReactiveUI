// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    /// A suspension driver that does not do anything.
    /// Useful potentially for unit testing or for platforms
    /// where you don't want to use a Suspension Driver.
    /// </summary>
    public class DummySuspensionDriver : ISuspensionDriver
    {
        /// <inheritdoc/>
        public IObservable<object> LoadState()
        {
            return Observable<object>.Default;
        }

        /// <inheritdoc/>
        public IObservable<Unit> SaveState(object state)
        {
            return Observables.Unit;
        }

        /// <inheritdoc/>
        public IObservable<Unit> InvalidateState()
        {
            return Observables.Unit;
        }
    }
}
