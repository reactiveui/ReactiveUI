// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    /// <inheritdoc />
    /// <summary>
    /// iOS platform assemblies and events.
    /// </summary>
    /// <seealso cref="BasePlatform" />
    // ReSharper disable once InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SA1300 // Element should begin with upper-case letter
    public class iOS : BasePlatform
#pragma warning restore SA1300 // Element should begin with upper-case letter
#pragma warning restore IDE1006 // Naming Styles
    {
        private readonly string _referenceAssembliesLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="iOS"/> class.
        /// </summary>
        /// <param name="referenceAssembliesLocation">The reference assemblies location.</param>
        public iOS(string referenceAssembliesLocation)
        {
            _referenceAssembliesLocation = referenceAssembliesLocation;
        }

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.iOS;

        /// <inheritdoc />
        public override Task Extract()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                var assembly =
                    @"/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
            }
            else
            {
                var assemblies =
                    Directory.GetFiles(
                        Path.Combine(_referenceAssembliesLocation, "Xamarin.iOS"),
                        "Xamarin.iOS.dll",
                        SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }

            return Task.CompletedTask;
        }
    }
}
