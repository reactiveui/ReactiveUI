// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Blazor.Internal;
#else
using ReactiveUI.Blazor.Internal;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Blazor;
#else
namespace ReactiveUI.Blazor;
#endif
/// <summary>A base component for handling property changes and updating the Blazor view appropriately.</summary>
/// <typeparam name="T">The type of view model. Must support <see cref="INotifyPropertyChanged"/>.</typeparam>
/// <remarks>
/// <para>
/// This component triggers <see cref="ComponentBase.StateHasChanged"/> when either the view model instance changes or
/// the current view model raises <see cref="INotifyPropertyChanged.PropertyChanged"/>.
/// </para>
/// <para>
/// Trimming/AOT: this type avoids expression-tree-based ReactiveUI helpers (e.g. WhenAnyValue) and uses event-based
/// observables instead.
/// </para>
/// </remarks>
[SuppressMessage("Usage", "BL0007:Component parameters should be auto properties", Justification = "Needed for design of the properties")]
public class ReactiveComponentBase<T> : ComponentBase, IViewFor<T>, INotifyPropertyChanged, ICanActivate, IDisposable
    where T : class, INotifyPropertyChanged
{
    /// <summary>Encapsulates reactive state and lifecycle management for this component.</summary>
    private readonly ReactiveComponentState _state = new();

    /// <summary>Indicates whether the instance has been disposed.</summary>
    private bool _disposed;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    [Parameter]
    public T? ViewModel
    {
        get => field;
        set
        {
            if (!ReactiveComponentHelpers.SetIfChanged(ref field, value))
            {
                return;
            }

            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (T?)value;
    }

    /// <inheritdoc />
    public IObservable<RxVoid> Activated => _state.Activated;

    /// <inheritdoc />
    public IObservable<RxVoid> Deactivated => _state.Deactivated;

    /// <summary>Disposes the component and releases managed resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ReactiveComponentHelpers.HandleInitialized(ViewModel, _state);
        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void OnAfterRender(bool firstRender)
    {
        ReactiveComponentHelpers.HandleFirstRender(
            firstRender,
            _state,
            () => ViewModel,
            h => PropertyChanged += h,
            h => PropertyChanged -= h,
            () => InvokeAsync(StateHasChanged));
        base.OnAfterRender(firstRender);
    }

    /// <summary>Invokes the property changed event.</summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new(propertyName));

    /// <summary>Releases managed resources used by the component.</summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release managed resources; <see langword="false"/> to release unmanaged resources only.
    /// </param>
    protected virtual void Dispose(bool disposing) =>
        ReactiveComponentHelpers.HandleDispose(ref _disposed, disposing, _state);
}
