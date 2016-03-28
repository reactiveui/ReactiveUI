using System;

namespace EventBuilder.Platforms
{
    public class WP81 : BasePlatform
    {
        public WP81()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                throw new NotSupportedException("Building events for WP81 on Mac is not implemented yet.");
            }
            Assemblies.Add(
                @"C:\Program Files (x86)\Windows Phone Silverlight Kits\8.1\Windows MetaData\Windows.winmd");
        }
    }
}