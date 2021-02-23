// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;

namespace ReactiveUI.Tests.Winforms
{
    public class CustomClickableComponent : Component
    {
        public event EventHandler? Click;

        public void PerformClick() => Click?.Invoke(this, EventArgs.Empty);
    }
}
