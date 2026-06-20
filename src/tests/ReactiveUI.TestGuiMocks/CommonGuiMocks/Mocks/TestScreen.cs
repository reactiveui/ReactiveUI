// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;

/// <summary>A mock <see cref="IScreen"/> used for hosting a router in tests.</summary>
public class TestScreen : ReactiveObject, IScreen
{
    /// <summary>Initializes a new instance of the <see cref="TestScreen"/> class.</summary>
    public TestScreen() => Router = new();

    /// <inheritdoc/>
    public RoutingState Router
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
