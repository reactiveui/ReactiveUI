// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.Blazor.Internal;

/// <summary>
/// Drives Blazor re-rendering for a component's view model: it re-renders when the view model instance is
/// reassigned (after the first assignment) and whenever the current view model raises any property change.
/// </summary>
/// <remarks>
/// A single fused sink that wires the CLR <see cref="INotifyPropertyChanged.PropertyChanged"/> events directly,
/// replacing the previous <c>Publish().RefCount().Skip().Select().Switch()</c> pipeline. It allocates only the
/// sink plus two reused handler delegates — no intermediate observables, no per-change delegate or disposable
/// allocations — and swaps the observed view model in place under a reference-equality guard.
/// </remarks>
/// <typeparam name="T">The view model type.</typeparam>
internal sealed class ViewModelReactivitySink<T> : IDisposable
    where T : class, INotifyPropertyChanged
{
    /// <summary>Returns the component's current view model.</summary>
    private readonly Func<T?> _getCurrentViewModel;

    /// <summary>Detaches the component property-changed handler on dispose.</summary>
    private readonly Action<PropertyChangedEventHandler> _removeComponentHandler;

    /// <summary>The component property name that carries the view model (typically <c>"ViewModel"</c>).</summary>
    private readonly string _viewModelPropertyName;

    /// <summary>Requests a UI re-render (typically <c>StateHasChanged</c> marshalled via <c>InvokeAsync</c>).</summary>
    private readonly Action _stateHasChanged;

    /// <summary>Cached handler watching the component for view model reassignment.</summary>
    private readonly PropertyChangedEventHandler _componentHandler;

    /// <summary>Cached handler watching the current view model for property changes.</summary>
    private readonly PropertyChangedEventHandler _viewModelHandler;

    /// <summary>The view model currently subscribed to, or <see langword="null"/>.</summary>
    private T? _subscribedViewModel;

    /// <summary>Guards against double disposal.</summary>
    private bool _disposed;

    /// <summary>Initializes a new instance of the <see cref="ViewModelReactivitySink{T}"/> class.</summary>
    /// <param name="getCurrentViewModel">Returns the component's current view model.</param>
    /// <param name="addComponentHandler">Attaches a handler to the component's property-changed event.</param>
    /// <param name="removeComponentHandler">Detaches a handler from the component's property-changed event.</param>
    /// <param name="viewModelPropertyName">The component property name that carries the view model.</param>
    /// <param name="stateHasChanged">Requests a UI re-render.</param>
    public ViewModelReactivitySink(
        Func<T?> getCurrentViewModel,
        Action<PropertyChangedEventHandler> addComponentHandler,
        Action<PropertyChangedEventHandler> removeComponentHandler,
        string viewModelPropertyName,
        Action stateHasChanged)
    {
        _getCurrentViewModel = getCurrentViewModel;
        _removeComponentHandler = removeComponentHandler;
        _viewModelPropertyName = viewModelPropertyName;
        _stateHasChanged = stateHasChanged;
        _componentHandler = OnComponentPropertyChanged;
        _viewModelHandler = OnViewModelPropertyChanged;

        addComponentHandler(_componentHandler);

        // Observe the initial view model's property changes without forcing a render — this matches the original
        // Skip(1) on instance changes combined with the immediate Switch onto the current view model's pulses.
        SwapViewModel(_getCurrentViewModel());
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _removeComponentHandler(_componentHandler);

        if (_subscribedViewModel is null)
        {
            return;
        }

        _subscribedViewModel.PropertyChanged -= _viewModelHandler;
        _subscribedViewModel = null;
    }

    /// <summary>Re-renders and re-targets the observed view model when the component reassigns it.</summary>
    /// <param name="sender">The component.</param>
    /// <param name="e">The property-changed arguments.</param>
    private void OnComponentPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!string.Equals(e.PropertyName, _viewModelPropertyName, StringComparison.Ordinal))
        {
            return;
        }

        var viewModel = _getCurrentViewModel();
        if (viewModel is null)
        {
            return;
        }

        SwapViewModel(viewModel);
        _stateHasChanged();
    }

    /// <summary>Re-renders when the current view model raises any property change.</summary>
    /// <param name="sender">The view model.</param>
    /// <param name="e">The property-changed arguments.</param>
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) => _stateHasChanged();

    /// <summary>Moves the property-changed subscription onto <paramref name="viewModel"/>, if it changed.</summary>
    /// <param name="viewModel">The view model to observe, or <see langword="null"/> to observe none.</param>
    private void SwapViewModel(T? viewModel)
    {
        if (ReferenceEquals(_subscribedViewModel, viewModel))
        {
            return;
        }

        if (_subscribedViewModel is not null)
        {
            _subscribedViewModel.PropertyChanged -= _viewModelHandler;
        }

        _subscribedViewModel = viewModel;

        if (viewModel is null)
        {
            return;
        }

        viewModel.PropertyChanged += _viewModelHandler;
    }
}
