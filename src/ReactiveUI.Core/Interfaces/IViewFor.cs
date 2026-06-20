// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>Provides a non-generic abstraction over views so infrastructure can interact with <see cref="IViewFor{T}"/> instances.</summary>
/// <remarks>
/// <para>
/// Most application code implements <see cref="IViewFor{T}"/> instead of this interface directly. The non-generic
/// type exists so routing and binding helpers can store heterogeneous view references at runtime while still exposing
/// the <see cref="ViewModel"/> property.
/// </para>
/// </remarks>
public interface IViewFor : IActivatableView
{
    /// <summary>Gets or sets the view model associated with the view.</summary>
    object? ViewModel { get; set; }
}
