﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public interface IDependencyResolver : IDisposable
    {
        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>. Must return <c>null</c>
        /// if the service is not available (must not throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <returns>The requested object, if found; <c>null</c> otherwise.</returns>
        ServiceType GetService<ServiceType>(string contract = null);

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>. Must return an empty
        /// collection if the service is not available (must not return <c>null</c> or throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>. The sequence
        /// should be empty (not <c>null</c>) if no objects of the given type are available.</returns>
        IEnumerable<ServiceType> GetServices<ServiceType>(string contract = null);
    }

    public interface IMutableDependencyResolver : IDependencyResolver
    {
        void Register<ServiceType>(Func<ServiceType> factory, string contract = null);
    }

    public static class DependencyResolverMixins
    {
        public static void InitializeResolver(this IMutableDependencyResolver resolver)
        {
            new Registrations().Register(resolver);

            var namespaces = new[] { 
                "ReactiveUI.Xaml", 
                "ReactiveUI.Mobile", 
                "ReactiveUI.NLog", 
                "ReactiveUI.Gtk", 
                "ReactiveUI.Cocoa", 
                "ReactiveUI.Android",
            };

            var fdr = typeof(ModernDependencyResolver);

            var assmName = new AssemblyName(
                fdr.AssemblyQualifiedName.Replace(fdr.FullName + ", ", ""));

            namespaces.ForEach(ns => {
                var targetType = ns + ".Registrations";
                string fullName = targetType + ", " + assmName.FullName.Replace(assmName.Name, ns);

                var registerTypeClass = Reflection.ReallyFindType(fullName, false);
                if (registerTypeClass == null) return;

                var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass);
                registerer.Register(resolver);
            });
        }

        public static IDisposable WithResolver(this IDependencyResolver resolver)
        {
            var origResolver = RxApp.DependencyResolver;
            RxApp.DependencyResolver = resolver;

            return Disposable.Create(() => RxApp.DependencyResolver = origResolver);
        }
    }

    public class FuncDependencyResolver : IMutableDependencyResolver
    {
        readonly Func<Type, string, IEnumerable<object>> innerGetServices;
        readonly Action<Func<object>, Type, string> innerRegister;

        public FuncDependencyResolver(Func<Type, string, IEnumerable<object>> getAllServices, Action<Func<object>, Type, string> register = null)
        {
            innerGetServices = getAllServices;
            innerRegister = register;
        }

        public ServiceType GetService<ServiceType>(string contract = null)
        {
            return (GetServices<ServiceType>(contract) ?? Enumerable.Empty<ServiceType>()).FirstOrDefault();
        }

        public IEnumerable<ServiceType> GetServices<ServiceType>(string contract = null)
        {
            Type serviceType = typeof(ServiceType);
            return innerGetServices(serviceType, contract).Cast<ServiceType>();
        }

        public void Dispose()
        {
        }

        public void Register<ServiceType>(Func<ServiceType> factory, string contract = null)
        {
            if (innerRegister == null) throw new NotImplementedException();
            Type serviceType = typeof(ServiceType);
            innerRegister(() => factory(), serviceType, contract);
        }
    }
}
