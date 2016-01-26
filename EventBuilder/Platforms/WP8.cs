using System;

namespace EventBuilder.Platforms
{
    public class WP8 : BasePlatform
    {
        public WP8()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                throw new NotSupportedException("Building events for WP8 on Mac is not implemented for obvious reasons.");
            }
            Assemblies.Add(
                @"C:\Program Files (x86)\Windows Phone Silverlight Kits\8.1\Windows MetaData\Windows.winmd");
        }
    }
}