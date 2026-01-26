// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// Custom event args for testing generic event handlers.
/// </summary>
public class CustomEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a test value.
    /// </summary>
    public int Value { get; set; }
}
