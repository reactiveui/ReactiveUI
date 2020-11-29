// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with the IMutableDependencyResolver interface.
    /// </summary>
    public static class DependencyResolverMixins
    {
        /// <summary>
        /// This method allows you to initialize resolvers with the default
        /// ReactiveUI types. All resolvers used as the default
        /// Locator.Current.
        /// If no namespaces are passed in, all registrations will be checked.
        /// </summary>
        /// <param name="resolver">The resolver to initialize.</param>
        /// <param name="registrationNamespaces">Which platforms to use.</param>
        [SuppressMessage("Globalization", "CA1307: operator could change based on locale settings", Justification = "Replace() does not have third parameter on all platforms")]
        public static void InitializeReactiveUI(this IMutableDependencyResolver resolver, params RegistrationNamespace[] registrationNamespaces)
        {
            var possibleNamespaces = new Dictionary<RegistrationNamespace, string>
            {
                { RegistrationNamespace.XamForms, "ReactiveUI.XamForms" },
                { RegistrationNamespace.Winforms, "ReactiveUI.Winforms" },
                { RegistrationNamespace.Wpf, "ReactiveUI.Wpf" },
                { RegistrationNamespace.Uno, "ReactiveUI.Uno" },
                { RegistrationNamespace.Blazor, "ReactiveUI.Blazor" },
                { RegistrationNamespace.Drawing, "ReactiveUI.Drawing" }
            };

            if (registrationNamespaces.Length == 0)
            {
                registrationNamespaces = PlatformRegistrationManager.DefaultRegistrationNamespaces;
            }

            var extraNs =
                possibleNamespaces
                    .Where(kvp => registrationNamespaces.Contains(kvp.Key))
                    .Select(kvp => kvp.Value)
                    .ToArray();

            // Set up the built-in registration
            new Registrations().Register((f, t) => resolver.RegisterConstant(f(), t));
            new PlatformRegistrations().Register((f, t) => resolver.RegisterConstant(f(), t));

            var fdr = typeof(DependencyResolverMixins);

            var assemblyName = new AssemblyName(fdr?.AssemblyQualifiedName?.Replace(fdr?.FullName + ", ", string.Empty)!);

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
        public static void RegisterViewsForViewModels(this IMutableDependencyResolver resolver, Assembly assembly)
        {
            if (resolver is null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            // for each type that implements IViewFor
            foreach (var ti in assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)) && !ti.IsAbstract))
            {
                // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
                var ivf = ti.ImplementedInterfaces.FirstOrDefault(t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));

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

        [SuppressMessage("Redundancy", "CA1801: Redundant parameter", Justification = "Used on some platforms")]
        private static Func<object> TypeFactory(TypeInfo typeInfo)
        {
            var parameterlessConstructor = typeInfo.DeclaredConstructors.FirstOrDefault(ci => ci.IsPublic && !ci.GetParameters().Any());
            if (parameterlessConstructor is null)
            {
                throw new Exception($"Failed to register type {typeInfo.FullName} because it's missing a parameterless constructor.");
            }

            return Expression.Lambda<Func<object>>(Expression.New(parameterlessConstructor)).Compile();
        }

        [SuppressMessage("Globalization", "CA1307: operator could change based on locale settings", Justification = "Replace() does not have third parameter on all platforms")]
        private static void ProcessRegistrationForNamespace(string namespaceName, AssemblyName assemblyName, IMutableDependencyResolver resolver)
        {
            var targetType = namespaceName + ".Registrations";
            if (assemblyName.Name is not null)
            {
                var fullName = targetType + ", " + assemblyName.FullName.Replace(assemblyName.Name, namespaceName);

                var registerTypeClass = Reflection.ReallyFindType(fullName, false);
                if (registerTypeClass is not null)
                {
                    var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass)!;
                    registerer?.Register((f, t) => resolver.RegisterConstant(f(), t));
                }
            }
        }
    }
}
