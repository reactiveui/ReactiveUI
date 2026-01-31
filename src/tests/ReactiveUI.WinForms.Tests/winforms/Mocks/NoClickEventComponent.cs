// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A component with no Click or MouseUp event for testing zero affinity.
/// </summary>
public class NoClickEventComponent : Component
{
    /// <summary>
    /// An event that is not Click or MouseUp.
    /// </summary>
    public event EventHandler? SomeOtherEvent;

    /// <summary>
    /// Raises the other event.
    /// </summary>
    public void RaiseOtherEvent() => SomeOtherEvent?.Invoke(this, EventArgs.Empty);
}
