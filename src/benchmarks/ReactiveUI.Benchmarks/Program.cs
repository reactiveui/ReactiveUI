// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Running;

namespace ReactiveUI.Benchmarks;

/// <summary>
/// Hosts the BenchmarkDotNet entry point for the ReactiveUI micro-benchmark suite.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    /// <summary>
    /// The entry point. Forwards command-line arguments (filters, exporters, profilers) to the benchmark switcher.
    /// </summary>
    /// <param name="args">The command-line arguments forwarded to BenchmarkDotNet.</param>
    public static void Main(string[] args) =>
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
