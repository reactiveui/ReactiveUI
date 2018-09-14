using System;

namespace ReactiveUI.Avalonia
{
    public class Registrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        }
    }
}