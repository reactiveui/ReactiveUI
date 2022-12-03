// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;

namespace ReactiveUI.Blazor;

/// <summary>
/// A base component for handling property changes and updating the blazer view appropriately.
/// </summary>
/// <typeparam name="T">The type of view model. Must support INotifyPropertyChanged.</typeparam>
[Obsolete("This class is deprecated. Use ReactiveComponentBase instead.")]
public abstract class ReactiveLayoutComponentBase<T> : ReactiveComponentBase<T>
    where T : class, INotifyPropertyChanged
{
}
