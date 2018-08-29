// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace EventBuilder
{
    /// <summary>
    /// Command line options for the event builder.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the last state of the parser.
        /// </summary>
        [ParserState]
        public IParserState LastParserState { get; set; }

        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        [Option('p', "platform", Required = true, HelpText = "Platform to automatically generate. Possible options include: NONE, ANDROID, IOS, WPF, MAC, TIZEN, UWP, XAMFORMS, WINFORMS, TVOS")]
        public AutoPlatform Platform { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        [Option('t', "template", Required = false, HelpText = "Specify another mustache template other than the default.")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the reference assemblies.
        /// </summary>
        [Option('r', "reference", Required = false, HelpText = "Specify a Reference Assemblies location to override the default")]
        public string ReferenceAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the assemblies.
        /// Manual generation using the specified assemblies. Use with --platform=NONE.
        /// </summary>
        [ValueList(typeof(List<string>))]
        public List<string> Assemblies { get; set; }

        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <returns>The help text usage.</returns>
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(
                this,
                current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
