// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Activation.Mocks;

/// <summary>
/// A view which simulates a activation.
/// </summary>
public sealed class ActivatingView : ReactiveObject, IViewFor<ActivatingViewModel>, ICanActivate, IDisposable
{
    private ActivatingViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActivatingView"/> class.
    /// </summary>
    public ActivatingView() =>
        this.WhenActivated(d =>
        {
            IsActiveCount++;
            d(Disposable.Create(() => IsActiveCount--));
        });

    /// <summary>
    /// Gets the loaded.
    /// </summary>
    public Subject<Unit> Loaded { get; } = new();

    /// <summary>
    /// Gets the unloaded.
    /// </summary>
    public Subject<Unit> Unloaded { get; } = new();

    /// <summary>
    /// Gets an observable that signals when the view is activated.
    /// </summary>
    public IObservable<Unit> Activated => Loaded;

    /// <summary>
    /// Gets an observable that signals when the view is deactivated.
    /// </summary>
    public IObservable<Unit> Deactivated => Unloaded;

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public ActivatingViewModel? ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ActivatingViewModel?)value;
    }

    /// <summary>
    /// Gets or sets the active count.
    /// </summary>
    public int IsActiveCount { get; set; }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        Loaded.Dispose();
        Unloaded.Dispose();
    }
}
