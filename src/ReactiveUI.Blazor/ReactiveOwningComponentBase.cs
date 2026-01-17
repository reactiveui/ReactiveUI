// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Components;

using ReactiveUI.Blazor.Internal;

namespace ReactiveUI.Blazor;

/// <summary>
/// A base component for handling property changes and updating the Blazor view appropriately.
/// </summary>
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
public class ReactiveOwningComponentBase<T> : OwningComponentBase<T>, IViewFor<T>, INotifyPropertyChanged, ICanActivate
    where T : class, INotifyPropertyChanged
{
    /// <summary>
    /// Encapsulates reactive state and lifecycle management for this component.
    /// </summary>
    private readonly ReactiveComponentState<T> _state = new();

    /// <summary>
    /// Backing field for <see cref="ViewModel"/>.
    /// </summary>
    private T? _viewModel;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the view model associated with this component.
    /// </summary>
    [Parameter]
    public T? ViewModel
    {
        get => _viewModel;
        set
        {
            if (EqualityComparer<T?>.Default.Equals(_viewModel, value))
            {
                return;
            }

            _viewModel = value;
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
    public IObservable<Unit> Activated => _state.Activated;

    /// <inheritdoc />
    public IObservable<Unit> Deactivated => _state.Deactivated;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ReactiveComponentHelpers.WireActivationIfSupported(ViewModel, _state);
        _state.NotifyActivated();
        base.OnInitialized();
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("OnAfterRender wires reactive subscriptions that may not be trimming-safe in all environments.")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "ComponentBase is an external reference")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "ComponentBase is an external reference")]
#endif
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            // These subscriptions are intentionally created here (not OnInitialized) due to framework interop constraints.
            _state.FirstRenderSubscriptions = ReactiveComponentHelpers.WireViewModelChangeReactivity(
                () => ViewModel,
                h => PropertyChanged += h,
                h => PropertyChanged -= h,
                nameof(ViewModel),
                () => InvokeAsync(StateHasChanged));
        }

        base.OnAfterRender(firstRender);
    }

    /// <summary>
    /// Invokes the property changed event.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Notify deactivation first so observers can perform cleanup while subscriptions are still active.
            _state.NotifyDeactivated();
            _state.Dispose();
        }

        base.Dispose(disposing);
    }
}
