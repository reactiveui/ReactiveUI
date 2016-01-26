using NuGet;
using Polly;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    public class XamForms : BasePlatform
    {
        private const string _packageName = "Xamarin.Forms";

        public XamForms()
        {
            var packageUnzipPath = Environment.CurrentDirectory;

            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, context) =>
                    {
                        Log.Warning(
                            "An exception was thrown whilst retrieving or installing {packageName}: {exception}",
                            _packageName, exception);
                    });

            retryPolicy.Execute(() =>
            {
                var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

                var packageManager = new PackageManager(repo, packageUnzipPath);

                var package = repo.FindPackagesById(_packageName).Single(x => x.IsLatestVersion);

                packageManager.InstallPackage(package.Id);

                Log.Debug("Using Xamarin Forms {Version} released on {Published}", package.Version, package.Published);
                Log.Debug("{ReleaseNotes}", package.ReleaseNotes);
            });

            var xamarinForms =
                Directory.GetFiles(packageUnzipPath,
                    "Xamarin.Forms.Core.dll", SearchOption.AllDirectories);

            var latestVersion = xamarinForms.Last();
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