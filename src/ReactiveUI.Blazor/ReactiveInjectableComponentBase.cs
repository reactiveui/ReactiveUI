// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace ReactiveUI.Blazor;

/// <summary>
/// Same as ReactiveComponentBase, except that ViewModel will be injected (automatically set) via
/// DI.
/// </summary>
/// <typeparam name="T">The type of view model. Must support INotifyPropertyChanged.</typeparam>
public abstract class ReactiveInjectableComponentBase<T> : ReactiveComponentBase<T>
    where T : class, INotifyPropertyChanged
{
    /// <inheritdoc />
    [Inject]
    public override T? ViewModel
    {
        get => base.ViewModel;
        set => base.ViewModel = value;
    }
}
