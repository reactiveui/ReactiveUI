// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

public class TestView : ReactiveUserControl<TestViewModel>, IScreen
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public TestView()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public TestView(IScreen? screen = null)
    {
        Router = screen?.Router ?? Locator.Current.GetService<RoutingState>()!;
    }

    public RoutingState Router { get; }
}
