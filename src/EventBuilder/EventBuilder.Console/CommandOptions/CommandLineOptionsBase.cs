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
    /// A base class for commonly shared options.
    /// </summary>
    public abstract class CommandLineOptionsBase
    {
        /// <summary>
        /// Gets or sets the path where to output the contents.
        /// </summary>
        [Option('o', "output-path", Required = true, HelpText = "The file path where to output the contents.")]
        public string OutputPath { get; set; }
    }
}
