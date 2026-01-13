// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Implement this to override how RoutedViewHost and ViewModelViewHost map view models to views.
/// </summary>
/// <remarks>
/// <para>
/// A locator typically consults an IoC container to find an <see cref="IViewFor"/> implementation for the supplied
/// view model. Implementations may use contracts to differentiate between platform-specific or themed view
/// registrations.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// public sealed class CustomViewLocator : IViewLocator
/// {
///     private readonly Dictionary<Type, Func<IViewFor>> _mappings = new();
///
///     public void Register<TViewModel, TView>(Func<TView> factory)
///         where TViewModel : class
///         where TView : class, IViewFor<TViewModel>
///     {
///         _mappings[typeof(TViewModel)] = () => factory();
///     }
///
///     public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
///         where TViewModel : class
///     {
///         return _mappings.TryGetValue(typeof(TViewModel), out var factory)
///             ? (IViewFor<TViewModel>)factory()
///             : null;
///     }
/// }
/// ]]>
/// </code>
/// </example>
public interface IViewLocator : IEnableLogger
{
    /// <summary>
    /// Resolves a view for a view model type known at compile time. Fully AOT-compatible.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type to resolve a view for.</typeparam>
    /// <param name="contract">Optional contract allowing multiple view registrations per view model.</param>
    /// <returns>The resolved view or <see langword="null"/> when no registration is available.</returns>
    IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
        where TViewModel : class;

    /// <summary>
    /// Resolves a view for a view model instance using runtime type information.
    /// </summary>
    /// <param name="instance">The view model instance to resolve a view for.</param>
    /// <param name="contract">Optional contract allowing multiple view registrations per view model.</param>
    /// <returns>The resolved view or <see langword="null"/> when no registration is available.</returns>
    [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    IViewFor? ResolveView(object? instance, string? contract = null);
}
