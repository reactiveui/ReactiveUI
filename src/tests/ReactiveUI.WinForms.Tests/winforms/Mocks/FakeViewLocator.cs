// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A fake view locator that resolves views using a configurable delegate.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S4018:Generic methods should provide type parameters",
    Justification = "IViewLocator declares parameterless generic ResolveView overloads that this mock must implement.")]
internal sealed class FakeViewLocator : IViewLocator
{
    /// <summary>
    /// Gets or sets the delegate used to resolve a view from a view model type.
    /// </summary>
    public Func<Type, IViewFor>? LocatorFunc { get; set; }

    /// <inheritdoc/>
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract)
        where TViewModel : class =>
        LocatorFunc?.Invoke(typeof(TViewModel)) as IViewFor<TViewModel>;

    /// <inheritdoc/>
    public IViewFor<TViewModel>? ResolveView<TViewModel>()
        where TViewModel : class =>
        ResolveView<TViewModel>(null);

    /// <inheritdoc/>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
        "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public IViewFor? ResolveView(object? instance, string? contract)
    {
        if (instance is null)
        {
            return null;
        }

        var view = LocatorFunc?.Invoke(instance.GetType());
        if (view is IViewFor viewFor)
        {
            viewFor.ViewModel = instance;
        }

        return view;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
        "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public IViewFor? ResolveView(object? instance) => ResolveView(instance, null);
}
