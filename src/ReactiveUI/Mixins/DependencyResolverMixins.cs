// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the IMutableDependencyResolver interface.
/// </summary>
[Preserve(AllMembers = true)]
public static class DependencyResolverMixins
{
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
                var contract = contractSource?.Contract;

                RegisterType(resolver, ti, ivf, contract);
            }
        }
    }

    /// <summary>
    /// Registers a type with the specified dependency resolver, using singleton or transient lifetime based on the
    /// type's attributes and an optional contract.
    /// </summary>
    /// <remarks>If the implementation type is marked with the SingleInstanceViewAttribute, it is registered
    /// as a singleton; otherwise, it is registered as transient. The contract parameter allows multiple registrations
    /// of the same service type under different contracts.</remarks>
    /// <param name="resolver">The dependency resolver with which to register the type. Cannot be null.</param>
    /// <param name="ti">The type information for the implementation to register. Must provide accessible constructors as required by the
    /// registration process.</param>
    /// <param name="serviceType">The service type to associate with the registration. This is the type that will be resolved from the dependency
    /// resolver.</param>
    /// <param name="contract">An optional contract string that distinguishes this registration from others of the same service type. If null,
    /// the registration is not associated with a contract.</param>
    private static void RegisterType(
        IMutableDependencyResolver resolver,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TypeInfo ti,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        string? contract)
    {
        var factory = TypeFactory(ti);
        var isSingleton = ti.GetCustomAttribute<SingleInstanceViewAttribute>() is not null;

        if (isSingleton && contract is not null)
        {
            resolver.RegisterLazySingleton(factory, serviceType, contract);
        }
        else if (isSingleton)
        {
            resolver.RegisterLazySingleton(factory, serviceType);
        }
        else if (contract is not null)
        {
            resolver.Register(factory, serviceType, contract);
        }
        else
        {
            resolver.Register(factory, serviceType);
        }
    }

    /// <summary>
    /// Creates a factory delegate that instantiates objects of the specified type using a public parameterless
    /// constructor.
    /// </summary>
    /// <param name="typeInfo">The type metadata for which to create the factory. The type must have a public parameterless constructor.</param>
    /// <returns>A delegate that creates a new instance of the specified type when invoked.</returns>
    /// <exception cref="Exception">Thrown if the specified type does not have a public parameterless constructor, or if instantiation fails.</exception>
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
