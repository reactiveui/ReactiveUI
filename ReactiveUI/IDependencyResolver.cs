using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                "ReactiveUI.Mobile", 
                "ReactiveUI.QuickUI",
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

        static bool processRegistrationForNamespace(string ns, AssemblyName assmName, IMutableDependencyResolver resolver)
        {
            var targetType = ns + ".Registrations";
            string fullName = targetType + ", " + assmName.FullName.Replace(assmName.Name, ns);

            var registerTypeClass = Reflection.ReallyFindType(fullName, false);
            if (registerTypeClass == null) return false;

            var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass);
            registerer.Register((f, t) => resolver.RegisterConstant(f(), t));

            return true;
        }
    }
}
