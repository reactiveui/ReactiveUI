﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
#endif

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// A button for custom clicking.
/// </summary>
public class CustomClickButton : Button
{
    /// <summary>
    /// Occurs when [custom click].
    /// </summary>
    public event EventHandler<EventArgs>? CustomClick;

    /// <summary>
    /// Raises the custom click.
    /// </summary>
    public void RaiseCustomClick() =>
        CustomClick?.Invoke(this, EventArgs.Empty);
}
