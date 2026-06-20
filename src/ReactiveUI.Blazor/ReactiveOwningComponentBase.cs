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
/// <para>
/// This type derives from <see cref="OwningComponentBase{TService}"/> so the DI scope and owned service lifetime are
/// managed by the base class.
/// </para>
/// </remarks>
[SuppressMessage("Usage", "BL0007:Component parameters should be auto properties", Justification = "Needed for design of the properties")]
public class ReactiveOwningComponentBase<T>
    : OwningComponentBase<T>, IViewFor<T>, INotifyPropertyChanged, ICanActivate
    where T : class, INotifyPropertyChanged
{
    /// <summary>Encapsulates reactive state and lifecycle management for this component.</summary>
    private readonly ReactiveComponentState _state = new();

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets or sets the view model associated with this component.</summary>
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

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ReactiveComponentHelpers.HandleInitialized(ViewModel, _state);
        base.OnInitialized();
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode(
        "OnAfterRender wires reactive subscriptions that may not be trimming-safe in all environments.")]
    [SuppressMessage(
        "AOT",
        "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.",
        Justification = "ComponentBase is an external reference")]
    [SuppressMessage(
        "Trimming",
        "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.",
        Justification = "ComponentBase is an external reference")]
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

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        ReactiveComponentHelpers.DeactivateAndDisposeState(disposing, _state);
        base.Dispose(disposing);
    }
}
