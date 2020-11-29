// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Default implementation for <see cref="IViewLocator"/>. The default <see cref="ViewModelToViewFunc"/>
    /// behavior is to replace instances of "View" with "ViewMode" in the Fully Qualified Name of the ViewModel type.
    /// </summary>
    public sealed class DefaultViewLocator : IViewLocator, IEnableLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
        /// </summary>
        /// <param name="viewModelToViewFunc">The method which will convert a ViewModel name into a View.</param>
        [SuppressMessage("Globalization", "CA1307: operator could change based on locale settings", Justification = "Replace() does not have third parameter on all platforms")]
        internal DefaultViewLocator(Func<string, string>? viewModelToViewFunc = null) => ViewModelToViewFunc = viewModelToViewFunc ?? (vm => vm.Replace("ViewModel", "View"));

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
        public IViewFor? ResolveView<T>(T viewModel, string? contract = null)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            var view = AttemptViewResolutionFor(viewModel.GetType(), contract);

            if (view is not null)
            {
                return view;
            }

            view = AttemptViewResolutionFor(typeof(T), contract);

            if (view is not null)
            {
                return view;
            }

            view = AttemptViewResolutionFor(ToggleViewModelType(viewModel?.GetType()), contract);

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

        private static Type? ToggleViewModelType(Type? viewModelType)
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

            if (viewModelType.GetTypeInfo().IsInterface)
            {
                if (viewModelType.Name.StartsWith("I", StringComparison.InvariantCulture))
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
            var idxComma = typeName.IndexOf(",", 0, StringComparison.InvariantCulture);
            var idxPeriod = typeName.LastIndexOf('.', idxComma - 1);
            return typeName.Substring(0, idxPeriod + 1) + typeName.Substring(idxPeriod + 2);
        }

        private static string InterfaceifyTypeName(string typeName)
        {
            var idxComma = typeName.IndexOf(",", 0, StringComparison.InvariantCulture);
            var idxPeriod = typeName.LastIndexOf(".", idxComma - 1, StringComparison.InvariantCulture);
            return typeName.Insert(idxPeriod + 1, "I");
        }

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
            view = AttemptViewResolution(proposedViewTypeName, contract);

            if (view is not null)
            {
                return view;
            }

            return null;
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
                if (service is null)
                {
                    return null;
                }

                if (!(service is IViewFor view))
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
}
