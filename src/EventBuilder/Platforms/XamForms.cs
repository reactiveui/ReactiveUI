// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventBuilder.NuGet;
using NuGet;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Polly;
using Serilog;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// Xamarin Forms assemblies and events.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    public class XamForms : BasePlatform
    {
        private readonly PackageIdentity[] _packageNames = new[]
        {
            new PackageIdentity("Xamarin.Forms", new NuGetVersion("3.3.0.967583")),
        };

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.XamForms;

        /// <inheritdoc />
        public async override Task Extract()
        {
            var packageUnzipPath = await NuGetPackageHelper.InstallPackages(_packageNames, Platform).ConfigureAwait(false);

            Log.Debug($"Package unzip path is {packageUnzipPath}");

            var xamarinForms =
                Directory.GetFiles(
                    packageUnzipPath,
                    "Xamarin.Forms.Core.dll",
                    SearchOption.AllDirectories);

            var latestVersion = xamarinForms.Last();
            Assemblies.Add(latestVersion);

            if (PlatformHelper.IsRunningOnMono())
            {
                CecilSearchDirectories.Add(
                    @"/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/xbuild-frameworks/.NETPortable/v4.5/Profile/Profile111");
                CecilSearchDirectories.Add(@"/Library/Frameworks/Mono.framework/External/xbuild-frameworks/MonoAndroid/v1.0/Facades");
            }
            else
            {
                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Facades");
            }
        }
    }
}
