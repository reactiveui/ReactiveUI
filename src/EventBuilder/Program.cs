// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using EventBuilder.Platforms;
using EventBuilder.Reflection;
using EventBuilder.Reflection.Resolvers;
using Serilog;
using Parser = CommandLine.Parser;

namespace EventBuilder
{
    internal static class Program
    {
        private static string _referenceAssembliesLocation = PlatformHelper.IsRunningOnMono() ?
            @"/Library⁩/Frameworks⁩/Libraries/⁨mono⁩" :
            @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework";

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            // allow app to be debugged in visual studio.
            if (Debugger.IsAttached)
            {
                args = "--platform=essentials --output-path=test.txt".Split(' ');
            }

            await new Parser(parserSettings => parserSettings.CaseInsensitiveEnumValues = true).ParseArguments<CommandLineOptions>(args).MapResult(
                async options =>
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(options.ReferenceAssemblies))
                        {
                            _referenceAssembliesLocation = options.ReferenceAssemblies;
                            Log.Debug($"Using {_referenceAssembliesLocation} instead of the default reference assemblies location.");
                        }

                        IPlatform platform = null;
                        switch (options.Platform)
                        {
                            case AutoPlatform.None:
                                if (options.Assemblies.Any() == false)
                                {
                                    throw new Exception("Assemblies to be used for manual generation were not specified.");
                                }

                                platform = new Bespoke();
                                platform.Assemblies.AddRange(options.Assemblies);

                                if (PlatformHelper.IsRunningOnMono())
                                {
                                    platform.SearchDirectories.AddRange(platform.Assemblies.Select(Path.GetDirectoryName).Distinct().ToList());
                                }
                                else
                                {
                                    platform.SearchDirectories.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");
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

                            case AutoPlatform.TVOS:
                                platform = new TVOS(_referenceAssembliesLocation);
                                break;

                            case AutoPlatform.WPF:
                                platform = new WPF();
                                break;

                            case AutoPlatform.XamForms:
                                platform = new XamForms();
                                break;

                            case AutoPlatform.Tizen4:
                                platform = new Tizen();
                                break;

                            case AutoPlatform.UWP:
                                platform = new UWP();
                                break;

                            case AutoPlatform.Winforms:
                                platform = new Winforms();
                                break;

                            case AutoPlatform.Essentials:
                                platform = new Essentials();
                                break;

                            default:
                                throw new ArgumentException($"Platform not {options.Platform} supported");
                        }

                        await EventGenerator.ExtractEventsFromAssemblies(options.OutputPath, platform).ConfigureAwait(false);

                        Environment.Exit((int)ExitCode.Success);
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex.ToString());

                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                    }

                    Environment.Exit((int)ExitCode.Error);
                },
                _ => Task.CompletedTask).ConfigureAwait(false);
        }
    }
}
