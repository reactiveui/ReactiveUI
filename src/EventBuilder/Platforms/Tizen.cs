// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading.Tasks;
using EventBuilder.NuGet;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Serilog;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// Tizen platform assemblies and events.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    public class Tizen : BasePlatform
    {
        private readonly PackageIdentity[] _packageNames = new[]
        {
            new PackageIdentity("Tizen.Net", new NuGetVersion("5.0.0.14562")),
            new PackageIdentity("NetStandard.Library", new NuGetVersion("2.0.0")),
        };

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.Tizen4;

        /// <inheritdoc />
        public async override Task Extract()
        {
            var packageUnzipPath = await NuGetPackageHelper.InstallPackages(_packageNames, Platform, FrameworkConstants.CommonFrameworks.Tizen4).ConfigureAwait(false);

            Log.Debug($"Package unzip path is {packageUnzipPath}");

            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "ElmSharp*.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "Tizen*.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "netstandard.dll", SearchOption.AllDirectories));

            foreach (var directory in Directory.GetDirectories(packageUnzipPath, "*.*", SearchOption.AllDirectories))
            {
                CecilSearchDirectories.Add(directory);
            }
        }
    }
}
