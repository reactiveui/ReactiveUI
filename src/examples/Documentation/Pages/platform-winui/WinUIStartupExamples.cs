// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat;
using Splat.Builder;
using static ReactiveUI.Builder.WinUIReactiveUIBuilderExtensions;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Shows the WinUI builder extensions the app's own startup does not call directly: the <see cref="IAppBuilder"/>
/// overload of <c>WithWinUI</c> is not present for WinUI (unlike WPF), so this shows the pieces <c>WithWinUI</c>
/// bundles together instead. Each example builds its own resolver so it does not touch the services the app already
/// registered at startup.
/// </summary>
public static class WinUIStartupExamples
{
    /// <summary>
    /// <c>WithWinUI</c> calls <c>WithWinUIConverters</c>, <c>WithWinUIScheduler</c> and registers
    /// <see cref="WinUI.Registrations"/> for you; an app that wants only the WinUI converters or scheduler
    /// can call them on their own.
    /// </summary>
    public static void ShowTheIndividualExtensions()
    {
        using ModernDependencyResolver resolver = new();
        IReactiveUIBuilder builder = (IReactiveUIBuilder)new ReactiveUIBuilder(resolver, resolver).WithCoreServices();
        IReactiveUIBuilder configured = builder.WithWinUIConverters().WithWinUIScheduler().WithPlatformModule<WinUI.Registrations>();

        Console.WriteLine($"WithWinUIConverters/WithWinUIScheduler/Registrations configured: {configured is not null}");
        Console.WriteLine($"WinUI main-thread scheduler: {WinUIMainThreadScheduler.GetType().Name}");
    }
}
