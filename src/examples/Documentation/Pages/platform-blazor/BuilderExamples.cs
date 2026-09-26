// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI.Documentation.PlatformBlazor;

/// <summary>Shows the Blazor extensions on <see cref="IReactiveUIBuilder"/> that a host calls during startup.</summary>
public static class BuilderExamples
{
    /// <summary>
    /// <c>WithBlazor</c> configures a Blazor Server app: it sets the main-thread scheduler to
    /// <see cref="BlazorReactiveUIBuilderExtensions.BlazorMainThreadScheduler"/> and registers <see cref="Blazor.Registrations"/>.
    /// </summary>
    public static void ConfigureBlazorServer()
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = builder.WithBlazor();

        Console.WriteLine(ReferenceEquals(builder.MainThreadScheduler, BlazorReactiveUIBuilderExtensions.BlazorMainThreadScheduler));

        // Output:
        // True
    }

    /// <summary>
    /// <c>WithBlazorWasm</c> configures a Blazor WebAssembly app: it sets the main-thread scheduler to
    /// <see cref="BlazorReactiveUIBuilderExtensions.BlazorWasmScheduler"/> and registers <see cref="Blazor.Registrations"/>.
    /// </summary>
    public static void ConfigureBlazorWasm()
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = builder.WithBlazorWasm();

        Console.WriteLine(ReferenceEquals(builder.MainThreadScheduler, BlazorReactiveUIBuilderExtensions.BlazorWasmScheduler));

        // Output:
        // True
    }

    /// <summary><c>WithBlazorScheduler</c> sets only the main-thread scheduler, without the platform registrations.</summary>
    public static void ConfigureBlazorServerSchedulerOnly()
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = builder.WithBlazorScheduler();

        Console.WriteLine(ReferenceEquals(builder.MainThreadScheduler, BlazorReactiveUIBuilderExtensions.BlazorMainThreadScheduler));

        // Output:
        // True
    }

    /// <summary><c>WithBlazorWasmScheduler</c> sets only the main-thread scheduler, without the platform registrations.</summary>
    public static void ConfigureBlazorWasmSchedulerOnly()
    {
        ReactiveUIBuilder builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = builder.WithBlazorWasmScheduler();

        Console.WriteLine(ReferenceEquals(builder.MainThreadScheduler, BlazorReactiveUIBuilderExtensions.BlazorWasmScheduler));

        // Output:
        // True
    }

    /// <summary>
    /// <see cref="BlazorReactiveUIBuilderExtensions.BlazorMainThreadScheduler"/> is a current-thread scheduler for Blazor
    /// Server; <see cref="BlazorReactiveUIBuilderExtensions.BlazorWasmScheduler"/> yields through the WebAssembly event
    /// loop. Both hand back a working clock.
    /// </summary>
    public static void ReadTheSchedulers()
    {
        Console.WriteLine(BlazorReactiveUIBuilderExtensions.BlazorMainThreadScheduler.Timestamp >= 0);
        Console.WriteLine(BlazorReactiveUIBuilderExtensions.BlazorWasmScheduler.Timestamp >= 0);

        // Output:
        // True
        // True
    }
}
