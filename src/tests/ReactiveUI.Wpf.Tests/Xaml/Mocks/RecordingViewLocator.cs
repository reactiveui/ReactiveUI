// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Tests.Xaml.Mocks;

/// <summary>A view locator that creates a view per lookup and counts which of its lookups a host asked.</summary>
/// <param name="createView">Creates the view each lookup returns, or returns <see langword="null"/> for none.</param>
public sealed class RecordingViewLocator(Func<IViewFor?> createView) : IViewLocator
{
    /// <summary>Gets the number of ahead-of-time safe lookups by run-time type.</summary>
    public int SafeLookups { get; private set; }

    /// <summary>Gets the number of reflective lookups by run-time type.</summary>
    public int UnsafeLookups { get; private set; }

    /// <inheritdoc/>
    public IViewFor? ResolveView<TViewModel>(TViewModel viewModel, string? contract)
        where TViewModel : class => createView();

    /// <inheritdoc/>
    public IViewFor? ResolveView(object? viewModel, string? contract)
    {
        SafeLookups++;
        return createView();
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("Resolves a view from the view model's runtime type.")]
    public IViewFor? ResolveViewUnsafe(object? viewModel, string? contract)
    {
        UnsafeLookups++;
        return createView();
    }
}
