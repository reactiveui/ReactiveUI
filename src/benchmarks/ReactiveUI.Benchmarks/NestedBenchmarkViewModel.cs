// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks;

/// <summary>
/// A view model with a reactive child view model, used to drive multi-level <c>WhenAnyValue</c> chains
/// (<c>x =&gt; x.Child.First</c>) in the benchmarks.
/// </summary>
internal sealed class NestedBenchmarkViewModel : ReactiveObject
{
    /// <summary>Gets or sets the child view model observed through the chain.</summary>
    internal BenchmarkViewModel? Child
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
