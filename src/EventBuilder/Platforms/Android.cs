using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    public class Android : BasePlatform
    {
        public Android()
        {
            if (PlatformHelper.IsRunningOnMono()) {
                var sdks =
                    Directory.GetFiles(
                        @"/Library/Frameworks/Xamarin.Android.framework/Libraries/xbuild-frameworks/MonoAndroid",
                        "Mono.Android.dll", SearchOption.AllDirectories);

                var latestVersion = sdks.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
                CecilSearchDirectories.Add(
                    "/Library/Frameworks/Xamarin.Android.framework/Libraries/xbuild-frameworks/MonoAndroid/v1.0");
            } else {
                var assemblies = WindowsSearchPaths
                    .SelectMany(x => Directory.GetFiles(x, "Mono.Android.dll", SearchOption.AllDirectories));

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                var latestAssemblyPath = Path.GetDirectoryName(latestVersion);
                var v1AssemblyPath = Path.Combine(Directory.GetParent(latestAssemblyPath).FullName, "v1");
                CecilSearchDirectories.Add(latestAssemblyPath);
                CecilSearchDirectories.Add(v1AssemblyPath);
            }
        }

        private static IEnumerable<string> WindowsSearchPaths => new[] {
            @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\MonoAndroid",
            @"C:\Program Files(x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid",
            @"C:\Program Files(x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid"
        };
    }
}