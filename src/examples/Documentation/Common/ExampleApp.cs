// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Documentation;

/// <summary>Starts ReactiveUI the way each example program does before it runs its examples.</summary>
public static class ExampleApp
{
    /// <summary>
    /// Registers ReactiveUI's services and runs main-thread work immediately. A console program has no UI thread,
    /// so work that an app would post to its dispatcher runs in place, and each example prints in order.
    /// </summary>
    public static void Start() => Start(static _ => { });

    /// <summary>
    /// Registers ReactiveUI's services, then lets the example add its own. View modules are added after the core services
    /// are built, because they map their views into the view locator the core services register.
    /// </summary>
    /// <param name="configure">Adds the example's own registrations, such as its views.</param>
    public static void Start(Action<IReactiveUIBuilder> configure)
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder().WithMainThreadScheduler(Sequencer.Immediate);
        _ = builder.WithCoreServices().BuildApp();
        configure(builder);
    }
}
