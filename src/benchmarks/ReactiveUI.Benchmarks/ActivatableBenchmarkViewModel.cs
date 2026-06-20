// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks;

/// <summary>
/// An activatable view model with a <c>WhenActivated</c> block that registers a disposable, used to drive the
/// activation benchmarks.
/// </summary>
internal sealed class ActivatableBenchmarkViewModel : ReactiveObject, IActivatableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="ActivatableBenchmarkViewModel"/> class.</summary>
    public ActivatableBenchmarkViewModel() =>
        this.WhenActivated(disposables =>
        {
            ActivationCount++;
            disposables(EmptyDisposable.Instance);
        });

    /// <summary>Gets the activator that drives the activation lifecycle.</summary>
    public ViewModelActivator Activator { get; } = new();

    /// <summary>Gets the number of times the activation block has run, so the work cannot be elided.</summary>
    public long ActivationCount { get; private set; }
}
