// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Device.Tests;

/// <summary>A screen with its own router.</summary>
public sealed class TestScreen : IScreen
{
    /// <inheritdoc/>
    public RoutingState Router { get; } = new();
}
