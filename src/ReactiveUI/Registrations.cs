using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// Registrations class
    /// </summary>
    /// <seealso cref="ReactiveUI.IWantsToRegisterStuff"/>
    public class Registrations : IWantsToRegisterStuff
    {
        /// <summary>
        /// Registers the specified register function.
        /// </summary>
        /// <param name="registerFunction">The register function.</param>
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new INPCObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new IROObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new POCOObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new EqualityTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new StringConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new DefaultViewLocator(), typeof(IViewLocator));
            registerFunction(() => new CanActivateViewFetcher(), typeof(IActivationForViewFetcher));
            registerFunction(() => new CreatesCommandBindingViaEvent(), typeof(ICreatesCommandBinding));
            registerFunction(() => new CreatesCommandBindingViaCommandParameter(), typeof(ICreatesCommandBinding));
        }
    }
}