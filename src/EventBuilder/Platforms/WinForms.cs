using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBuilder.Platforms
{
    public class WinForms : BasePlatform
    {
        public WinForms()
        {
            if (PlatformHelper.IsRunningOnMono())
            {
                throw new NotSupportedException("Building events for WPF on Mac is not implemented yet.");
            }
            else
            {
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\WindowsBase.dll");
                Assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Windows.Forms.dll");

                CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");

            }

        }
    }
}
