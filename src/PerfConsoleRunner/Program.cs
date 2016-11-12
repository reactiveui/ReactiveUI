using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfConsoleRunner
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var file = (new StackTrace(true).GetFrame(0)).GetFileName();
            var solutionDir = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(file), "..")).FullName;

#if DEBUG
            var relDir = "Debug";
#else
            var relDir = "Release";
#endif

            var testAssembly = Path.Combine(solutionDir, @"ReactiveUI.Tests\bin\Debug\Net45\ReactiveUI.Tests_Net45.dll")
                .Replace("Debug", relDir);

            Xunit.ConsoleClient.Program.Main(new[] { testAssembly });
            return 0;
        }
    }
}
