// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A component exposing only a <c>MouseUp</c> event (no <c>Click</c>), used to exercise the reflection fallback's
/// MouseEventArgs branch in the WinForms command binder.
/// </summary>
public class MouseUpOnlyComponent : Component
{
    /// <summary>Occurs when the mouse button is released over the component.</summary>
    public event EventHandler<MouseEventArgs>? MouseUp;

    /// <summary>Raises the <see cref="MouseUp"/> event.</summary>
    /// <param name="e">The mouse event arguments.</param>
    public void RaiseMouseUp(MouseEventArgs e) => MouseUp?.Invoke(this, e);
}
