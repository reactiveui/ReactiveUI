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
            var platforms = new[] { 
                "ReactiveUI.Xaml", 
                "ReactiveUI.Winforms",
                "ReactiveUI.Gtk", 
                "ReactiveUI.Cocoa", 
                "ReactiveUI.Android",
            };

            var extraNs = new[] {
                "ReactiveUI",
                "ReactiveUI.NLog", 
                "ReactiveUI.Mobile", 
            };

            var fdr = typeof(ModernDependencyResolver);

            var assmName = new AssemblyName(
                fdr.AssemblyQualifiedName.Replace(fdr.FullName + ", ", ""));

            var platDllCount = platforms.Count(x => processRegistrationForNamespace(x, assmName, resolver) == true);
            if (platDllCount == 0) {
                LogHost.Default.Warn("We couldn't load a Platform DLL. This probably means you need to Install-Package ReactiveUI-Platforms on your App");
            }

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
