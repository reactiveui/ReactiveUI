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

        public void Register<ServiceType>(Func<ServiceType> factory, string contract = null)
        {
            Type serviceType = typeof(ServiceType);
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair)) {
                _registry[pair] = new List<Func<object>>();
            }
 
            _registry[pair].Add(() => factory());
        }

        public ServiceType GetService<ServiceType>(string contract = null)
        {
            return this.GetServices<ServiceType>(contract).FirstOrDefault();
        }

        public IEnumerable<ServiceType> GetServices<ServiceType>(string contract = null)
        {
            Type serviceType = typeof(ServiceType);
            var pair = Tuple.Create(serviceType, contract ?? string.Empty);
            if (!_registry.ContainsKey(pair)) return Enumerable.Empty<ServiceType>();
 
            return _registry[pair].Select(x => (ServiceType)x());
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
        public static void RegisterConstant<ServiceType>(this IMutableDependencyResolver This, ServiceType value, string contract = null)
        {
            This.Register<ServiceType>(() => value, contract);
        }

        public static void RegisterLazySingleton<ServiceType>(this IMutableDependencyResolver This, Func<ServiceType> valueFactory, string contract = null)       
        {
            var val = new Lazy<ServiceType>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            This.Register<ServiceType>(() => val.Value, contract);
        }
    }
}
