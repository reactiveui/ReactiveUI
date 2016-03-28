using System;
using System.IO;

namespace EventBuilder.Platforms
{
    public class Mac : BasePlatform
    {
        public Mac()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                var assembly = @"/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/XamMac.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
                CecilSearchDirectories.Add("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5");
            }
            else
            {
                throw new NotSupportedException("Building events for Xamarin.Mac on Windows is not implemented yet.");
            }
        }
    }
}