// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
/// </remarks>
public class ReactiveComponentBase<T> : ComponentBase, IViewFor<T>, INotifyPropertyChanged, ICanActivate, IDisposable
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

    /// <summary>
    /// Indicates whether the instance has been disposed.
    /// </summary>
    private bool _disposed;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
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

    /// <summary>
    /// Disposes the component and releases managed resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ReactiveComponentHelpers.WireActivationIfSupported(ViewModel, _state);
        _state.NotifyActivated();
        base.OnInitialized();
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Releases managed resources used by the component.
    /// </summary>
    /// <param name="disposing">
    /// <see langword="true"/> to release managed resources; <see langword="false"/> to release unmanaged resources only.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Notify deactivation first so observers can perform cleanup while subscriptions are still active.
            _state.NotifyDeactivated();
            _state.Dispose();
        }

        _disposed = true;
    }
}
