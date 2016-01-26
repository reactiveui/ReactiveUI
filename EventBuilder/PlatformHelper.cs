using System;

namespace EventBuilder
{
    public static class PlatformHelper
    {
        private static readonly Lazy<bool> _IsRunningOnMono = new Lazy<bool>(() => Type.GetType("Mono.Runtime") != null);

        public static bool IsRunningOnMono()
        {
            return _IsRunningOnMono.Value;
        }
    }
}