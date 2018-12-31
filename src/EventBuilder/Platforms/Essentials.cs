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
    /// Xamarin Essentials  platform.
    /// </summary>
    /// <seealso cref="EventBuilder.Platforms.BasePlatform" />
    public class Essentials : BasePlatform
    {
        private readonly PackageIdentity[] _packageNames = new[]
        {
            new PackageIdentity("Xamarin.Essentials", new NuGetVersion("1.0.0")),
        };

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.Essentials;

        /// <inheritdoc />
        public async override Task Extract()
        {
            var packageUnzipPath = await NuGetPackageHelper.InstallPackages(_packageNames, Platform).ConfigureAwait(false);

            Log.Debug($"Package unzip path is {packageUnzipPath}");

            var xamarinForms =
                Directory.GetFiles(
                    packageUnzipPath,
                    "Xamarin.Essentials.dll",
                    SearchOption.AllDirectories);

            var latestVersion = xamarinForms.First(x => x.Contains("netstandard1.0", StringComparison.InvariantCulture));
            Assemblies.Add(latestVersion);

            if (PlatformHelper.IsRunningOnMono())
            {
                CecilSearchDirectories.Add(
                    @"/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/xbuild-frameworks/.NETPortable/v4.5/Profile/Profile111");
            }
            else
            {
                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable\v4.5\Profile\Profile111");
            }
        }
    }
}
