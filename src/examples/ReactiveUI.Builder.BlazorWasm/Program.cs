// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReactiveUI.Builder;

namespace ReactiveUI.Builder.BlazorWasm;

/// <summary>The host type generated for the example's top-level statements.</summary>
internal static class Program
{
    /// <summary>Gets the assembly containing the WebAssembly example.</summary>
    internal static System.Reflection.Assembly HostAssembly => typeof(Program).Assembly;

    /// <summary>The main entry point for the WebAssembly example.</summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");

        builder.RootComponents.Add<HeadOutlet>("head::after");

        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithBlazorWasm()
            .BuildApp();

        await builder.Build().RunAsync();
    }
}
