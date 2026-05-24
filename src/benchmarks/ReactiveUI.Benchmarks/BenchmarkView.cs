// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks;

/// <summary>
/// A minimal view bound to <see cref="BenchmarkViewModel"/>, used to exercise one-way and two-way bindings.
/// </summary>
internal sealed class BenchmarkView : ReactiveObject, IViewFor<BenchmarkViewModel>
{
    /// <summary>Gets or sets the bound view model.</summary>
    public BenchmarkViewModel? ViewModel
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (BenchmarkViewModel?)value;
    }

    /// <summary>Gets or sets the view-side text bound to the view model's <see cref="BenchmarkViewModel.First"/>.</summary>
    public string? FirstText
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
