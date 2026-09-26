// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;
using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.PlatformBlazor;

/// <summary>
/// A component that owns a scoped <see cref="TodoListViewModel"/> for as long as the component lives, through
/// <see cref="ReactiveOwningComponentBase{T}"/>. A DI container creates the scope and resolves the service for
/// <c>Service</c>; this example sets <see cref="ReactiveOwningComponentBase{T}.ViewModel"/> directly instead, because a
/// console program has no DI-backed renderer to create that scope.
/// </summary>
public sealed class TodoScopedComponent : ReactiveOwningComponentBase<TodoListViewModel>
{
    /// <summary>
    /// Runs the framework's <c>OnInitialized</c> step. A Blazor host calls this once the component is attached to a
    /// renderer; a console example has no renderer, so it calls the step directly.
    /// </summary>
    public void Initialize() => OnInitialized();

    /// <summary>
    /// Raises <see cref="ReactiveOwningComponentBase{T}.PropertyChanged"/> for an explicit property name, the way a
    /// component announces a change to a value it computes from <see cref="ReactiveOwningComponentBase{T}.ViewModel"/>.
    /// </summary>
    public void RaiseManualChange() => OnPropertyChanged(nameof(ViewModel));
}
