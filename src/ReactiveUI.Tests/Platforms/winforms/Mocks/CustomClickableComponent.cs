// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Tests.Winforms
{
    /// <summary>
    /// A custom clickable component.
    /// </summary>
    public class CustomClickableComponent : Component
    {
        /// <summary>
        /// Occurs when the click.
        /// </summary>
        public event EventHandler? Click;

        /// <summary>
        /// Performs the click.
        /// </summary>
        public void PerformClick() => Click?.Invoke(this, EventArgs.Empty);
    }
}
