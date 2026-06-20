// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks
{
    /// <summary>A test control that records when it has been activated.</summary>
    internal sealed class DesignModeTestControl : Control, IActivatableView
    {
        /// <summary>Initializes a new instance of the <see cref="DesignModeTestControl"/> class.</summary>
        public DesignModeTestControl()
        {
            this.WhenActivated(() =>
            {
                Activated = true;
                return [];
            });
        }

        /// <summary>Gets a value indicating whether this control has been activated.</summary>
        public bool Activated { get; private set; }
    }
}
