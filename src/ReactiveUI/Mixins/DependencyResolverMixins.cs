// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

using ReactiveUI.Builder;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the IMutableDependencyResolver interface.
/// </summary>
[Preserve(AllMembers = true)]
public static class DependencyResolverMixins
{
    /// <summary>
    /// Initializes static members of the <see cref="DependencyResolverMixins"/> class.
    /// </summary>
    static DependencyResolverMixins() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// Registers inside the Splat dependency container all the classes that derive off
    /// IViewFor using Reflection. This is a easy way to register all the Views
    /// that are associated with View Models for an entire assembly.
    /// </summary>
    /// <param name="resolver">The dependency injection resolver to register the Views with.</param>
    /// <param name="assembly">The assembly to search using reflection for IViewFor classes.</param>
    [RequiresUnreferencedCode("Scans assembly for IViewFor implementations using reflection. For AOT compatibility, use the ReactiveUIBuilder pattern to register views explicitly.")]
    public static void RegisterViewsForViewModels(this IMutableDependencyResolver resolver, Assembly assembly)
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);
        ArgumentExceptionHelper.ThrowIfNull(assembly);

        // for each type that implements IViewFor
        foreach (var ti in assembly.DefinedTypes
                                   .Where(static ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)) && !ti.IsAbstract))
        {
            // Skip types explicitly marked to be excluded from auto view registration
            if (ti.GetCustomAttribute<ExcludeFromViewRegistrationAttribute>() is not null)
            {
                continue;
            }

            // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>`
            var ivf = ti.ImplementedInterfaces.FirstOrDefault(static t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));

            // need to check for null because some classes may implement IViewFor but not IViewFor<T> - we don't care about those
            if (ivf is not null)
            {
                // my kingdom for c# 6!
                var contractSource = ti.GetCustomAttribute<ViewContractAttribute>();
                var contract = contractSource is not null ? contractSource.Contract : string.Empty;

                RegisterType(resolver, ti, ivf, contract);
            }
        }
    }

    private static void RegisterType(
        IMutableDependencyResolver resolver,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TypeInfo ti,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        string contract)
    {
        var factory = TypeFactory(ti);
        if (ti.GetCustomAttribute<SingleInstanceViewAttribute>() is not null)
        {
            resolver.RegisterLazySingleton(factory, serviceType, contract);
        }
        else
        {
            resolver.Register(factory, serviceType, contract);
        }
    }

    private static Func<object> TypeFactory(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TypeInfo typeInfo)
    {
        var parameterlessConstructor = typeInfo.DeclaredConstructors.FirstOrDefault(ci => ci.IsPublic && ci.GetParameters().Length == 0);
        return parameterlessConstructor is null
            ? throw new Exception($"Failed to register type {typeInfo.FullName} because it's missing a parameterless constructor.")
            : () => Activator.CreateInstance(typeInfo.AsType())
                   ?? throw new Exception($"Failed to instantiate type {typeInfo.FullName} - ensure it has a public parameterless constructor.");
    }
}
