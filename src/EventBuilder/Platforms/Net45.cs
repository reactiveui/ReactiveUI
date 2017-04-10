using System;

namespace EventBuilder.Platforms
{
    public class Net45 : BasePlatform
    {
        public Net45()
        {
            if (PlatformHelper.IsRunningOnMono()) {
                throw new NotSupportedException("Building events for NET45 on Mac is not implemented yet.");
            } else {
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\WindowsBase.dll");
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\PresentationCore.dll");
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\PresentationFramework.dll");

                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");
            }
        }
    }
}
