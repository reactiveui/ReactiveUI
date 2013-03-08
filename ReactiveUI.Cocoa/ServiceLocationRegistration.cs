using System;

namespace ReactiveUI.Cocoa
{
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register ()
        {
            RxApp.Register (typeof(KVOObservableForProperty), typeof(ICreatesObservableForProperty));
        }
    }
}