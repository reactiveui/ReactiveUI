using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    // ReSharper disable once InconsistentNaming
    public class iOS : BasePlatform
    {
        public iOS()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                var assembly =
                    @"/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
            }
            else
            {
                var assemblies =
                    Directory.GetFiles(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Xamarin.iOS",
                        "Xamarin.iOS.dll", SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }
        }
    }
}