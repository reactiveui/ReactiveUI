// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using EventBuilder.Core;

namespace EventBuilder.CommandOptions
{
    /// <summary>
    /// Command line options for the platform based generation.
    /// </summary>
    [Verb("generate-platform", HelpText = "Generate from a predetermined platform.")]
    public class PlatformCommandLineOptions : CommandLineOptionsBase
    {
        /// <summary>
        /// Gets or sets the platform.
        /// </summary>
        [Option('p', "platforms", Separator = ',', Required = true, HelpText = "Platform to automatically generate. Possible options include: ANDROID, IOS, WPF, MAC, TIZEN, UWP, XAMFORMS, WINFORMS, TVOS, ESSENTIALS")]
        public IEnumerable<AutoPlatform> Platforms { get; set; }

                /// <summary>
        /// Gets or sets the reference assemblies.
        /// </summary>
        [Option('r', "reference", Required = false, HelpText = "Specify a Reference Assemblies location to override the default.")]
        public string ReferenceAssemblies { get; set; }
    }
}
