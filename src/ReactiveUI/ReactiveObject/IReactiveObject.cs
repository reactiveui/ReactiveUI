// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using Splat;

namespace ReactiveUI
{
    public interface IReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger
    {
        void RaisePropertyChanging(PropertyChangingEventArgs args);

        void RaisePropertyChanged(PropertyChangedEventArgs args);
    }
}
