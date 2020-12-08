// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows.Forms;

namespace ReactiveUI.Tests.Winforms
{
    public class CustomClickableControl : Control
    {
        public void PerformClick() => InvokeOnClick(this, EventArgs.Empty);

        public void RaiseMouseClickEvent(MouseEventArgs args) => OnMouseClick(args);

        public void RaiseMouseUpEvent(MouseEventArgs args) => OnMouseUp(args);
    }
}