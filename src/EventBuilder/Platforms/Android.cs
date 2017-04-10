using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    public class Android : BasePlatform
    {
        public Android(string referenceAssembliesLocation)
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

                var assemblies =
                   Directory.GetFiles(Path.Combine(referenceAssembliesLocation, "MonoAndroid"),
                       "Mono.Android.dll", SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
                CecilSearchDirectories.Add(Path.Combine(referenceAssembliesLocation, "MonoAndroid", "v1.0"));
            }
        }
    }
}