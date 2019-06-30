﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml.Controls;

#if HAS_UNO
namespace ReactiveUI.Uno
#else
namespace ReactiveUI
#endif
{
    /// <summary>
    /// A control with a single transition.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Collections should implement generic interface", Justification = "Deliberate usage")]
    public
#if HAS_UNO
        partial
#endif
        class TransitioningContentControl : ContentControl
    {
    }
}
