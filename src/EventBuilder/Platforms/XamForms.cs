// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using NuGet;
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
        private const string _packageName = "Xamarin.Forms";

        /// <summary>
        /// Initializes a new instance of the <see cref="XamForms"/> class.
        /// </summary>
        public XamForms()
        {
            var packageUnzipPath = Environment.CurrentDirectory;

            Log.Debug("Package unzip path is {PackageUnzipPath}", packageUnzipPath);

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, context) =>
                    {
                        Log.Warning(
                            "An exception was thrown whilst retrieving or installing {packageName}: {exception}",
                            _packageName,
                            exception);
                    });

            retryPolicy.Execute(() =>
            {
                var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

                var packageManager = new PackageManager(repo, packageUnzipPath);

                var package = repo.FindPackagesById(_packageName).Single(x => x.Version.ToString() == "3.3.0.967583");

                Log.Debug("Using Xamarin Forms {Version} released on {Published}", package.Version, package.Published);
                Log.Debug("{ReleaseNotes}", package.ReleaseNotes);

                packageManager.InstallPackage(package, ignoreDependencies: true, allowPrereleaseVersions: false);
            });

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
                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.1\Facades");
            }
        }

        /// <inheritdoc />
        public override AutoPlatform Platform => AutoPlatform.XamForms;
    }
}
