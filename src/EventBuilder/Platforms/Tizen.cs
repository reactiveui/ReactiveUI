// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using NuGet;
using Polly;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    public class Tizen : BasePlatform
    {
        private const string _packageName = "Tizen.NET";

        public override AutoPlatform Platform => AutoPlatform.Tizen;

        public Tizen()
        {
            var packageUnzipPath = Environment.CurrentDirectory;

            Log.Debug($"Package unzip path is {packageUnzipPath}");

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, context) => {
                        Log.Warning(
                            "An exception was thrown whilst retrieving or installing {0}: {1}",
                            _packageName, exception);
                    });

            retryPolicy.Execute(() => {
                var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
                var packageManager = new PackageManager(repo, packageUnzipPath);
                var package = repo.FindPackagesById(_packageName).Single(x => x.Version.ToString() == "4.0.0");

                Log.Debug("Using Tizen.NET {0} released on {1}", package.Version, package.Published);
                Log.Debug("{0}", package.ReleaseNotes);

                packageManager.InstallPackage(package, ignoreDependencies: true, allowPrereleaseVersions: false);
            });

            var elmSharp = Directory.GetFiles(packageUnzipPath, "ElmSharp*.dll", SearchOption.AllDirectories);
            Assemblies.AddRange(elmSharp);

            var tizenNet = Directory.GetFiles(packageUnzipPath, "Tizen*.dll", SearchOption.AllDirectories);
            Assemblies.AddRange(tizenNet);

            CecilSearchDirectories.Add($"{packageUnzipPath}\\Tizen.NET.4.0.0\\build\\tizen40\\ref");
            CecilSearchDirectories.Add($"{packageUnzipPath}\\Tizen.NET.4.0.0\\lib\\netstandard2.0");
        }
    }
}
