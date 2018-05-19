using System;
using System.IO;
using System.Linq;
using NuGet;
using Polly;
using Serilog;

namespace EventBuilder.Platforms
{
    public class XamEssentials : BasePlatform
    {
        private const string PackageSource = "https://packages.nuget.org/api/v2";
        private const string PackageName = "Xamarin.Essentials";
        private const string PackageVersion = "0.6.0-preview";
        private const string PackageAssembly = "Xamarin.Essentials.dll";

        public XamEssentials()
        {
            var packagePath = Environment.CurrentDirectory;

            Log.Debug("Package unzip path is {PackagePath}", packagePath);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, context) => {
                        Log.Warning("An exception was thrown whilst retrieving or installing {packageName}: {exception}", PackageName, exception);
                    });

            retryPolicy.Execute(() => {
                var packageRepository = PackageRepositoryFactory.Default.CreateRepository(PackageSource);
                var packageManager = new PackageManager(packageRepository, packagePath);

                var package = packageRepository
                    .FindPackagesById(PackageName)
                    .Single(x => x.Version.ToString() == PackageVersion);

                Log.Debug("Using Xamarin Essentials {Version} released on {Published}", package.Version, package.Published);
                Log.Debug("{ReleaseNotes}", package.ReleaseNotes);

                packageManager.InstallPackage(package, ignoreDependencies: true, allowPrereleaseVersions: true);
            });

            var assemblyFileName = Directory
                .GetFiles(packagePath, PackageAssembly, SearchOption.AllDirectories)
                .Last();

            Assemblies.Add(assemblyFileName);

            if (PlatformHelper.IsRunningOnMono()) {
                CecilSearchDirectories.Add(
                    @"/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/xbuild-frameworks/.NETPortable/v4.5/Profile/Profile111");
            } else {
                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETPortable\v4.5\Profile\Profile111");
            }
        }
    }
}
