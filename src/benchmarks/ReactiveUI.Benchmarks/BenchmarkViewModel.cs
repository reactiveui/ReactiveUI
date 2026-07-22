// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks;

/// <summary>
/// A simple <see cref="ReactiveObject"/> view model with observable properties used to exercise
/// <c>WhenAnyValue</c>, bindings, and <c>ToProperty</c> in the benchmarks.
/// </summary>
internal sealed class BenchmarkViewModel : ReactiveObject
{
    /// <summary>Gets or sets the first observable string property.</summary>
    internal string? First
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the second observable string property.</summary>
    internal string? Second
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the third observable string property (used by higher-arity WhenAnyValue benchmarks).</summary>
    internal string? Third
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the fourth observable string property (used by higher-arity WhenAnyValue benchmarks).</summary>
    internal string? Fourth
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets an observable integer property.</summary>
    internal int Count
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
