// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Controls;

namespace ReactiveUI.Tests.Xaml.Mocks;

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
