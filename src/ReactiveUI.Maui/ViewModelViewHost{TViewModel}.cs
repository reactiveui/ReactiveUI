// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>
/// This content view will automatically load and host the view for the given view model. The view model whose view is
/// to be displayed should be assigned to the inherited view-model property. Optionally, the chosen view can be
/// customized by specifying a contract through the inherited contract members.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model. Must have a public parameterless constructor for AOT compatibility.</typeparam>
/// <remarks>
/// This is the AOT-compatible generic version of ViewModelViewHost. It uses compile-time type information
/// to resolve views without reflection, making it safe for Native AOT and trimming scenarios.
/// </remarks>
[RequiresUnreferencedCode(
    "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
[RequiresDynamicCode(
    "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic " +
    "constraints), trimming can't validate that the requirements of those annotations are met.")]
public class ViewModelViewHost<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes
        .PublicParameterlessConstructor)]
TViewModel> : ViewModelViewHost, IViewFor<TViewModel>
    where TViewModel : class
{
    /// <summary>Gets or sets the view model whose associated view is to be displayed.</summary>
    public new TViewModel? ViewModel
    {
        get => (TViewModel?)base.ViewModel;
        set => base.ViewModel = value;
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }

    /// <summary>
    /// Resolves and displays the view for the given view model with respect to the contract.
    /// This method uses the generic ResolveView method which is AOT-compatible.
    /// </summary>
    /// <param name="viewModel">The view model to resolve a view for.</param>
    /// <param name="contract">The contract to use when resolving the view.</param>
    /// <remarks>
    /// This method is excluded from code coverage because it is only exercised by the inherited runtime subscription wiring,
    /// which is skipped during unit tests due to the <see cref="ModeDetector.InUnitTestRunner"/> check.
    /// This code is exercised in integration tests and production runtime scenarios.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic " +
        "constraints), trimming can't validate that the requirements of those annotations are met.")]
    protected override void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is not null and not TViewModel)
        {
            throw new InvalidOperationException(
                $"View model '{viewModel.GetType().FullName}' is not assignable to '{typeof(TViewModel).FullName}'.");
        }

        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;

        // Use the generic ResolveView<TViewModel> method - this is AOT-safe!
        var viewInstance = viewLocator.ResolveView<TViewModel>(contract);
        if (viewInstance is null && !ContractFallbackByPass)
        {
            viewInstance = viewLocator.ResolveView<TViewModel>();
        }

        if (viewInstance is null)
        {
            throw new InvalidOperationException($"Couldn't find view for '{viewModel}'.");
        }

        if (viewInstance is not View castView)
        {
            throw new InvalidOperationException(
                $"View '{viewInstance.GetType().FullName}' is not a subclass of '{typeof(View).FullName}'.");
        }

        viewInstance.ViewModel = (TViewModel)viewModel;

        Content = castView;
    }
}
