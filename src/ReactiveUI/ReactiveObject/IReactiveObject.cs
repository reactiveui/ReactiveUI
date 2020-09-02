﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
    /// <summary>
    /// A reactive object is a interface for ViewModels which will expose
    /// logging, and notify when properties are either changing or changed.
    /// The primary use of this interface is to allow external classes such as
    /// the ObservableAsPropertyHelper to trigger these events inside the ViewModel.
    /// </summary>
    public interface IReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger
    {
        /// <summary>
        /// Raise a property is changing event.
        /// </summary>
        /// <param name="args">The arguments with details about the property that is changing.</param>
        void RaisePropertyChanging(PropertyChangingEventArgs args);

        /// <summary>
        /// Raise a property has changed event.
        /// </summary>
        /// <param name="args">The arguments with details about the property that has changed.</param>
        void RaisePropertyChanged(PropertyChangedEventArgs args);
    }
}
