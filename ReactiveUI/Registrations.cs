using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {            
            registerFunction(() => new INPCObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new IRNPCObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new POCOObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new NullDefaultPropertyBindingProvider(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new EqualityTypeConverter(), typeof(IImplicitBindingTypeConverter));
            registerFunction(() => new StringConverter(), typeof(IImplicitBindingTypeConverter));
            registerFunction(() => new DefaultViewLocator(), typeof(IViewLocator));
            registerFunction(() => new DefaultLogManager(), typeof(ILogManager));
        }
    }
}
