// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Views;

namespace ReactiveUI;

/// <summary>
/// Represents a host that provides access to a layout view instance.
/// </summary>
public interface ILayoutViewHost
{
    /// <summary>
    /// Gets the view associated with the current context, if available.
    /// </summary>
    View? View { get; }
}
