// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
#if REACTIVE_SHIM
using ICanActivateContract = ReactiveUI.Reactive.ICanActivate;
#else
using ICanActivateContract = ReactiveUI.ICanActivate;
#endif

namespace ReactiveUI.Tests.Activation;

/// <summary>A view which simulates a activation.</summary>
public sealed class ActivatingView : ReactiveObject, IViewFor<ActivatingViewModel>, ICanActivateContract, IDisposable
{
    /// <summary>Initializes a new instance of the <see cref="ActivatingView" /> class.</summary>
    [SuppressMessage(
        "Performance",
        "PSH1011:Anonymous function captures state",
        Justification = "The deactivation callback decrements this view's own counter; capturing the instance is the intended activation pattern.")]
    public ActivatingView() =>
        this.WhenActivated(d =>
        {
            IsActiveCount++;
            d(Scope.Create(() => IsActiveCount--));
        });

    /// <summary>Gets an observable that signals when the view is activated.</summary>
    public IObservable<RxVoid> Activated => Loaded;

    /// <summary>Gets an observable that signals when the view is deactivated.</summary>
    public IObservable<RxVoid> Deactivated => Unloaded;

    /// <summary>Gets the loaded.</summary>
    public Signal<RxVoid> Loaded { get; } = new();

    /// <summary>Gets the unloaded.</summary>
    public Signal<RxVoid> Unloaded { get; } = new();

    /// <summary>Gets or sets the active count.</summary>
    public int IsActiveCount { get; set; }

    /// <summary>Gets or sets the view model.</summary>
    public ActivatingViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the view model.</summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ActivatingViewModel?)value;
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    public void Dispose()
    {
        Loaded.Dispose();
        Unloaded.Dispose();
    }
}
