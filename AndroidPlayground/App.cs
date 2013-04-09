using System;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Routing;

namespace AndroidPlayground
{
    public class App
    {
        static App _Current;
        public static App Current {
            get { return (_Current = _Current ?? new App()); }
        }

        public FuncServiceLocator Locator { get; protected set; }

        protected App()
        {
            var locator = new FuncServiceLocator();

            RxApp.ConfigureServiceLocator(
                (t,s) => locator.GetAllServices(t,s).FirstOrDefault(),
                (t,s) => locator.GetAllServices(t,s).ToArray(),
                (c,t,s) => locator.Register(() => Activator.CreateInstance(c), t, s));

            locator.Register(() => typeof(MainView), typeof(IViewFor<MainViewModel>));
            locator.Register(() => typeof(SecondaryView), typeof(IViewFor<SecondaryViewModel>));

            RxApp.Register(typeof(AppBootstrapper), typeof(IApplicationRootState));

            Locator = locator;
        }
    }

    public class FuncServiceLocator
    {
        readonly Dictionary<Tuple<Type, string>, List<Func<object>>> _registry = new Dictionary<Tuple<Type, string>, List<Func<object>>>();

        public void Register(Func<object> factory, Type type, string contract = null)
        {
            var pair = Tuple.Create(type, contract ?? "");
            if (!_registry.ContainsKey(pair)) _registry[pair] = new List<Func<object>>();

            _registry[pair].Add(factory);
        }

        public IEnumerable<object> GetAllServices(Type type, string contract = null)
        {
            var pair = Tuple.Create(type, contract ?? "");
            if (!_registry.ContainsKey(pair)) {
                return Enumerable.Empty<object>();
            }

            return _registry[pair].Select(x => x());
        }

        public void ClearRegistration(Type type, string contract = null)
        {
            var pair = Tuple.Create(type, contract ?? "");
            if (_registry.ContainsKey(pair)) {
                _registry.Remove(pair);
            }
        }
    }
}

