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
/// public sealed class ConventionViewLocator : IViewLocator
/// {
///     public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
///     {
///         if (viewModel is null)
///         {
///             return null;
///         }
///
///         var viewTypeName = viewModel.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
///         var viewType = Type.GetType(viewTypeName);
///         return viewType is null ? null : (IViewFor?)Activator.CreateInstance(viewType);
///     }
/// }
/// ]]>
/// </code>
/// </example>
public interface IViewLocator : IEnableLogger
{
    /// <summary>
    /// Determines the view for an associated view model.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="viewModel">The view model for which a view is required.</param>
    /// <param name="contract">Optional contract allowing multiple view registrations per view model.</param>
    /// <returns>The resolved view or <see langword="null"/> when no registration is available.</returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ResolveView uses reflection and type discovery which require dynamic code generation")]
    [RequiresUnreferencedCode("ResolveView uses reflection and type discovery which may require unreferenced code")]
#endif
    IViewFor? ResolveView<T>(T? viewModel, string? contract = null);
}
