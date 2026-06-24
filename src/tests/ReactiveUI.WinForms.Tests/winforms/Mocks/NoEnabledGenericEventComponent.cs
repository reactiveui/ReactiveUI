// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A component with a generic <see cref="EventHandler{TEventArgs}"/> event but no <c>Enabled</c> property, used to
/// exercise the WinForms command binder's missing-Enabled branch for the generic event-handler overload.
/// </summary>
public class NoEnabledGenericEventComponent : Component
{
    /// <summary>A custom event using the generic event handler.</summary>
    public event EventHandler<CustomEventArgs>? CustomEvent;

    /// <summary>Raises the <see cref="CustomEvent"/> event.</summary>
    public void RaiseCustomEvent() => CustomEvent?.Invoke(this, new());
}
