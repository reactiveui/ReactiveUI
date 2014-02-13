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

            namespaces.ForEach(ns => {
                var targetType = ns + ".Registrations";
                string fullName = targetType + ", " + assmName.FullName.Replace(assmName.Name, ns);

                var registerTypeClass = Reflection.ReallyFindType(fullName, false);
                if (registerTypeClass == null) return;

                var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass);
                registerer.Register((f, t) => resolver.RegisterConstant(f(), t));
            });
        }
    }
}
