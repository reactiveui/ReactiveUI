// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Default implementation of <see cref="IViewLocator"/> that resolves views by convention (replacing "ViewModel" with "View").
/// </summary>
/// <remarks>
/// <para>
/// This locator queries Splat's service locator for a registered <see cref="IViewFor"/> using several fallbacks, including
/// the exact view type, <c>IViewFor&lt;TViewModel&gt;</c>, and interface/class naming conversions. Override
/// <see cref="ViewModelToViewFunc"/> to customize the name translation strategy.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
///
/// var locator = new DefaultViewLocator();
/// var view = locator.ResolveView(new LoginViewModel());
/// ]]>
/// </code>
/// </example>
public sealed partial class DefaultViewLocator : IViewLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
    /// </summary>
    /// <param name="viewModelToViewFunc">Custom mapping from view model type name to view type name.</param>
    internal DefaultViewLocator(Func<string, string>? viewModelToViewFunc = null) =>
        ViewModelToViewFunc = viewModelToViewFunc ?? (static vm => vm.Replace("ViewModel", "View"));

    /// <summary>
    /// Gets or sets the function used to convert a view model type name into a view type name during resolution.
    /// </summary>
    /// <value>
    /// The view model to view function.
    /// </value>
    public Func<string, string> ViewModelToViewFunc { get; set; }

    /// <summary>
    /// Returns the view associated with a view model, deriving the name of the type via <see cref="ViewModelToViewFunc"/>, then discovering it via the
    /// service locator.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <remarks>
    /// <para>
    /// Given view model type <c>T</c> with runtime type <c>RT</c>, this implementation will attempt to resolve the following views:
    /// <list type="number">
    /// <item>
    /// <description>
    /// Look for a service registered under the type whose name is given to us by passing <c>RT</c> to <see cref="ViewModelToViewFunc"/> (which defaults to changing "ViewModel" to "View").
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Look for a service registered under the type <c>IViewFor&lt;RT&gt;</c>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Look for a service registered under the type whose name is given to us by passing <c>T</c> to <see cref="ViewModelToViewFunc"/> (which defaults to changing "ViewModel" to "View").
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Look for a service registered under the type <c>IViewFor&lt;T&gt;</c>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If <c>T</c> is an interface, change its name to that of a class (i.e. drop the leading "I"). If it's a class, change to an interface (i.e. add a leading "I").
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Repeat steps 1-4 with the type resolved from the modified name.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <param name="viewModel">
    /// The view model whose associated view is to be resolved.
    /// </param>
    /// <param name="contract">
    /// Optional contract to be used when resolving from Splat.
    /// </param>
    /// <returns>
    /// The view associated with the given view model.
    /// </returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("View resolution uses reflection and type discovery")]
    [RequiresUnreferencedCode("View resolution may reference types that could be trimmed")]
#endif
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        viewModel.ArgumentNullExceptionThrowIfNull(nameof(viewModel));

        var mapped = TryResolveAOTMapping(viewModel!.GetType(), contract);
        if (mapped is not null)
        {
            return mapped;
        }

        var view = AttemptViewResolutionFor(viewModel!.GetType(), contract)
                   ?? AttemptViewResolutionFor(typeof(T), contract)
                   ?? AttemptViewResolutionFor(ToggleViewModelType(viewModel.GetType()), contract)
                   ?? AttemptViewResolutionFor(ToggleViewModelType(typeof(T)), contract);

        if (view is not null)
        {
            return view;
        }

        this.Log().Warn(CultureInfo.InvariantCulture, "Failed to resolve view for view model type '{0}'.", typeof(T).FullName);
        return null;
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Type resolution requires dynamic code generation")]
    [RequiresUnreferencedCode("Type resolution may reference types that could be trimmed")]
#endif
    private static Type? ToggleViewModelType(Type viewModelType)
    {
        var viewModelTypeName = viewModelType.AssemblyQualifiedName;

        if (viewModelTypeName is null)
        {
            return null;
        }

        if (viewModelType.GetTypeInfo().IsInterface)
        {
#if NET6_0_OR_GREATER
            if (viewModelType.Name.StartsWith('I'))
#else
            if (viewModelType.Name.StartsWith("I", StringComparison.InvariantCulture))
#endif
            {
                var toggledTypeName = DeinterfaceifyTypeName(viewModelTypeName);
                return Reflection.ReallyFindType(toggledTypeName, throwOnFailure: false);
            }
        }
        else
        {
            var toggledTypeName = InterfaceifyTypeName(viewModelTypeName);
            return Reflection.ReallyFindType(toggledTypeName, throwOnFailure: false);
        }

        return null;
    }

    private static string DeinterfaceifyTypeName(string typeName)
    {
        var idxComma = typeName.IndexOf(',', 0);
        var idxPeriod = typeName.LastIndexOf('.', idxComma - 1);
#if NET6_0_OR_GREATER
        return string.Concat(typeName.AsSpan(0, idxPeriod + 1), typeName.AsSpan(idxPeriod + 2));
#else
        return typeName.Substring(0, idxPeriod + 1) + typeName.Substring(idxPeriod + 2);
#endif
    }

    private static string InterfaceifyTypeName(string typeName)
    {
        var idxComma = typeName.IndexOf(',', 0);
        var idxPeriod = typeName.LastIndexOf('.', idxComma - 1);
        return typeName.Insert(idxPeriod + 1, "I");
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("View resolution uses reflection and type discovery")]
    [RequiresUnreferencedCode("View resolution may reference types that could be trimmed")]
#endif
    private IViewFor? AttemptViewResolutionFor(Type? viewModelType, string? contract)
    {
        if (viewModelType is null)
        {
            return null;
        }

        var viewModelTypeName = viewModelType.AssemblyQualifiedName;

        if (viewModelTypeName is null)
        {
            return null;
        }

        var proposedViewTypeName = ViewModelToViewFunc(viewModelTypeName);
        var view = AttemptViewResolution(proposedViewTypeName, contract);

        if (view is not null)
        {
            return view;
        }

        proposedViewTypeName = typeof(IViewFor<>).MakeGenericType(viewModelType).AssemblyQualifiedName;
        return AttemptViewResolution(proposedViewTypeName, contract);
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("View resolution uses reflection and type discovery")]
    [RequiresUnreferencedCode("View resolution may reference types that could be trimmed")]
#endif
    private IViewFor? AttemptViewResolution(string? viewTypeName, string? contract)
    {
        try
        {
            var viewType = Reflection.ReallyFindType(viewTypeName, throwOnFailure: false);
            if (viewType is null)
            {
                // this.Log().Debug(CultureInfo.InvariantCulture, "Failed to find type '{0}'", viewTypeName);
                return null;
            }

            var service = AppLocator.Current?.GetService(viewType, contract);

            if (service is null)
            {
                 return null;
            }

            if (service is not IViewFor view)
            {
                return null;
            }

            this.Log().Debug(CultureInfo.InvariantCulture, "Resolved service type '{0}'", viewType.FullName);

            return view;
        }
        catch (Exception ex)
        {
            this.Log().Error(ex, $"Exception occurred whilst attempting to resolve type {viewTypeName} into a view.");
            throw;
        }
    }
}
