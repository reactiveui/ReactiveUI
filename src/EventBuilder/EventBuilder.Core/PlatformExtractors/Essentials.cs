// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventBuilder.Core.NuGet;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Serilog;

namespace EventBuilder.Core.PlatformExtractors
{
    /// <summary>
    /// Xamarin Essentials  platform.
    /// </summary>
    public class Essentials : BasePlatform
    {
        private readonly PackageIdentity[] _packageNames = new[]
        {
            new PackageIdentity("Xamarin.Essentials", new NuGetVersion("1.0.1")),
            new PackageIdentity("NetStandard.Library", new NuGetVersion("2.0.3")),
        };

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.Essentials;

        /// <inheritdoc />
        public override async Task Extract(string referenceAssembliesLocation)
        {
            var packageUnzipPath = await NuGetPackageHelper.InstallPackages(_packageNames, Platform, FrameworkConstants.CommonFrameworks.NetStandard20).ConfigureAwait(false);

            Log.Debug($"Package unzip path is {packageUnzipPath}");

            var xamarinForms =
                Directory.GetFiles(
                    packageUnzipPath,
                    "Xamarin.Essentials.dll",
                    SearchOption.AllDirectories);

            var latestVersion = xamarinForms.First(x => x.Contains("netstandard2.0"));
            Assemblies.Add(latestVersion);

            foreach (var directory in Directory.GetDirectories(packageUnzipPath, "*.*", SearchOption.AllDirectories))
            {
                SearchDirectories.Add(directory);
            }
        }
    }
}
