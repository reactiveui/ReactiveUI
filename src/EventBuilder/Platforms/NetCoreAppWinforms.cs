// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
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
    /// Win Forms platform assemblies and events for netcoreapp3.0 and above.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    public class NetCoreAppWinforms : BasePlatform
    {
        private readonly PackageIdentity[] _packageNames = new[]
        {
            new PackageIdentity("Microsoft.WindowsDesktop.App", new NuGetVersion("3.0.0-alpha-27128-4")),
            new PackageIdentity("NetStandard.Library", new NuGetVersion("2.0.0")),
        };

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.NetCoreAppWinforms;

        /// <inheritdoc />
        public async override Task Extract()
        {
            var packageUnzipPath = await NuGetPackageHelper.InstallPackages(_packageNames, Platform).ConfigureAwait(false);

            Log.Debug($"Package unzip path is {packageUnzipPath}");

            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.Data.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.DirectoryServices.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.Drawing.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.Messaging.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.Windows.Forms.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.Windows.Forms.DataVisualization.dll", SearchOption.AllDirectories));
            Assemblies.AddRange(Directory.GetFiles(packageUnzipPath, "System.ServiceProcess.dll", SearchOption.AllDirectories));

            foreach (var directory in Directory.GetDirectories(packageUnzipPath, "*.*", SearchOption.AllDirectories))
            {
                CecilSearchDirectories.Add(directory);
            }
        }
    }
}
