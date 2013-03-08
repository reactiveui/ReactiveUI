using System;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace ReactiveUI.Cocoa
{
    public class ServiceLocationRegistration : IWantsToRegisterStuff
    {
        public void Register ()
        {
            RxApp.Register(typeof(KVOObservableForProperty), typeof(ICreatesObservableForProperty));
            RxApp.Register(typeof(CocoaDefaultPropertyBinding), typeof(IDefaultPropertyBindingProvider));
            RxApp.Register(typeof(TargetActionCommandBinder), typeof(ICreatesCommandBinding));
        }
    }
}