// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Routing;

/// <summary>The view for <see cref="TodoDetailPage"/>. The view locator finds it because it implements <see cref="IViewFor{T}"/>.</summary>
[System.Diagnostics.DebuggerDisplay("TodoDetailPageView")]
public sealed class TodoDetailPageView : ReactiveObject, IViewFor<TodoDetailPage>
{
    /// <inheritdoc/>
    public TodoDetailPage? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TodoDetailPage?)value;
    }
}
