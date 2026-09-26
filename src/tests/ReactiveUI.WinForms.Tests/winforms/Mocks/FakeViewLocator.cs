// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>A fake view locator that resolves views using a configurable delegate.</summary>
internal sealed class FakeViewLocator : IViewLocator
{
    /// <summary>Gets or sets the delegate used to resolve a view from a view model type.</summary>
    internal Func<Type, IViewFor>? LocatorFunc { get; set; }

    /// <summary>Gets the number of ahead-of-time safe lookups by run-time type.</summary>
    internal int SafeLookups { get; private set; }

    /// <summary>Gets the number of reflective lookups by run-time type.</summary>
    internal int UnsafeLookups { get; private set; }

    /// <inheritdoc/>
    public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
        where TViewModel : class => Resolve(viewModel, typeof(TViewModel));

    /// <inheritdoc/>
    public IViewFor? ResolveView(object? viewModel, string? contract)
    {
        SafeLookups++;
        return viewModel is null ? null : Resolve(viewModel, viewModel.GetType());
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("Resolves a view from the view model's runtime type.")]
    public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract)
    {
        UnsafeLookups++;
        return viewModel is null ? null : Resolve(viewModel, viewModel.GetType());
    }

    /// <summary>Resolves a view for the view model type and hands it the view model.</summary>
    /// <param name="viewModel">The view model the view displays.</param>
    /// <param name="viewModelType">The view model type to look the view up by.</param>
    /// <returns>The view, or <see langword="null"/> when none is configured.</returns>
    private IViewFor? Resolve(object viewModel, Type viewModelType)
    {
        var view = LocatorFunc?.Invoke(viewModelType);
        if (view is not null)
        {
            view.ViewModel = viewModel;
        }

        return view;
    }
}
