// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// TV OS platform assemblies and events.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    // ReSharper disable once InconsistentNaming
    public class TVOS : BasePlatform
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TVOS"/> class.
        /// </summary>
        /// <param name="referenceAssembliesLocation">The reference assemblies location.</param>
        public TVOS(string referenceAssembliesLocation)
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                var assembly =
                    @"/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS10/Xamarin.TVOS10.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
            }
            else
            {
                var assemblies =
                    Directory.GetFiles(
                        Path.Combine(referenceAssembliesLocation, "Xamarin.TVOS"),
                        "Xamarin.TVOS.dll",
                        SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }
        }

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.TVOS;
    }
}
