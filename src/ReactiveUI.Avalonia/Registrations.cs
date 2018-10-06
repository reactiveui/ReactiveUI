using System;
using Avalonia.Threading;

namespace ReactiveUI.Avalonia
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
        }
    }
}
