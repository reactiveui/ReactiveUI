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
        /// Gets or sets the platform.
        /// </summary>
        [Option('p', "platform", Required = true, HelpText = "Platform to automatically generate. Possible options include: NONE, ANDROID, IOS, WPF, MAC, TIZEN, UWP, XAMFORMS, WINFORMS, TVOS")]
        public AutoPlatform Platform { get; set; }

        /// <summary>
        /// Gets or sets the path where to output the contents.
        /// </summary>
        [Option('o', "output-path", Required = true, HelpText = "The file path where to output the contents.")]
        public string OutputPath { get; set; }

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
        [Option('a', "assemblies", Required = false, HelpText = "List of assemblies to process. Used for the NONE option")]
#pragma warning disable CA2227 // Collection properties should be read only
        public IEnumerable<string> Assemblies { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
