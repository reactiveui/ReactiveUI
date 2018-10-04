using System;
using Avalonia.Threading;

namespace ReactiveUI.Avalonia
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            registerFunction(() => new AvaloniaObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
        }
    }
}
