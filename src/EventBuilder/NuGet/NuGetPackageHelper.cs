using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.PackageManagement;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using Polly;
using Serilog;

namespace EventBuilder.NuGet
{
    /// <summary>
    /// A helper class for handling NuGet packages.
    /// </summary>
    public static class NuGetPackageHelper
    {
        /// <summary>
        /// Installs a nuget package into the specified directory.
        /// </summary>
        /// <param name="packageIdentities">The identities of the packages to find.</param>
        /// <param name="platform">The name of the platform.</param>
        /// <param name="framework">Optional framework parameter which will force NuGet to evaluate as the specified Framework. If null it will use .NET Standard 2.0.</param>
        /// <returns>The directory where the NuGet packages are unzipped to.</returns>
        public static async Task<string> InstallPackages(IEnumerable<PackageIdentity> packageIdentities, AutoPlatform platform, NuGetFramework framework = null)
        {
            var packageUnzipPath = Path.Combine(Path.GetTempPath(), "EventBuilder.NuGet", platform.ToString());
            if (!Directory.Exists(packageUnzipPath))
            {
                Directory.CreateDirectory(packageUnzipPath);
            }

            await Task.WhenAll(packageIdentities.Select(x => InstallPackage(x, packageUnzipPath, framework))).ConfigureAwait(false);

            return packageUnzipPath;
        }

        private static async Task InstallPackage(PackageIdentity packageIdentity, string packageRoot, NuGetFramework framework)
        {
            var packagesPath = Path.Combine(packageRoot, "packages");
            var settings = Settings.LoadDefaultSettings(packageRoot, null, new XPlatMachineWideSetting());
            var sourceRepositoryProvider = new SourceRepositoryProvider(settings);
            var folder = new FolderNuGetProject(packageRoot, new PackagePathResolver(packageRoot), framework ?? FrameworkConstants.CommonFrameworks.NetStandard20);
            var packageManager = new NuGetPackageManager(sourceRepositoryProvider, settings, packagesPath)
            {
                PackagesFolderNuGetProject = folder
            };

            var resolutionContext = new ResolutionContext(
                DependencyBehavior.Lowest,
                includePrelease: false,
                includeUnlisted: false,
                VersionConstraints.None);
            var projectContext = new NuGetProjectContext(settings);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, _, __) =>
                    {
                        Log.Warning(
                            "An exception was thrown whilst retrieving or installing {0}: {1}",
                            packageIdentity,
                            exception);
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                await packageManager.InstallPackageAsync(
                    packageManager.PackagesFolderNuGetProject,
                    packageIdentity,
                    resolutionContext,
                    projectContext,
                    sourceRepositoryProvider.GetDefaultRepositories(),
                    Array.Empty<SourceRepository>(),
                    CancellationToken.None).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}
