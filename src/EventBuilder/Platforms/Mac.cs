// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// Mac platform assemblies and events.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    // ReSharper disable once InconsistentNaming
    public class Mac : BasePlatform
    {
        private readonly string _referenceAssembliesLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mac"/> class.
        /// </summary>
        /// <param name="referenceAssembliesLocation">The reference assemblies location.</param>
        public Mac(string referenceAssembliesLocation)
        {
            _referenceAssembliesLocation = referenceAssembliesLocation;
        }

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.Mac;

        /// <inheritdoc />
        public override Task Extract()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                var assembly =
                    @"/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
            }
            else
            {
                var assemblies =
                    Directory.GetFiles(
                        Path.Combine(_referenceAssembliesLocation, "Xamarin.Mac"),
                        "Xamarin.Mac.dll",
                        SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }

            return Task.CompletedTask;
        }
    }
}
