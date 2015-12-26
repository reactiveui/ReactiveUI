using System;
using System.Linq;
using System.Reflection;

namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            if (IsRunningFromNUnit()) return;
            throw new Exception("You are referencing the Portable version of ReactiveUI in an App. Reference the platform-specific version.");
        }

        private bool IsRunningFromNUnit()
        {
            var currentdomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
            var getassemblies = currentdomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[]{ });
            var assemblies = getassemblies.Invoke(currentdomain, new object[]{ }) as Assembly[];
            return assemblies.Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("nunit.framework"));
        }
    }
}
