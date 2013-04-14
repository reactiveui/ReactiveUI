using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

#if SILVERLIGHT
using System.Windows;
#elif WINRT
using Windows.ApplicationModel;
#endif

namespace ReactiveUI.Xaml
{
    internal static class RealUnitTestDetector
    {
        public static bool InUnitTestRunner(string[] testAssemblies, string[] designEnvironments)
        {
            if (DesignModeDetector.IsInDesignMode()) return true;

#if SILVERLIGHT
            // NB: Deployment.Current.Parts throws an exception when accessed in Blend
            try {
                var ret = Deployment.Current.Parts.Any(x =>
                    testAssemblies.Any(name => x.Source.ToUpperInvariant().Contains(name)));

                if (ret) {
                    return ret;
                }
            } catch(Exception) {
                return true;
            }

            try {
                if (Application.Current.RootVisual != null && System.ComponentModel.DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual)) {
                    return false;
                }
            } catch {
                return true;
            }

            return false;
#elif WINRT
            if (DesignMode.DesignModeEnabled) return true;

            var depPackages = Package.Current.Dependencies.Select(x => x.Id.FullName);
            if (depPackages.Any(x => testAssemblies.Any(name => x.ToUpperInvariant().Contains(name)))) return true;


            var fileTask = Task.Factory.StartNew(async () => {
                var files = await Package.Current.InstalledLocation.GetFilesAsync();
                return files.Select(x => x.Path).ToArray();
            }, TaskCreationOptions.HideScheduler).Unwrap();

            return fileTask.Result.Any(x => testAssemblies.Any(name => x.ToUpperInvariant().Contains(name)));
#else
            // Try to detect whether we're in design mode - bonus points, 
            // without access to any WPF references :-/
            var entry = Assembly.GetEntryAssembly();
            if (entry != null) {
                var exeName = (new FileInfo(entry.Location)).Name.ToUpperInvariant();

                if (designEnvironments.Any(x => x.Contains(exeName))) {
                    return true;
                }
            }

            return AppDomain.CurrentDomain.GetAssemblies().Any(x =>
                testAssemblies.Any(name => x.FullName.ToUpperInvariant().Contains(name)));
#endif
        }

        public static bool InUnitTestRunner()
        {
            // XXX: This is hacky and evil, but I can't think of any better way
            // to do this
            string[] testAssemblies = new[] {
                "CSUNIT",
                "NUNIT",
                "XUNIT",
                "MBUNIT",
                "TESTDRIVEN",
                "QUALITYTOOLS.TIPS.UNITTEST.ADAPTER",
                "QUALITYTOOLS.UNITTESTING.SILVERLIGHT",
                "PEX",
                "MSBUILD",
                "NBEHAVE",
                "TESTPLATFORM",
            };

            string[] designEnvironments = new[] {
                "BLEND.EXE",
                "XDESPROC.EXE",
            };

            return InUnitTestRunner(testAssemblies, designEnvironments);
        }
    }
}