using System;
using System.IO;
using System.Linq;
using NuGet;
using Polly;
using Serilog;

namespace EventBuilder.Platforms
{
    public class Mac : BasePlatform
    {

      private const string _packageName = "ReactiveUI-TargetingPack-XamarinMac";

      public Mac()
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
              var repo = PackageRepositoryFactory.Default.CreateRepository("https://www.myget.org/F/reactiveui/api/v2");

              var packageManager = new PackageManager(repo, packageUnzipPath);

              var package = repo.FindPackagesById(_packageName).First(x => x.IsLatestVersion);

              packageManager.InstallPackage(package.Id);

              Log.Debug("Using {_packageName} {Version} released on {Published}", _packageName, package.Version, package.Published);
              Log.Debug("{ReleaseNotes}", package.ReleaseNotes);
          });

          var xamarinMac =
              Directory.GetFiles(packageUnzipPath,
                  "Xamarin.Mac.dll", SearchOption.AllDirectories).First(x => x.Contains("XamarinMacMobile"));

            Assemblies.Add(xamarinMac);

            CecilSearchDirectories.Add(Path.GetDirectoryName(xamarinMac));
        }
    }
}
