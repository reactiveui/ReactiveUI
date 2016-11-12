using System.IO;
using System.Linq;

namespace EventBuilder.Platforms
{
    // ReSharper disable once InconsistentNaming
    public class Mac : BasePlatform
    {
        public Mac()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                var assembly =
                    @"/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll";
                Assemblies.Add(assembly);

                CecilSearchDirectories.Add(Path.GetDirectoryName(assembly));
            }
            else
            {
                var assemblies =
                    Directory.GetFiles(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Xamarin.Mac",
                        "Xamarin.Mac.dll", SearchOption.AllDirectories);

                var latestVersion = assemblies.Last();
                Assemblies.Add(latestVersion);

                CecilSearchDirectories.Add(Path.GetDirectoryName(latestVersion));
            }
        }
    }
}