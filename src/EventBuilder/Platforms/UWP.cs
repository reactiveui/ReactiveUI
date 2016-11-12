using System;

namespace EventBuilder.Platforms
{
    public class UWP : BasePlatform
    {
        public UWP()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                throw new NotSupportedException("Building events for UWP on Mac is not implemented yet.");
            }
            Assemblies.Add(@"C:\Program Files (x86)\Windows Kits\10\UnionMetadata\Windows.winmd");
        }
    }
}