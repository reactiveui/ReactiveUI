// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace EventBuilder
{
    public enum AutoPlatform
    {
        None,
        Android,
        iOS,
        Mac,
        WPF,
        XamForms,
        UWP
    }

    public class CommandLineOptions
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [Option('p', "platform", Required = true,
            HelpText =
                "Platform to automatically generate. Possible options include: NONE, ANDROID, IOS, WPF, MAC, UWP, XAMFORMS"
            )]
        public AutoPlatform Platform { get; set; }

        [Option('t', "template", Required = false,
            HelpText = "Specify another mustache template other than the default.")]
        public string Template { get; set; }

        [Option('r', "reference", Required = false, HelpText = "Specify a Reference Assemblies location to override the default")]
        public string ReferenceAssemblies { get; set; }

        // Manual generation using the specified assemblies. Use with --platform=NONE.
        [ValueList(typeof(List<string>))]
        public List<string> Assemblies { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}