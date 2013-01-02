using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Routing
{
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register()
        {
            RxApp.Register(typeof(AutoDataTemplateBindingHook), typeof(IPropertyBindingHook));
        }
    }
}
