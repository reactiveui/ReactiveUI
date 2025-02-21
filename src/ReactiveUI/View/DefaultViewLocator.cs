// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Default implementation for <see cref="IViewLocator"/>. The default <see cref="ViewModelToViewFunc"/>
/// behavior is to replace instances of "View" with "ViewMode" in the Fully Qualified Name of the ViewModel type.
/// </summary>
#if NET6_0_OR_GREATER
[RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
[RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
[Preserve]
#endif
public sealed class DefaultViewLocator : IViewLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
    /// </summary>
    /// <param name="viewModelToViewFunc">The method which will convert a ViewModel name into a View.</param>
    internal DefaultViewLocator(Func<string, string>? viewModelToViewFunc = null) =>
        ViewModelToViewFunc = viewModelToViewFunc ?? (vm => vm.Replace("ViewModel", "View"));

    /// <summary>
    /// Gets or sets a function that is used to convert a view model name to a proposed view name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If unset, the default behavior is to change "ViewModel" to "View". If a different convention is followed, assign an appropriate function to this
    /// property.
    /// </para>
    /// <para>
    /// Note that the name returned by the function is a starting point for view resolution. Variants on the name will be resolved according to the rules
    /// set out by the <see cref="ResolveView{T}"/> method.
    /// </para>
    /// </remarks>
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
    [RequiresDynamicCode("The method uses reflection and will not work in AOT environments.")]
    [RequiresUnreferencedCode("The method uses reflection and will not work in AOT environments.")]
    [Preserve]
#endif
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        viewModel.ArgumentNullExceptionThrowIfNull(nameof(viewModel));

        var view = AttemptViewResolutionFor(viewModel!.GetType(), contract);

        if (view is not null)
        {
            return view;
        }

        view = AttemptViewResolutionFor(typeof(T), contract);

        if (view is not null)
        {
            return view;
        }

        view = AttemptViewResolutionFor(ToggleViewModelType(viewModel.GetType()), contract);

        if (view is not null)
        {
            return view;
        }

        view = AttemptViewResolutionFor(ToggleViewModelType(typeof(T)), contract);

        if (view is not null)
        {
            return view;
        }

        this.Log().Warn(CultureInfo.InvariantCulture, "Failed to resolve view for view model type '{0}'.", typeof(T).FullName);
        return null;
    }

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
    [RequiresDynamicCode("The method is used to resolve views for view models.")]
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

    private IViewFor? AttemptViewResolution(string? viewTypeName, string? contract)
    {
        try
        {
            var viewType = Reflection.ReallyFindType(viewTypeName, throwOnFailure: false);
            if (viewType is null)
            {
                return null;
            }

            var service = Locator.Current.GetService(viewType, contract);

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
