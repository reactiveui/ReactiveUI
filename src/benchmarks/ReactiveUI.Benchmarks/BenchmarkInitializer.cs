// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Initializes ReactiveUI's core services once per process. BenchmarkDotNet runs each benchmark in its own child
/// process, so a module initializer is used to guarantee the builder runs before any benchmark touches
/// <c>WhenAnyValue</c>, bindings, or commands.
/// </summary>
internal static class BenchmarkInitializer
{
    /// <summary>Runs the ReactiveUI builder so the reactive sinks can resolve their registered services.</summary>
    [ModuleInitializer]
    internal static void Initialize() => _ = RxAppBuilder.CreateReactiveUIBuilder().BuildApp();
}
