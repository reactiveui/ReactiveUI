using System.Collections.Generic;

namespace EventBuilder.Platforms
{
    public class BasePlatform : IPlatform
    {
        public BasePlatform()
        {
            Assemblies = new List<string>();
            CecilSearchDirectories = new List<string>();
        }

        public List<string> Assemblies { get; set; }
        public List<string> CecilSearchDirectories { get; set; }
    }
}