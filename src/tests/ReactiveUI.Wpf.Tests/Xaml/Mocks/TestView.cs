// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>
/// A mock view that also acts as an <see cref="IScreen"/> for routing tests.
/// </summary>
public class TestView : ReactiveUserControl<TestViewModel>, IScreen
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestView"/> class.
    /// </summary>
    public TestView()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestView"/> class.
    /// </summary>
    /// <param name="screen">The screen whose router should be reused, or <see langword="null"/> to resolve one.</param>
    public TestView(IScreen? screen)
    {
        Router = screen?.Router ?? AppLocator.Current.GetService<RoutingState>()!;
    }

    /// <inheritdoc/>
    public RoutingState Router { get; }
}
