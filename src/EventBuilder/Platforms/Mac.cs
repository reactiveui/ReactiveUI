// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    // ReSharper disable once InconsistentNaming
    public class Mac : BasePlatform
    {
        public Mac(string referenceAssembliesLocation)
        {
            if (PlatformHelper.IsRunningOnMono()) {
                var assembly =
                    @"/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
            } else {
                var assemblies =
                    Directory.GetFiles(Path.Combine(referenceAssembliesLocation, "Xamarin.Mac"),

                        "Xamarin.Mac.dll", SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }
        }
    }
}