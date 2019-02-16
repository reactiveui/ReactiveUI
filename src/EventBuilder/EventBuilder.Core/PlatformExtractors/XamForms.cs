// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventBuilder.Core.NuGet;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Serilog;

namespace EventBuilder.Core.PlatformExtractors
{
    /// <summary>
    /// Xamarin Forms assemblies and events.
    /// </summary>
    public class XamForms : BasePlatform
    {
        private readonly PackageIdentity[] _packageNames = new[]
        {
            new PackageIdentity("Xamarin.Forms", new NuGetVersion("3.4.0.1029999")),
            new PackageIdentity("NetStandard.Library", new NuGetVersion("2.0.3")),
        };

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.XamForms;

        /// <inheritdoc />
        public override async Task Extract(string referenceAssembliesLocation)
        {
            var packageUnzipPath = await NuGetPackageHelper.InstallPackages(_packageNames, Platform).ConfigureAwait(false);

            Log.Debug($"Package unzip path is {packageUnzipPath}");

            var files = Directory.GetFiles(packageUnzipPath, "Xamarin.Forms.Core.dll", SearchOption.AllDirectories);
            files = files.Concat(Directory.GetFiles(packageUnzipPath, "Xamarin.Forms.Xaml.dll", SearchOption.AllDirectories)).ToArray();

            Assemblies.Add(files.First(x => x.Contains("netstandard2.0")));

            foreach (var directory in Directory.GetDirectories(packageUnzipPath, "*.*", SearchOption.AllDirectories))
            {
                SearchDirectories.Add(directory);
            }
        }
    }
}
