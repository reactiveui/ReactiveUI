using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(IMutableDependencyResolver resolver)
        {
            resolver.Register<ICreatesObservableForProperty>(() => new INPCObservableForProperty());
            resolver.Register<ICreatesObservableForProperty>(() => new IRNPCObservableForProperty());
            resolver.Register<ICreatesObservableForProperty>(() => new POCOObservableForProperty());
            resolver.Register<IDefaultPropertyBindingProvider>(() => new NullDefaultPropertyBindingProvider());
            resolver.Register<IBindingTypeConverter>(() => new EqualityTypeConverter());
            resolver.Register<IBindingTypeConverter>(() => new StringConverter());
        }
    }
}
