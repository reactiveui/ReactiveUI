using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Splat;

namespace ReactiveUI
{
    public static class DependencyResolverMixins
    {
        /// <summary>
        /// This method allows you to initialize resolvers with the default 
        /// ReactiveUI types. All resolvers used as the default 
        /// Locator.Current
        /// </summary>
        /// <param name="resolver">The resolver to initialize.</param>
        public static void InitializeReactiveUI(this IMutableDependencyResolver resolver)
        {
            var extraNs = new[] {
                "ReactiveUI.XamForms",
                "ReactiveUI.Winforms", 
            };

            // Set up the built-in registration
            (new Registrations()).Register((f,t) => resolver.RegisterConstant(f(), t));
            (new PlatformRegistrations()).Register((f,t) => resolver.RegisterConstant(f(), t));

            var fdr = typeof(ModernDependencyResolver);

            var assmName = new AssemblyName(
                fdr.AssemblyQualifiedName.Replace(fdr.FullName + ", ", ""));

            extraNs.ForEach(ns => processRegistrationForNamespace(ns, assmName, resolver));
        }

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
                if (ivf != null) {
                    // my kingdom for c# 6!
                    var contractSource = ti.GetCustomAttribute<ViewContractAttribute>();
                    var contract = contractSource != null ? contractSource.Contract : string.Empty;

                    registerType(resolver, ti, ivf, contract);
                }
            }
        }

        public static void Register<TService>(this IMutableDependencyResolver resolver, Func<object> factory, string contract = null)
        {
            resolver.Register(factory, typeof(TService), contract);
        }

        public static void RegisterConstant<TService>(this IMutableDependencyResolver resolver, TService value, string contract = null)
        {
            resolver.RegisterConstant(value, typeof(TService), contract);
        }

        static void registerType(IMutableDependencyResolver resolver, TypeInfo ti, Type serviceType, string contract)
        {
            var factory = typeFactory(ti);
            if (ti.GetCustomAttribute<SingleInstanceViewAttribute>() != null)
            {
                resolver.RegisterLazySingleton(factory, serviceType, contract);
            }
            else
            {
                resolver.Register(factory, serviceType, contract);
            }
        }

        static Func<object> typeFactory(TypeInfo typeInfo)
        {
#if PORTABLE
            throw new Exception("You are referencing the Portable version of ReactiveUI in an App. Reference the platform-specific version.");
#else
            return Expression.Lambda<Func<object>>(Expression.New(
                typeInfo.DeclaredConstructors.First(ci => ci.IsPublic && !ci.GetParameters().Any()))).Compile();
#endif
        }

        static void processRegistrationForNamespace(string ns, AssemblyName assmName, IMutableDependencyResolver resolver)
        {
            var targetType = ns + ".Registrations";
            var fullName = targetType + ", " + assmName.FullName.Replace(assmName.Name, ns);

            var registerTypeClass = Reflection.ReallyFindType(fullName, false);
            if (registerTypeClass != null) {
            var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass);
            registerer.Register((f, t) => resolver.RegisterConstant(f(), t));
        }
    }
}
}
