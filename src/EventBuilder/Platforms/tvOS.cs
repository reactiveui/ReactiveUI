// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// TV OS platform assemblies and events.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    // ReSharper disable once InconsistentNaming
    public class TVOS : BasePlatform
    {
        private readonly string _referenceAssembliesLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TVOS"/> class.
        /// </summary>
        /// <param name="referenceAssembliesLocation">The reference assemblies location.</param>
        public TVOS(string referenceAssembliesLocation)
        {
            _referenceAssembliesLocation = referenceAssembliesLocation;
        }

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.TVOS;

        /// <inheritdoc />
        public override Task Extract()
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
                        Path.Combine(_referenceAssembliesLocation, "Xamarin.TVOS"),
                        "Xamarin.TVOS.dll",
                        SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }

            return Task.CompletedTask;
        }
    }
}
