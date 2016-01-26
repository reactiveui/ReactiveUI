using System.Collections.Generic;

namespace EventBuilder.Platforms
{
    public interface IPlatform
    {
        List<string> Assemblies { get; set; }

        // Cecil when run on Mono needs some direction as to the location of the platform specific MSCORLIB.
        List<string> CecilSearchDirectories { get; set; }
    }
}