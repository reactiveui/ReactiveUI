// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace EventBuilder.CommandOptions
{
    /// <summary>
    /// A options class that represent when the user wants to generate from their own custom list of assemblies and search directories.
    /// </summary>
    [Verb("generate-custom", HelpText = "Generate from own specified assembly paths and search directories.")]
    public class CustomAssembliesCommandLineOptions : CommandLineOptionsBase
    {
        /// <summary>
        /// Gets or sets the reference assemblies.
        /// </summary>
        [Option('s', "search-directories", Required = true, HelpText = "Specify a search directories where to search for additional support libraries.")]
        public IEnumerable<string> SearchDirectories { get; set; }

        /// <summary>
        /// Gets or sets the assemblies.
        /// Manual generation using the specified assemblies. Use with --platform=NONE.
        /// </summary>
        [Option('a', "assemblies", Required = true, HelpText = "List of assemblies to process.")]
        public IEnumerable<string> Assemblies { get; set; }
    }
}
