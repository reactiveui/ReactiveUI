using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// Represents a dependency resolver, a service to look up global class 
    /// instances or types.
    /// </summary>
    public interface IDependencyResolver : IDisposable
    {
        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>. Must return <c>null</c>
        /// if the service is not available (must not throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <returns>The requested object, if found; <c>null</c> otherwise.</returns>
        object GetService(Type serviceType, string contract = null);

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>. Must return an empty
        /// collection if the service is not available (must not return <c>null</c> or throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>. The sequence
        /// should be empty (not <c>null</c>) if no objects of the given type are available.</returns>
        IEnumerable<object> GetServices(Type serviceType, string contract = null);
    }

    /// <summary>
    /// Represents a dependency resolver where types can be registered after 
    /// setup.
    /// </summary>
    public interface IMutableDependencyResolver : IDependencyResolver
    {
        void Register(Func<object> factory, Type serviceType, string contract = null);
    }

    public static class DependencyResolverMixins
    {
        /// <summary>
        /// Gets an instance of the given <paramref name="serviceType"/>. Must return <c>null</c>
        /// if the service is not available (must not throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <returns>The requested object, if found; <c>null</c> otherwise.</returns>
        public static T GetService<T>(this IDependencyResolver This, string contract = null)
        {
            return (T)This.GetService(typeof(T), contract);
        }

        /// <summary>
        /// Gets all instances of the given <paramref name="serviceType"/>. Must return an empty
        /// collection if the service is not available (must not return <c>null</c> or throw).
        /// </summary>
        /// <param name="serviceType">The object type.</param>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>. The sequence
        /// should be empty (not <c>null</c>) if no objects of the given type are available.</returns>
        public static IEnumerable<T> GetServices<T>(this IDependencyResolver This, string contract = null)
        {
            return This.GetServices(typeof(T), contract).Cast<T>();
        }

        /// <summary>
        /// This method allows you to initialize resolvers with the default 
        /// ReactiveUI types. All resolvers used as the default 
        /// RxApp.DependencyResolver
        /// </summary>
        /// <param name="resolver">The resolver to initialize.</param>
        public static void InitializeResolver(this IMutableDependencyResolver resolver)
        {
            var namespaces = new[] { 
                "ReactiveUI",
                "ReactiveUI.Xaml", 
                "ReactiveUI.Winforms",
                "ReactiveUI.Mobile", 
                "ReactiveUI.NLog", 
                "ReactiveUI.Gtk", 
                "ReactiveUI.Cocoa", 
                "ReactiveUI.Android",
            };

            var fdr = typeof(ModernDependencyResolver);

            var assmName = new AssemblyName(
                fdr.AssemblyQualifiedName.Replace(fdr.FullName + ", ", ""));

            RxApp.suppressLogging = true;

            namespaces.ForEach(ns => {
                var targetType = ns + ".Registrations";
                string fullName = targetType + ", " + assmName.FullName.Replace(assmName.Name, ns);

                var registerTypeClass = Reflection.ReallyFindType(fullName, false);
                if (registerTypeClass == null) return;

                var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass);
                registerer.Register((f, t) => resolver.Register(f, t));
            });

            RxApp.suppressLogging = false;
        }

        /// <summary>
        /// Override the default Dependency Resolver until the object returned 
        /// is disposed.
        /// </summary>
        /// <param name="resolver">The test resolver to use.</param>
        public static IDisposable WithResolver(this IDependencyResolver resolver)
        {
            var origResolver = RxApp.DependencyResolver;
            RxApp.DependencyResolver = resolver;

            return Disposable.Create(() => RxApp.DependencyResolver = origResolver);
        }
    }

    /// <summary>
    /// A simple dependency resolver which takes Funcs for all its actions.
    /// GetService is always implemented via GetServices().LastOrDefault()
    /// </summary>
    public class FuncDependencyResolver : IMutableDependencyResolver
    {
        readonly Func<Type, string, IEnumerable<object>> innerGetServices;
        readonly Action<Func<object>, Type, string> innerRegister;

        public FuncDependencyResolver(Func<Type, string, IEnumerable<object>> getAllServices, Action<Func<object>, Type, string> register = null)
        {
            innerGetServices = getAllServices;
            innerRegister = register;
        }

        public object GetService(Type serviceType, string contract = null)
        {
            return (GetServices(serviceType, contract) ?? Enumerable.Empty<object>()).LastOrDefault();
        }

        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            return innerGetServices(serviceType, contract);
        }

        public void Dispose()
        {
        }

        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            if (innerRegister == null) throw new NotImplementedException();
            innerRegister(factory, serviceType, contract);
        }
    }
}
