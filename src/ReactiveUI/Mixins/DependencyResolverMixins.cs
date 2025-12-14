// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using Splat.Builder;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the IMutableDependencyResolver interface.
/// </summary>
[Preserve(AllMembers = true)]
public static class DependencyResolverMixins
{
    /// <summary>
    /// This method allows you to initialize resolvers with the default
    /// ReactiveUI types. All resolvers used as the default
    /// AppLocator.Current.
    /// If no namespaces are passed in, all registrations will be checked.
    /// </summary>
    /// <param name="resolver">The resolver to initialize.</param>
    /// <param name="registrationNamespaces">Which platforms to use.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("InitializeReactiveUI uses reflection to locate types which may be trimmed.")]
    [RequiresUnreferencedCode("InitializeReactiveUI uses reflection to locate types which may be trimmed.")]
#endif
    public static void InitializeReactiveUI(this IMutableDependencyResolver resolver, params RegistrationNamespace[] registrationNamespaces)
    {
        if (AppBuilder.UsingBuilder && !ModeDetector.InUnitTestRunner() && ReferenceEquals(resolver, AppLocator.CurrentMutable))
        {
            // If the builder has been used for the default resolver in a non-test environment,
            // do not re-register defaults via reflection for AppLocator.CurrentMutable.
            return;
        }

        resolver.ArgumentNullExceptionThrowIfNull(nameof(resolver));
        registrationNamespaces.ArgumentNullExceptionThrowIfNull(nameof(registrationNamespaces));

        var possibleNamespaces = new Dictionary<RegistrationNamespace, string>
        {
            { RegistrationNamespace.Winforms, "ReactiveUI.Winforms" },
            { RegistrationNamespace.Wpf, "ReactiveUI.Wpf" },
            { RegistrationNamespace.Uno, "ReactiveUI.Uno" },
            { RegistrationNamespace.UnoWinUI, "ReactiveUI.Uno.WinUI" },
            { RegistrationNamespace.Blazor, "ReactiveUI.Blazor" },
            { RegistrationNamespace.Drawing, "ReactiveUI.Drawing" },
            { RegistrationNamespace.Maui, "ReactiveUI.Maui" },
            { RegistrationNamespace.Uwp, "ReactiveUI.Uwp" },
            { RegistrationNamespace.WinUI, "ReactiveUI.WinUI" },
        };

        if (registrationNamespaces.Length == 0)
        {
            registrationNamespaces = PlatformRegistrationManager.DefaultRegistrationNamespaces;
        }

        var extraNs =
            registrationNamespaces
                .Where(ns => possibleNamespaces.ContainsKey(ns))
                .Select(ns => possibleNamespaces[ns])
                .ToArray();

        // Set up the built-in registration
        new Registrations().Register((f, t) => resolver.RegisterConstant(f(), t));
#if NET6_0_OR_GREATER
        new PlatformRegistrations().Register((f, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] t) => resolver.RegisterConstant(f(), t));
#else
        new PlatformRegistrations().Register((f, t) => resolver.RegisterConstant(f(), t));
#endif

        var fdr = typeof(DependencyResolverMixins);

        var assemblyName = new AssemblyName(fdr.AssemblyQualifiedName!.Replace(fdr.FullName + ", ", string.Empty));

        foreach (var ns in extraNs)
        {
            ProcessRegistrationForNamespace(ns, assemblyName, resolver);
        }
    }

    /// <summary>
    /// Registers inside the Splat dependency container all the classes that derive off
    /// IViewFor using Reflection. This is a easy way to register all the Views
    /// that are associated with View Models for an entire assembly.
    /// </summary>
    /// <param name="resolver">The dependency injection resolver to register the Views with.</param>
    /// <param name="assembly">The assembly to search using reflection for IViewFor classes.</param>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("RegisterViewsForViewModels scans the provided assembly and creates instances via reflection; this is not compatible with AOT.")]
    [RequiresUnreferencedCode("RegisterViewsForViewModels uses reflection over types and members which may be trimmed.")]
#endif
    public static void RegisterViewsForViewModels(this IMutableDependencyResolver resolver, Assembly assembly)
    {
        resolver.ArgumentNullExceptionThrowIfNull(nameof(resolver));
        assembly.ArgumentNullExceptionThrowIfNull(nameof(assembly));

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

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("RegisterType creates instances via Activator.CreateInstance which requires dynamic code generation in AOT.")]
    [RequiresUnreferencedCode("RegisterType uses reflection to locate parameterless constructors which may be trimmed.")]
#endif
    private static void RegisterType(IMutableDependencyResolver resolver, TypeInfo ti, Type serviceType, string contract)
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

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("TypeFactory uses reflection to invoke parameterless constructors which may be trimmed.")]
#endif
    private static Func<object> TypeFactory(
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
        TypeInfo typeInfo)
    {
        var parameterlessConstructor = typeInfo.DeclaredConstructors.FirstOrDefault(ci => ci.IsPublic && ci.GetParameters().Length == 0);
        return parameterlessConstructor is null
            ? throw new Exception($"Failed to register type {typeInfo.FullName} because it's missing a parameterless constructor.")
            : () => Activator.CreateInstance(typeInfo.AsType())
                   ?? throw new Exception($"Failed to instantiate type {typeInfo.FullName} - ensure it has a public parameterless constructor.");
    }

#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("ProcessRegistrationForNamespace uses reflection to locate types which may be trimmed.")]
    [RequiresDynamicCode("Calls ReactiveUI.IWantsToRegisterStuff.Register(Action<Func<Object>, Type>)")]
#endif
    private static void ProcessRegistrationForNamespace(string namespaceName, AssemblyName assemblyName, IMutableDependencyResolver resolver)
    {
        var targetTypeName = namespaceName + ".Registrations";

        // Preferred path: find the target assembly by simple name among loaded assemblies
        var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == namespaceName);
        Type? registerTypeClass = null;
        if (asm is null)
        {
            try
            {
                asm = Assembly.Load(new AssemblyName(namespaceName));
            }
            catch
            {
                asm = null;
            }
        }

        if (asm is not null)
        {
            registerTypeClass = asm.GetType(targetTypeName, throwOnError: false, ignoreCase: false);
        }

        // Fallback to legacy lookup using full name synthesis
        if (registerTypeClass is null && assemblyName.Name is not null)
        {
            var fullName = targetTypeName + ", " + assemblyName.FullName.Replace(assemblyName.Name, namespaceName);
            registerTypeClass = Reflection.ReallyFindType(fullName, false);
        }

        if (registerTypeClass is not null)
        {
            var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass)!;
#if NET6_0_OR_GREATER
            registerer.Register((f, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] t) => resolver.RegisterConstant(f(), t));
#else
            registerer.Register((f, t) => resolver.RegisterConstant(f(), t));
#endif
        }
    }
}
