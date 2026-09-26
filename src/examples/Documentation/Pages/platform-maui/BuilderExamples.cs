// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Hosting;
using ReactiveUI.Builder;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="MauiReactiveUIBuilderExtensions"/>, the extension members a MAUI app's <c>MauiProgram</c> calls to
/// wire up ReactiveUI. A real app calls these once, from <c>MauiProgram.CreateMauiApp</c>, in place of the console
/// pages' <c>ExampleApp.Start()</c>; these examples build a throwaway builder instead, so they can run alongside
/// every other example on this page.
/// </summary>
public static class BuilderExamples
{
    /// <summary><c>WithMauiConverters</c> registers <c>BooleanToVisibilityTypeConverter</c> and <c>VisibilityToBooleanTypeConverter</c> with the binding converter service.</summary>
    public static void WithMauiConvertersRegistersTheVisibilityConverters()
    {
        IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();

        IReactiveUIBuilder configured = builder.WithMauiConverters();

        Console.WriteLine(ReferenceEquals(builder, configured));

        // Output:
        // True
    }

    /// <summary><c>WithMauiScheduler</c> given a dispatcher points <see cref="RxSchedulers.MainThreadScheduler"/> at a sequencer built from it, without needing a running MAUI app.</summary>
    public static void WithMauiSchedulerAcceptsAnExplicitDispatcher()
    {
        IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder()
            .WithMauiScheduler(new ImmediateDispatcher());

        Console.WriteLine(builder is not null);

        // Output:
        // True
    }

    /// <summary><c>WithMaui</c> given a dispatcher registers MAUI's platform module, converters and scheduler in one call.</summary>
    public static void WithMauiAcceptsAnExplicitDispatcher()
    {
        IReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder()
            .WithMaui(new ImmediateDispatcher());

        Console.WriteLine(builder is not null);

        // Output:
        // True
    }

    /// <summary><c>MauiAppBuilder.UseReactiveUI</c> given a dispatcher configures ReactiveUI for MAUI and builds the app, all from one call in <c>MauiProgram</c>.</summary>
    public static void UseReactiveUiWithADispatcherConfiguresMaui()
    {
        MauiAppBuilder mauiBuilder = MauiApp.CreateBuilder();

        MauiAppBuilder configured = mauiBuilder.UseReactiveUI(new ImmediateDispatcher());

        Console.WriteLine(ReferenceEquals(mauiBuilder, configured));

        // Output:
        // True
    }

    /// <summary><c>MauiAppBuilder.UseReactiveUI</c> given a delegate lets the app add its own registrations after the core services are built.</summary>
    public static void UseReactiveUiWithADelegateAddsRegistrations()
    {
        MauiAppBuilder mauiBuilder = MauiApp.CreateBuilder();
        bool delegateRan = false;

        _ = mauiBuilder.UseReactiveUI(builder =>
        {
            _ = builder.WithMauiConverters();
            delegateRan = true;
        });

        Console.WriteLine(delegateRan);

        // Output:
        // True
    }
}
