using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class ModernDependencyResolver : IMutableDependencyResolver
    {
        private Dictionary<Tuple<Type, string>, List<Func<object>>> _registry;

        public ModernDependencyResolver() : this(null) { }

        protected ModernDependencyResolver(Dictionary<Tuple<Type, string>, List<Func<object>>> registry)
        {
            _registry = registry != null ? 
                registry.ToDictionary(k => k.Key, v => v.Value.ToList()) :
                new Dictionary<Tuple<Type, string>, List<Func<object>>>();
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
            return this.GetServices(serviceType, contract).LastOrDefault();
        }
 
        public IEnumerable<object> GetServices(Type serviceType, string contract = null)
        {
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair)) return Enumerable.Empty<object>();
 
            return _registry[pair].Select(x => x());
        }

        public ModernDependencyResolver Duplicate()
        {
            return new ModernDependencyResolver(_registry);
        }

        public void Dispose()
        {
            _registry = null;
        }
    }

    public static class MutableDependencyResolverMixins
    {
        public static void RegisterConstant(this IMutableDependencyResolver This, object value, Type serviceType, string contract = null)
        {
            This.Register(() => value, serviceType, contract);
        }

        public static void RegisterLazySingleton(this IMutableDependencyResolver This, Func<object> valueFactory, Type serviceType, string contract = null)
        {
            var val = new Lazy<object>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            This.Register(() => val.Value, serviceType, contract);
        }
    }
}
