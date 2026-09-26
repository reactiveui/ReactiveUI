// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows the overloads of <c>WithMaui</c>, <c>WithMauiScheduler</c> and <c>MauiAppBuilder.UseReactiveUI</c> that take
/// no dispatcher, and the <see cref="MauiReactiveUIBuilderExtensions.MauiMainThreadScheduler"/> property they read.
/// With no dispatcher argument, each one asks the calling thread for <c>Microsoft.Maui.Dispatching.Dispatcher.Current</c>,
/// which only exists once a MAUI app is running, so this method builds but the page never calls it.
/// </summary>
public static class NoDispatcherBuilderExamples
{
    /// <summary>Every no-dispatcher overload needs a running MAUI app to find a dispatcher for the calling thread.</summary>
    public static void NoDispatcherOverloadsNeedARunningApp()
    {
        IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = builder.WithMauiScheduler();
        _ = builder.WithMaui();

        Console.WriteLine(MauiReactiveUIBuilderExtensions.MauiMainThreadScheduler.GetType().Name);
    }
}
