using System;
using System.Collections.Generic;

namespace EventBuilder.Platforms
{
    public class WPA81 : BasePlatform
    {
        public WPA81()
        {
            Assemblies = new List<string>();
            CecilSearchDirectories = new List<string>();

            if (PlatformHelper.IsRunningOnMono())
            {
                throw new NotSupportedException("Building events for WPA81 on Mac is not implemented yet.");

            }
            Assemblies.Add(
                @"C:\Program Files (x86)\Windows Phone Kits\8.1\References\CommonConfiguration\Neutral\Windows.winmd");
        }
    }
}