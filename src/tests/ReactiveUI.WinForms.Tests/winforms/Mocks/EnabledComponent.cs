// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A component with Enabled property and generic event for testing AOT-safe binding.
/// </summary>
public class EnabledComponent : Component
{
    /// <summary>
    /// A custom event using generic EventHandler.
    /// </summary>
    public event EventHandler<CustomEventArgs>? CustomEvent;

    /// <summary>
    /// Gets or sets a value indicating whether the component is enabled.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Raises the custom event.
    /// </summary>
    public void RaiseCustomEvent() => CustomEvent?.Invoke(this, new CustomEventArgs());
}
