using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using ReactiveUI;
using System.Reactive.Concurrency;

namespace ReactiveUI.QuickUI
{
    /// <summary>
    /// Ignore me. This class is a secret handshake between RxUI and RxUI.Xaml
    /// in order to register certain classes on startup that would be difficult
    /// to register otherwise.
    /// </summary>
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            //TODO
            //registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));

            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            registerFunction(() => new XamlDefaultPropertyBinding(), typeof(IDefaultPropertyBindingProvider));
            registerFunction(() => new CreatesCommandBindingViaCommandParameter(), typeof(ICreatesCommandBinding));
            registerFunction(() => new CreatesCommandBindingViaEvent(), typeof(ICreatesCommandBinding));
            //TODO:
            //registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
        }
    }
}
