// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Splat;

namespace ReactiveUI;

/// <summary>Extension methods associated with the IMutableDependencyResolver interface.</summary>
[Preserve(AllMembers = true)]
public static class DependencyResolverMixins
{
    /// <summary>Provides view-registration extension members for <see cref="IMutableDependencyResolver"/>.</summary>
    /// <param name="resolver">The dependency injection resolver to register the Views with.</param>
    extension(IMutableDependencyResolver resolver)
    {
        /// <summary>
        /// Registers inside the Splat dependency container all the classes that derive off
        /// IViewFor using Reflection. This is a easy way to register all the Views
        /// that are associated with View Models for an entire assembly.
        /// </summary>
        /// <param name="assembly">The assembly to search using reflection for IViewFor classes.</param>
        [RequiresUnreferencedCode(
            "Scans assembly for IViewFor implementations using reflection. For AOT compatibility, use the ReactiveUIBuilder pattern to register views explicitly.")]
        public void RegisterViewsForViewModels(Assembly assembly)
        {
            ArgumentExceptionHelper.ThrowIfNull(resolver);
            ArgumentExceptionHelper.ThrowIfNull(assembly);

            foreach (var ti in assembly.DefinedTypes)
            {
                if (ti.IsAbstract || ti.GetCustomAttribute<ExcludeFromViewRegistrationAttribute>() is not null)
                {
                    continue;
                }

                if (!TryResolveViewForInterface(ti, out var ivf))
                {
                    continue;
                }

                var contractSource = ti.GetCustomAttribute<ViewContractAttribute>();
                var contract = contractSource?.Contract;

                RegisterType(resolver, ti, ivf, contract);
            }
        }
    }

    /// <summary>Creates a factory delegate that instantiates objects of the specified type using a public parameterless constructor.</summary>
    /// <param name="typeInfo">The type metadata for which to create the factory. The type must have a public parameterless constructor.</param>
    /// <returns>A delegate that creates a new instance of the specified type when invoked.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the specified type does not have a public parameterless constructor, or if instantiation fails.</exception>
    /// <remarks>Internal so the missing-parameterless-constructor guard can be exercised directly in tests.</remarks>
    internal static Func<object> TypeFactory(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TypeInfo typeInfo)
    {
        ConstructorInfo? parameterlessConstructor = null;
        foreach (var ci in typeInfo.DeclaredConstructors)
        {
            if (ci.IsPublic && ci.GetParameters().Length == 0)
            {
                parameterlessConstructor = ci;
                break;
            }
        }

        return parameterlessConstructor is null
            ? throw new InvalidOperationException(
                $"Failed to register type {typeInfo.FullName} because it's missing a parameterless constructor.")
            : () => Activator.CreateInstance(typeInfo.AsType())
                    ?? throw new InvalidOperationException(
                        $"Failed to instantiate type {typeInfo.FullName} - ensure it has a public parameterless constructor.");
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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TypeInfo ti,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type serviceType,
        string? contract)
    {
        var factory = TypeFactory(ti);
        switch (ti.GetCustomAttribute<SingleInstanceViewAttribute>() is not null)
        {
            case true when contract is not null:
                {
                    resolver.RegisterLazySingleton(factory, serviceType, contract);
                    break;
                }

            case true:
                {
                    resolver.RegisterLazySingleton(factory, serviceType);
                    break;
                }

            default:
                {
                    if (contract is not null)
                    {
                        resolver.Register(factory, serviceType, contract);
                    }
                    else
                    {
                        resolver.Register(factory, serviceType);
                    }

                    break;
                }
        }
    }

    /// <summary>
    /// In a single pass over the implemented interfaces, confirms the type derives from <see cref="IViewFor"/>
    /// and captures the concrete <c>IViewFor&lt;T&gt;</c> interface to register against.
    /// </summary>
    /// <param name="ti">The candidate type's metadata.</param>
    /// <param name="viewForInterface">The resolved <c>IViewFor&lt;T&gt;</c> interface when the type is registrable.</param>
    /// <returns><see langword="true"/> when the type is a non-marker view that can be registered.</returns>
    [RequiresUnreferencedCode("Inspects implemented interfaces via reflection.")]
    private static bool TryResolveViewForInterface(TypeInfo ti, [NotNullWhen(true)] out Type? viewForInterface)
    {
        var implementsViewFor = false;
        viewForInterface = null;
        foreach (var iface in ti.ImplementedInterfaces)
        {
            if (iface == typeof(IViewFor))
            {
                implementsViewFor = true;
                continue;
            }

            if (viewForInterface is null && ImplementsInterface(iface.GetTypeInfo().ImplementedInterfaces, typeof(IViewFor)))
            {
                viewForInterface = iface;
            }
        }

        return implementsViewFor && viewForInterface is not null;
    }

    /// <summary>Determines whether a sequence of interface types contains the target interface.</summary>
    /// <param name="interfaces">The implemented interface types to search.</param>
    /// <param name="target">The interface type to look for.</param>
    /// <returns><see langword="true"/> when the target interface is present; otherwise <see langword="false"/>.</returns>
    private static bool ImplementsInterface(IEnumerable<Type> interfaces, Type target)
    {
        foreach (var iface in interfaces)
        {
            if (iface == target)
            {
                return true;
            }
        }

        return false;
    }
}
