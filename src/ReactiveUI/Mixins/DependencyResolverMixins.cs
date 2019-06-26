// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
        /// </summary>
        /// <param name="resolver">The resolver to initialize.</param>
        [SuppressMessage("Globalization", "CA1307: operator could change based on locale settings", Justification = "Replace() does not have third parameter on all platforms")]
        public static void InitializeReactiveUI(this IMutableDependencyResolver resolver)
        {
            var extraNs = new[]
            {
                "ReactiveUI.XamForms",
                "ReactiveUI.Winforms",
                "ReactiveUI.Wpf",
                "ReactiveUI.Uno"
            };

            // Set up the built-in registration
            new Registrations().Register((f, t) => resolver.RegisterConstant(f(), t));
            new PlatformRegistrations().Register((f, t) => resolver.RegisterConstant(f(), t));

            var fdr = typeof(DependencyResolverMixins);

            var assemblyName = new AssemblyName(fdr.AssemblyQualifiedName.Replace(fdr.FullName + ", ", string.Empty));

            extraNs.ForEach(ns => ProcessRegistrationForNamespace(ns, assemblyName, resolver));
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
            // for each type that implements IViewFor
            foreach (var ti in assembly.DefinedTypes
                .Where(ti => ti.ImplementedInterfaces.Contains(typeof(IViewFor)))
                .Where(ti => !ti.IsAbstract))
            {
                // grab the first _implemented_ interface that also implements IViewFor, this should be the expected IViewFor<>
                var ivf = ti.ImplementedInterfaces.FirstOrDefault(t => t.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IViewFor)));

                // need to check for null because some classes may implement IViewFor but not IViewFor<T> - we don't care about those
                if (ivf != null)
                {
                    // my kingdom for c# 6!
                    var contractSource = ti.GetCustomAttribute<ViewContractAttribute>();
                    var contract = contractSource != null ? contractSource.Contract : string.Empty;

                    RegisterType(resolver, ti, ivf, contract);
                }
            }
        }

        private static void RegisterType(IMutableDependencyResolver resolver, TypeInfo ti, Type serviceType, string contract)
        {
            var factory = TypeFactory(ti);
            if (ti.GetCustomAttribute<SingleInstanceViewAttribute>() != null)
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
#if PORTABLE && !WASM
            throw new Exception("You are referencing the Portable version of ReactiveUI in an App. Reference the platform-specific version.");
#else
            return Expression.Lambda<Func<object>>(Expression.New(
                typeInfo.DeclaredConstructors.First(ci => ci.IsPublic && !ci.GetParameters().Any()))).Compile();
#endif
        }

        [SuppressMessage("Globalization", "CA1307: operator could change based on locale settings", Justification = "Replace() does not have third parameter on all platforms")]
        private static void ProcessRegistrationForNamespace(string ns, AssemblyName assemblyName, IMutableDependencyResolver resolver)
        {
            var targetType = ns + ".Registrations";
            var fullName = targetType + ", " + assemblyName.FullName.Replace(assemblyName.Name, ns);

            var registerTypeClass = Reflection.ReallyFindType(fullName, false);
            if (registerTypeClass != null)
            {
                var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass);
                registerer.Register((f, t) => resolver.RegisterConstant(f(), t));
            }
        }
    }
}
