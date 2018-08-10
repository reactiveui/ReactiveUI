// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    // ReSharper disable once InconsistentNaming
    public class iOS : BasePlatform
    {
        public override AutoPlatform Platform => AutoPlatform.iOS;

        public iOS(string referenceAssembliesLocation)
        {
            if (PlatformHelper.IsRunningOnMono()) {
                var assembly =
                    @"/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
            } else {
                var assemblies =
                    Directory.GetFiles(Path.Combine(referenceAssembliesLocation, "Xamarin.iOS"),
                        "Xamarin.iOS.dll", SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }
        }
    }
}