// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using EventBuilder.CommandOptions;
using EventBuilder.Core;
using Serilog;
using Parser = CommandLine.Parser;

namespace EventBuilder.Console
{
    internal static class Program
    {
        private static string _referenceAssembliesLocation = PlatformHelper.IsRunningOnMono() ?
            @"/Library⁩/Frameworks⁩/Libraries/⁨mono⁩" :
            @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework";

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            // allow app to be debugged in visual studio.
            if (Debugger.IsAttached)
            {
                args = "generate-platform --platforms=uwp --output-path=test.txt".Split(' ');
            }

            var parserResult = new Parser(parserSettings => parserSettings.CaseInsensitiveEnumValues = true)
                .ParseArguments<CustomAssembliesCommandLineOptions, PlatformCommandLineOptions>(args);

            var result = await parserResult.MapResult(
                async (PlatformCommandLineOptions options) =>
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(options.ReferenceAssemblies))
                        {
                            _referenceAssembliesLocation = options.ReferenceAssemblies;
                        }

                        await EventGenerator.ExtractEventsFromAssemblies(options.OutputPath, _referenceAssembliesLocation, options.Platforms).ConfigureAwait(false);

                        return ExitCode.Success;
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex.ToString());
                        return ExitCode.Error;
                    }
                },
                async (CustomAssembliesCommandLineOptions options) =>
                {
                    try
                    {
                        await EventGenerator.ExtractEventsFromAssemblies(options.OutputPath, options.Assemblies, options.SearchDirectories).ConfigureAwait(false);

                        return ExitCode.Success;
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex.ToString());
                        return ExitCode.Error;
                    }
                },
                _ =>
                {
                    System.Console.WriteLine(HelpOutput(parserResult));
                    return Task.FromResult(ExitCode.Error);
                }).ConfigureAwait(false);

            return (int)result;
        }

        private static string HelpOutput(ParserResult<object> args)
        {
            var result = new StringBuilder();

            result.AppendLine("Welcome to the event builder.")
                .AppendLine("This application takes will generate IObservable event wrappers for assemblies")
                .AppendLine("Valid options are:")
                .AppendLine(HelpText.AutoBuild(args));

            return result.ToString();
        }
    }
}
