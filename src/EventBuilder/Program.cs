using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using EventBuilder.Cecil;
using EventBuilder.Platforms;
using Mono.Cecil;
using Nustache.Core;
using Serilog;
using Parser = CommandLine.Parser;

namespace EventBuilder
{
    internal class Program
    {
        /// <summary>
        ///     The exit/return code (aka %ERRORLEVEL%) on application exit.
        /// </summary>
        public enum ExitCode
        {
            Success = 0,
            Error = 1
        }

        private static string _mustacheTemplate = "DefaultTemplate.mustache";
        private static string _referenceAssembliesLocation = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework";

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var options = new CommandLineOptions();

            // allow app to be debugged in visual studio.
            if (Debugger.IsAttached) {
                //args = "--help ".Split(' ');
                args = "--platform=ios".Split(' ');
                //args = new[]
                //{
                //    "--platform=none",
                //    @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Xamarin.iOS\v1.0\Xamarin.iOS.dll"
                //};
            }

            // Parse in 'strict mode'; i.e. success or quit
            if (Parser.Default.ParseArgumentsStrict(args, options)) {
                try {
                    if (!string.IsNullOrWhiteSpace(options.Template)) {
                        _mustacheTemplate = options.Template;

                        Log.Debug("Using {template} instead of the default template.", _mustacheTemplate);
                    }

                    if (!string.IsNullOrWhiteSpace(options.ReferenceAssemblies)) {
                        _referenceAssembliesLocation = options.ReferenceAssemblies;
                        Log.Debug($"Using {_referenceAssembliesLocation} instead of the default reference assemblies location.");
                    }

                    IPlatform platform = null;
                    switch (options.Platform) {
                    case AutoPlatform.None:
                        if (!options.Assemblies.Any()) {
                            throw new Exception("Assemblies to be used for manual generation were not specified.");
                        }

                        platform = new Bespoke();
                        platform.Assemblies = options.Assemblies;

                        if (PlatformHelper.IsRunningOnMono()) {
                            platform.CecilSearchDirectories =
                                platform.Assemblies.Select(x => Path.GetDirectoryName(x)).Distinct().ToList();
                        } else {
                            platform.CecilSearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");
                        }
                        break;

                    case AutoPlatform.Android:
                        platform = new Android(_referenceAssembliesLocation);
                        break;

                    case AutoPlatform.iOS:
                        platform = new iOS(_referenceAssembliesLocation);
                        break;

                    case AutoPlatform.Mac:
                        platform = new Mac(_referenceAssembliesLocation);
                        break;

                    case AutoPlatform.NET45:
                        platform = new Net45();
                        break;

                    case AutoPlatform.XamForms:
                        platform = new XamForms();
                        break;

                    case AutoPlatform.UWP:
                        platform = new UWP();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                    }

                    ExtractEventsFromAssemblies(platform);

                    Environment.Exit((int)ExitCode.Success);
                } catch (Exception ex) {
                    Log.Fatal(ex.ToString());
                }
            }

            Environment.Exit((int)ExitCode.Error);
        }

        public static void ExtractEventsFromAssemblies(IPlatform platform)
        {
            Log.Debug("Extracting events from the following assemblies: {assemblies}", platform.Assemblies);

            Log.Debug("Using the following search directories: {assemblies}", platform.CecilSearchDirectories);
            var targetAssemblyDirs = platform.CecilSearchDirectories;

            var rp = new ReaderParameters
            {
                AssemblyResolver = new PathSearchAssemblyResolver(targetAssemblyDirs.ToArray())
            };

            var targetAssemblies = platform.Assemblies.Select(x => AssemblyDefinition.ReadAssembly(x, rp)).ToArray();

            Log.Debug("Using {template} as the mustache template", _mustacheTemplate);
            var template = File.ReadAllText(_mustacheTemplate, Encoding.UTF8);

            var namespaceData = EventTemplateInformation.Create(targetAssemblies);

            var delegateData = DelegateTemplateInformation.Create(targetAssemblies);

            var result = Render.StringToString(template,
                new { Namespaces = namespaceData, DelegateNamespaces = delegateData })
                .Replace("System.String", "string")
                .Replace("System.Object", "object")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("`1", "")
                .Replace("`2", "")
                .Replace("`3", "");

            Console.WriteLine(result);
        }
    }
}