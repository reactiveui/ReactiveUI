// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ViewLocator;
#else
using static ReactiveUI.Binding.ViewLocator;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>The view lookup the platform view hosts share.</summary>
internal static class ViewHostResolution
{
    /// <summary>Finds a view by the view model's run-time type without building any type at run time.</summary>
    /// <param name="viewLocator">The view locator to ask.</param>
    /// <param name="viewModel">The view model to find a view for.</param>
    /// <param name="contract">The contract to resolve under, or <see langword="null"/> for the default view.</param>
    /// <returns>The view, or <see langword="null"/> when neither the generated lookup nor a <c>Map</c> registration has one.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IViewFor? ResolveViewWithoutReflection(IViewLocator viewLocator, object viewModel, string? contract) =>
        viewLocator.ResolveView(viewModel, contract);

    /// <summary>Finds a view under a contract, then under the default contract unless the fallback is bypassed.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    /// <param name="viewLocator">The host's view locator, or <see langword="null"/> to use the current one.</param>
    /// <param name="viewModel">The view model to find a view for.</param>
    /// <param name="contract">The contract to resolve under first.</param>
    /// <param name="contractFallbackByPass">Whether to skip the retry under the default contract.</param>
    /// <returns>The view, or <see langword="null"/> when no lookup has one.</returns>
    internal static IViewFor? ResolveViewWithFallback(
        Func<IViewLocator, object, string?, IViewFor?> resolveView,
        IViewLocator? viewLocator,
        object viewModel,
        string? contract,
        bool contractFallbackByPass)
    {
        var locator = viewLocator ?? GetCurrent();
        var view = resolveView(locator, viewModel, contract);
        if (view is null && !contractFallbackByPass)
        {
            view = resolveView(locator, viewModel, null);
        }

        return view;
    }

    /// <summary>Finds a view for a view model and hands the view model to it.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    /// <param name="viewLocator">The host's view locator, or <see langword="null"/> to use the current one.</param>
    /// <param name="viewModel">The view model to find a view for, or <see langword="null"/> for none.</param>
    /// <param name="contract">The contract to resolve under first.</param>
    /// <param name="contractFallbackByPass">Whether to skip the retry under the default contract.</param>
    /// <returns>The view with its view model set, or <see langword="null"/> when there is no view model or no view.</returns>
    internal static IViewFor? ResolveAttachedView(
        Func<IViewLocator, object, string?, IViewFor?> resolveView,
        IViewLocator? viewLocator,
        object? viewModel,
        string? contract,
        bool contractFallbackByPass)
    {
        if (viewModel is null)
        {
            return null;
        }

        var view = ResolveViewWithFallback(resolveView, viewLocator, viewModel, contract, contractFallbackByPass);
        if (view is not null)
        {
            view.ViewModel = viewModel;
        }

        return view;
    }

    /// <summary>Builds the warning a host logs when it finds no view for a view model.</summary>
    /// <param name="hostName">The name of the host's type.</param>
    /// <param name="viewModel">The view model that has no view.</param>
    /// <param name="unsafeHostName">The name of the host that also asks the service locator.</param>
    /// <returns>The warning text.</returns>
    internal static string NoViewFoundWarning(string hostName, object viewModel, string unsafeHostName) =>
        $"The {hostName} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}. "
        + "The view locator checked the generated view lookup and its Map registrations; "
        + $"use {unsafeHostName} to also resolve a view registered only with the service locator.";
}
