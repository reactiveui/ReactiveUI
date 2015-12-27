using System;

namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            if (Splat.ModeDetector.InUnitTestRunner()) return;
            throw new Exception("You are referencing the Portable version of ReactiveUI in an App. Reference the platform-specific version.");
        }
    }
}
