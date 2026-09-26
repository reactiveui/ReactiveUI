// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Blazor;
using ReactiveUI.Documentation.Todo;

namespace ReactiveUI.Documentation.PlatformBlazor;

/// <summary>
/// A layout that wraps every page of the to-do app and reacts to a <see cref="TodoListViewModel"/> of its own, such
/// as a header that shows the remaining item count next to the page content.
/// </summary>
public sealed class TodoShellLayoutComponent : ReactiveLayoutComponentBase<TodoListViewModel>
{
    /// <summary>
    /// Runs the framework's <c>OnInitialized</c> step. A Blazor host calls this once the component is attached to a
    /// renderer; a console example has no renderer, so it calls the step directly.
    /// </summary>
    public void Initialize() => OnInitialized();

    /// <summary>
    /// Raises <see cref="ReactiveLayoutComponentBase{T}.PropertyChanged"/> for an explicit property name, the way a
    /// layout announces a change to a value it computes from <see cref="ReactiveLayoutComponentBase{T}.ViewModel"/>.
    /// </summary>
    public void RaiseManualChange() => OnPropertyChanged(nameof(ViewModel));
}
