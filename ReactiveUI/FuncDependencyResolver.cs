using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class FuncDependencyResolver : IDependencyResolver
    {
        private Dictionary<Tuple<Type, string>, List<Func<object>>> _registry;

        /// <summary>
        /// Default dependency resolver when no other one is registered.
        /// </summary>
        public FuncDependencyResolver()
        {
            Registrations.Register((f,t) => Register(f,t));

            var namespaces = 
#if PORTABLE
            new[] { "ReactiveUI.Xaml", "ReactiveUI.Mobile", "ReactiveUI.NLog", };
#else
            new[] { "ReactiveUI.Xaml", "ReactiveUI.Mobile", "ReactiveUI.NLog", "ReactiveUI.Gtk", "ReactiveUI.Cocoa", "ReactiveUI.Android" };
#endif
            var assmName = new AssemblyName(typeof(FuncDependencyResolver).AssemblyQualifiedName);
            namespaces.ForEach(ns =>
            {
                var targetType = ns + ".Registrations";
                string fullName = targetType + ", " + assmName.FullName.Replace(assmName.Name, ns);

                var registerTypeClass = Reflection.ReallyFindType(fullName, false);
                if (registerTypeClass != null)
                {
                    var registerer = (IWantsToRegisterStuff)Activator.CreateInstance(registerTypeClass);
                    registerer.Register((f, t) => Register(f, t));
                }
            });
        }

        public void Register(Func<object> factory, Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair)) {
                _registry[pair] = new List<Func<object>>();
            }
 
            _registry[pair].Add(factory);
        }
        
        public object GetService(Type serviceType, string contract = null)
        {
            return this.GetServices(serviceType, contract).FirstOrDefault();
        }
 
        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair)) return Enumerable.Empty<object>();
 
            return _registry[pair].Select(x => x());
        }

        public void Dispose()
        {
            _registry = null;
        }
    }
}
